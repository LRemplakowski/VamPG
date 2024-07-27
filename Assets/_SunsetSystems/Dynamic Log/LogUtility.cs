using SunsetSystems.Combat;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Inventory.Data;

namespace SunsetSystems.DynamicLog
{
    public static class LogUtility
    {
        public static string LogMessageFromAttackResult(ICombatant attacker, ICombatant target, AttackResult attack)
        {
            var attackerName = attacker.References.CreatureData.FullName;
            var targetName = target.References.CreatureData.FullName;
            string result;
            if (attack.Successful)
            {
                if (attack.Critical)
                {
                    result = $"{attackerName} critically hits {targetName} and deals {attack.AdjustedDamage} damage!";
                }
                else
                {
                    result = $"{attackerName} hits {targetName} and deals {attack.AdjustedDamage} damage!";
                }
            }
            else
            {
                result = $"{attackerName} misses {targetName}!";
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
