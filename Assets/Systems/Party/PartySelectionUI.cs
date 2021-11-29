using Entities.Characters.Data;
using System.Collections;
using System.Collections.Generic;
using Systems.Management;
using UnityEngine;

namespace Systems.Party.UI
{
    public class PartySelectionUI : UIWindow
    {
        [SerializeField]
        private Transform selectedMembersParent, availableMembersParent;
        [SerializeField]
        private PartySelectionElement memberElementPrefab;
        [SerializeField, ReadOnly]
        private CreatureUIData[] selectedMembers, availableMembers;

        private PartyManager partyManager;

        // Start is called before the first frame update
        void OnEnable()
        {
            if (partyManager == null)
                partyManager = ReferenceManager.GetManager<PartyManager>();
            DestroyChildren(selectedMembersParent);
            DestroyChildren(availableMembersParent);
            FillMemberUI(selectedMembersParent, selectedMembers);
            FillMemberUI(availableMembersParent, availableMembers);
        }

        private void FillMemberUI(Transform parent, CreatureUIData[] memberData)
        {
            foreach (CreatureUIData data in memberData)
            {
                PartySelectionElement element = Instantiate(memberElementPrefab, parent);
                element.Initialize(data);
            }
        }

        private void DestroyChildren(Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
