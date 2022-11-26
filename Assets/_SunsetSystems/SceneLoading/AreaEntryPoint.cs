using SunsetSystems.Utils;
using UnityEngine;

namespace SunsetSystems.Loading
{
    [RequireComponent(typeof(Tagger))]
    public class AreaEntryPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
            Vector3 xOffset = new(1f, 0f, 0f);
            Gizmos.DrawLine(transform.position - xOffset, transform.position + xOffset);
            Vector3 zOffset = new(0f, 0f, 1f);
            Gizmos.DrawLine(transform.position - zOffset, transform.position + zOffset);
        }
    }
}
