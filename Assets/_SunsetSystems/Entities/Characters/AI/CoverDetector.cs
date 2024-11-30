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

    public static bool FiringLineObstructedByCover(ICombatant attacker, ITargetable target, out ICover coverSource)
    {
        if (target.CombatContext.IsInCover)
        {
            Vector3 attackerRaycast = attacker.AimingOrigin;
            Vector3 targetRaycast = target.CombatContext.AimingOrigin;
            float distance = Vector3.Distance(attackerRaycast, targetRaycast);
            Vector3 direction = (attackerRaycast - targetRaycast).normalized;
            RaycastHit[] hits = Physics.RaycastAll(new Ray(attackerRaycast, direction), distance, _instance.coverLayerMask);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    coverSource = hit.collider.GetComponent<ICover>();
                    if (target.CombatContext.CurrentCoverSources.Contains(coverSource))
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
