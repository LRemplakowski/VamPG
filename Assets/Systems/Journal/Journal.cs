using Entities.Characters;
using System.Collections;
using System.Collections.Generic;
using Systems.Management;
using UnityEngine;

namespace Systems.Journal
{
    public class Journal : Manager
    {
        [SerializeField]
        private CreatureData[] recruitedCompanions;
        [SerializeField]
        private PlayerCharacterBackground playerBackground;

        
    }
}
