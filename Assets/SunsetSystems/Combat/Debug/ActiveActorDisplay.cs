using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SunsetSystems.Combat.Dev
{
    public class ActiveActorDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private CombatManager combatManager;

        // Start is called before the first frame update
        void Start()
        {
            if (!text)
                text = GetComponentInChildren<TextMeshProUGUI>();
            if (!combatManager)
                combatManager = CombatManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (combatManager.CurrentActiveActor != null)
                text.text = "Active actor:\n" + combatManager.CurrentActiveActor.gameObject.name;
        }
    }
}
