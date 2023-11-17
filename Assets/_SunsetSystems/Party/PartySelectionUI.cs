using SunsetSystems.Entities.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Party.UI
{
    public class PartySelectionUI : UIWindow
    {
        [SerializeField]
        private Transform selectedMembersParent, availableMembersParent;
        [SerializeField]
        private PartySelectionElement memberElementPrefab;
        [SerializeField, ReadOnly]
        private CreatureUIData[] selectedMembers, availableMembers;

        // Start is called before the first frame update
        void OnEnable()
        {
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
