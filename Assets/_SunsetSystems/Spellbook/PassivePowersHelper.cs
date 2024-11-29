using SunsetSystems.Abilities;
using SunsetSystems.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems
{
    [CreateAssetMenu(fileName = "Passive Powers Helper", menuName = "Character/Passive Powers Helper")]
    public class PassivePowersHelper : ScriptableObjectSingleton<PassivePowersHelper>
    {
        [field: SerializeField]
        public DisciplinePower HeightAttackAndDamageBonus { get; private set; }
    }
}
