using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Journal;
using UnityEngine;

namespace SunsetSystems.MainMenu
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private PlayerCharacterBackground selectedBackground;
        [SerializeField, ReadOnly]
        private BodyType selectedBodyType;
        [SerializeField]
        private CharacterStats stats;
        [SerializeField]
        private int startSceneIndex;

        private void Reset()
        {
            if (!stats)
                stats = ScriptableObject.CreateInstance<CharacterStats>();
        }

        private void Start()
        {
            if (!stats)
                stats = ScriptableObject.CreateInstance<CharacterStats>();
        }

        public void SelectBackground(PlayerCharacterBackground selectedBackground)
        {
            this.selectedBackground = selectedBackground;
        }

        public void SelectBodyType(BodyType selectedBodyType)
        {
            this.selectedBodyType = selectedBodyType;
        }

        public void SetAttribueValue(AttributeType attribute, int value)
        {
            stats.GetAttribute(attribute).SetValue(value);
            Debug.LogWarning("Attribute: " + attribute + "; current value: " + stats.GetAttribute(attribute).GetValue() + "; expected value: " + value);
        }

        public void SetSkillValue(SkillType skill, int value)
        {
            stats.GetSkill(skill).SetValue(value);
            Debug.LogWarning("Skill: " + skill + "; current value: " + stats.GetSkill(skill).GetValue() + "; expected value: " + value);
        }
    }
}
