using Entities.Characters.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.CharacterPortraits
{
    public class HealthTrackerUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject healthBoxPrefab;
        private List<HealthBox> healthBoxes = new List<HealthBox>();

        [SerializeField, Range(0, Consts.MAXIMUM_POSSIBLE_HEALTH)]
        private int _maxHealth;
        [SerializeField]
        private int _superficialDamage, _aggravatedDamage;
        internal int MaxHealth
        {
            get
            {
                return _maxHealth;
            }
            set
            {
                _maxHealth = value;
                UpdateHealthTracker();
            }
        }
        internal int SuperficialDamage
        {
            get
            {
                return _superficialDamage;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MaxHealth);
                _superficialDamage = value;
                UpdateHealthTracker();
            }
        }
        internal int AggravatedDamage
        {
            get
            {
                return _aggravatedDamage;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MaxHealth);
                _aggravatedDamage = value;
                UpdateHealthTracker();
            }
        }

        //private void Start()
        //{
        //    foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        //    {
        //        if (!t.gameObject.Equals(this.gameObject))
        //            Destroy(t.gameObject);
        //    }
        //}

        internal void SetHealthData(HealthData data)
        {
            _maxHealth = data.maxHealth;
            _superficialDamage = data.superficialDamage;
            _aggravatedDamage = data.aggravatedDamage;
            UpdateHealthTracker();
        }

        [ContextMenu("Update health tracker")]
        private void UpdateHealthTracker()
        {
            Debug.Log("MaxHealth = " + MaxHealth + "; healtBoxes.Count = " + healthBoxes.Count);
            if (MaxHealth > healthBoxes.Count)
            {
                int difference = MaxHealth - healthBoxes.Count;
                for (int i = 0; i < difference; i++)
                {
                    Debug.Log("Health box added");
                    HealthBox box = Instantiate(healthBoxPrefab, gameObject.transform).GetComponent<HealthBox>();
                    box.State = HealthBoxType.Healthy;
                    healthBoxes.Add(box);
                }
            } 
            else if (MaxHealth < healthBoxes.Count)
            {
                Debug.Log("removing health boxes");
                int difference = healthBoxes.Count - MaxHealth;
                RemoveHealthBoxes(difference);
            }
            foreach (HealthBox box in healthBoxes)
            {
                box.State = HealthBoxType.Healthy;
            }
            for (int i = 0; i < AggravatedDamage; i++)
            {
                HealthBox box = healthBoxes[i];
                if (box.State == HealthBoxType.Healthy)
                    box.State = HealthBoxType.Aggravated;
            }
            int superficialBoxes = MaxHealth - AggravatedDamage >= SuperficialDamage ? 
                SuperficialDamage : 
                MaxHealth - AggravatedDamage;
            for (int i = AggravatedDamage; i < superficialBoxes + AggravatedDamage; i++)
            {
                HealthBox box = healthBoxes[i];
                if (box.State == HealthBoxType.Healthy)
                    box.State = HealthBoxType.Superficial;
            }
        }

        // It's a terrible piece of code, I know. ~Switchu
        private void RemoveHealthBoxes(int amount)
        {
            while (amount > 0)
            {
                HealthBox box;
                box = healthBoxes.Find(b => b.State == HealthBoxType.Healthy);
                if (box != null)
                {
                    healthBoxes.Remove(box);
                    Destroy(box);
                    amount--;
                    continue;
                }
                box = healthBoxes.Find(b => b.State == HealthBoxType.Superficial);
                if (box != null)
                {
                    healthBoxes.Remove(box);
                    Destroy(box);
                    amount--;
                    continue;
                }
                box = healthBoxes.Find(b => b.State == HealthBoxType.Aggravated);
                if (box != null)
                {
                    healthBoxes.Remove(box);
                    Destroy(box);
                    amount--;
                    continue;
                }
                break;
            }
        }
    }
}
