using System.Linq;
using SunsetSystems.Combat;
using UnityEngine;

public class CoverDetector : MonoBehaviour
{
    [SerializeField]
    private LayerMask coverLayerMask;

    private static CoverDetector _instance;

    private void Start()
    {
        if (_instance == null)
            _instance = this;
    }

    public static bool FiringLineObstructedByCover(IAttackContext context, out ICover coverSource)
    {
        if (context.IsInCover(AttackParticipant.Target))
        {
            Vector3 attackerRaycast = context.GetAimingPosition(AttackParticipant.Attacker);
            Vector3 targetRaycast = context.GetAimingPosition(AttackParticipant.Target);
            float distance = Vector3.Distance(attackerRaycast, targetRaycast);
            Vector3 direction = (attackerRaycast - targetRaycast).normalized;
            RaycastHit[] hits = Physics.RaycastAll(new Ray(attackerRaycast, direction), distance, _instance.coverLayerMask);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    coverSource = hit.collider.GetComponent<ICover>();
                    if (context.GetCoverSources(AttackParticipant.Target).Contains(coverSource))
                    {
                        return true;
                    }
                }
            }
        }
        coverSource = null;
        return false;
    }
}
