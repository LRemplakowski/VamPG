using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FadeCameraObstruction : MonoBehaviour
{
    [SerializeField]
    private Camera myCamera;
    [SerializeField]
    private Transform cameraFocus;
    private float raycastRange;
    [SerializeField]
    private LayerMask ignoreObstruction;

    // Start is called before the first frame update
    void Start()
    {
        myCamera = GetComponent<Camera>();
        if (cameraFocus != null)
        {
            raycastRange = Vector3.Distance(transform.position, cameraFocus.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Ray ray = myCamera.ScreenPointToRay(new Vector2(myCamera.pixelWidth / 2, myCamera.pixelHeight / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, ignoreObstruction, QueryTriggerInteraction.Ignore))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer)
            {
                foreach (Material mat in renderer.materials)
                {
                    
                }
            }
        }
    }
}
