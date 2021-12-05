using Entities.Characters.Actions;
using InsaneSystems.RTSSelection;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Characters
{
    public class MainCharacter : PlayerControlledCharacter
    {
        public delegate void OnMainCharacterInitialized();
        public static event OnMainCharacterInitialized onMainCharacterInitialized;

        protected override void Start()
        {
            base.Start();
            onMainCharacterInitialized?.Invoke();
        }
    } 
}
