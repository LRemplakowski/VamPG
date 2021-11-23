using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMA.CharacterSystem;
using UnityEngine;

namespace Entities.Characters
{
    public class CreatureData : ExposableMonobehaviour
    {
        [SerializeField]
        private CreatureAsset data;

        private void Start()
        {
            DynamicCharacterAvatar dca = GetComponent<DynamicCharacterAvatar>();
        }
    }
}
