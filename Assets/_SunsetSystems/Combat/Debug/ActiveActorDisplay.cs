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

        // Start is called before the first frame update
        void Start()
        {
            if (!text)
                text = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            if (CombatManager.CurrentActiveActor != null)
                text.text = "Active actor:\n" + CombatManager.CurrentActiveActor.gameObject.name;
        }
    }
}
