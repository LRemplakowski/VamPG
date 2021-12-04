using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace SunsetSystems.Formation.Data
{
    [System.Serializable]
    public class FormationData
    {
        [SerializeField]
        public readonly ReadOnlyCollection<Vector3> positions;

        public FormationData(List<Vector3> positions)
        {
            this.positions = new ReadOnlyCollection<Vector3>(positions);
        }

        public FormationData(Vector3[] positions)
        {
            this.positions = new ReadOnlyCollection<Vector3>(positions);
        }
    }
}
