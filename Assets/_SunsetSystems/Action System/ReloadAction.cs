using System.Collections;
using SunsetSystems.Equipment;
using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    public class ReloadAction : EntityAction
    {
        [SerializeField]
        private FlagWrapper _actionFinishedFlag;
        [SerializeField]
        private IWeaponManager _weaponManager;

        public ReloadAction(IWeaponManager weaponManager, IActionPerformer actionPerformer) : base(actionPerformer)
        {
            
            _actionFinishedFlag = new() { Value = false };
            _weaponManager = weaponManager;
        }

        public override void Begin()
        {
            conditions.Add(new WaitForFlag(_actionFinishedFlag));
            _weaponManager.ReloadSelectedWeapon();
            _actionFinishedFlag.Value = true;
        }
    }
}