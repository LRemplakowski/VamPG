using SunsetSystems.Combat;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Inventory.Data;

namespace SunsetSystems.DynamicLog
{
    public static class LogUtility
    {
        public static string LogMessageFromAttackResult(ICombatant attacker, ITargetable target, AttackResult attack)
        {
            var attackerName = attacker.References.CreatureData.FullName;
            var targetName = target.GetLocalizedName();
            string result;
            if (attack.Successful)
            {
                if (attack.Critical)
                {
                    result = $"{attackerName.Trim()} critically hits {targetName.Trim()} and deals {attack.AdjustedDamage} damage!";
                }
                else
                {
                    result = $"{attackerName.Trim()} hits {targetName.Trim()} and deals {attack.AdjustedDamage} damage!";
                }
            }
            else
            {
                result = $"{attackerName.Trim()} misses {targetName.Trim()}!";
            }
            return result;
        }

        public static string LogMessageFromItemPickup(IBaseItem item)
        {
            return $"Item gained: {item.Name}";
        }

        public static string LogMessageFromItemLoss(IBaseItem item)
        {
            return $"Item lost: {item.Name}";
        }
    }
}
