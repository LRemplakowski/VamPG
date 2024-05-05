using System.Collections.Generic;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{

    public class PlayerAndActivationTime
    {
        public Dictionary<GameObject, float> playerToLastTriggerActivationTime = new Dictionary<GameObject, float>();        
        public PlayerAndActivationTime(GameObject player, float lastTriggerActivationTime)
        {
            playerToLastTriggerActivationTime[player] = lastTriggerActivationTime;
        }
    }


public class PlayerToCameraRaycastTriggerManager : MonoBehaviour
{
        private Dictionary<ManualTriggerByParent, PlayerAndActivationTime> triggerToPlayerAndActivationTime = new Dictionary<ManualTriggerByParent, PlayerAndActivationTime>();
        
        public List<GameObject> playerList;
        public float timeUntilExit = 0.1f;
        public bool ShowDebugRays = false;


        void Update()
        {
            Camera cam = Camera.main;
                        

            if (cam != null && playerList != null && playerList.Count > 0)
            {
                // iterating through all players that are raycasting to the camera
                foreach (GameObject player in playerList)
                {
                    if(ShowDebugRays)
                    {
                        Debug.DrawRay(player.transform.position, cam.transform.position - player.transform.position, Color.magenta);
                    }


                    // iterates through all raycast hits and if one hit or its parents contain the "ManualTriggerByParent" component, ActivateTrigger(player) 
                    // will be called with the current player as it's argument. It's a bit complex but we have to keep track of which player already activated
                    // which trigger, and fill out the dictionaries if certain trigger and/or player didn't exist in it yet
                    float distancePlayerToCamera = Vector3.Distance(cam.transform.position, player.transform.position);
                    RaycastHit[] hits = Physics.RaycastAll(player.transform.position, cam.transform.position - player.transform.position, distancePlayerToCamera);
                    foreach (RaycastHit hit in hits)
                    {   
                        ManualTriggerByParent manualTriggerByParent = hit.transform.gameObject.GetComponentInParent<ManualTriggerByParent>();
                        if(manualTriggerByParent != null) 
                        {
                            if (!triggerToPlayerAndActivationTime.ContainsKey(manualTriggerByParent))
                            {
                                triggerToPlayerAndActivationTime[manualTriggerByParent] = new PlayerAndActivationTime(player, Time.realtimeSinceStartup);
                                manualTriggerByParent.ActivateTrigger(player.gameObject.GetComponent<Collider>());
                            } 
                            else if (!triggerToPlayerAndActivationTime[manualTriggerByParent].playerToLastTriggerActivationTime.ContainsKey(player))
                            {
                                triggerToPlayerAndActivationTime[manualTriggerByParent].playerToLastTriggerActivationTime[player] = Time.realtimeSinceStartup;
                                manualTriggerByParent.ActivateTrigger(player.gameObject.GetComponent<Collider>());

                            }
                            else
                            {
                                float lastTriggerActivationTime = triggerToPlayerAndActivationTime[manualTriggerByParent].playerToLastTriggerActivationTime[player];
                                // this checks if we haven't recently called the activate function for the particular player and trigger, as multiple activation calls in
                                // a row breaks the shader code
                                if (Time.realtimeSinceStartup - lastTriggerActivationTime > timeUntilExit)
                                {
                                    manualTriggerByParent.ActivateTrigger(player.GetComponent<Collider>());

                                }
                                triggerToPlayerAndActivationTime[manualTriggerByParent].playerToLastTriggerActivationTime[player] = Time.realtimeSinceStartup;
                            }
                        }
                    }
                }


                //  this checks if any triggers weren't activated recently and so automatically calls the deactive functions for specific triggers and players
                if (triggerToPlayerAndActivationTime.Count > 0)
                {
                    List<ManualTriggerByParent> keyList = new List<ManualTriggerByParent>(triggerToPlayerAndActivationTime.Keys);
                    foreach (ManualTriggerByParent trigger in keyList)
                    {
                        List<GameObject> playerList = new List<GameObject>(triggerToPlayerAndActivationTime[trigger].playerToLastTriggerActivationTime.Keys);
                        foreach (GameObject player in playerList)
                        {
                            float lastTriggerActivationTime = triggerToPlayerAndActivationTime[trigger].playerToLastTriggerActivationTime[player];

                            //  If this is true, we can call the deactive function as we know that this particular player doesn't raycast onto
                            //  this specific trigger gameobject anymore. 
                            if (Time.realtimeSinceStartup - lastTriggerActivationTime > timeUntilExit)
                            {
                                trigger.DeactivateTrigger(player.GetComponent<Collider>());
                                if (triggerToPlayerAndActivationTime[trigger].playerToLastTriggerActivationTime.Keys.Count > 1)
                                {
                                    triggerToPlayerAndActivationTime[trigger].playerToLastTriggerActivationTime.Remove(player);
                                }
                                else
                                {
                                    triggerToPlayerAndActivationTime.Remove(trigger);
                                }
                            }
                        }
                    }


                }

            }
        }
    }
}