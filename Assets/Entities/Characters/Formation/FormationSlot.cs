using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Characters.Formation
{
    public class FormationSlot : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one / 10);
        }
    }
}