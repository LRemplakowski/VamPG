using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Formation.Data;
using UnityEngine;

namespace SunsetSystems.Formation.UI
{
    [CreateAssetMenu(fileName = "New Predefined Formation", menuName = "Systems/Formations/Predefined Formation")]
    public class PredefinedFormation : ScriptableObject
    {
        [SerializeField]
        private Vector3[] formationPositions = new Vector3[6];
        [SerializeField]
        private Sprite formationIcon;

        public FormationData GetData()
        {
            return new FormationData(formationPositions);
        }

        public Sprite GetSprite()
        {
            return formationIcon;
        }
    }
}
