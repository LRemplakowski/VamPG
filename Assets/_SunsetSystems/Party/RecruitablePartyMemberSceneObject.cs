using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Game;
using UnityEngine;

namespace SunsetSystems.Party
{
    public class RecruitablePartyMemberSceneObject : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private ICreatureReferences _references;

        private void OnValidate()
        {
            if (_references == null && gameObject.TryGetComponent(out _references))
                Debug.Log($"Cached recruitable party member references for {gameObject.name}!");              
        }

        // Start is called before the first frame update
        private void Start()
        {
            GameManager.Instance.OnLevelStart += DisableGameObjectIfAlreadyRecruited;
        }

        private void DisableGameObjectIfAlreadyRecruited()
        {
            if (PartyManager.Instance.IsRecruitedMember(_references.CreatureData.DatabaseID))
                gameObject.SetActive(false);
        }
    }
}
