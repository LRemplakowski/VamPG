using System;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public interface IAbilityButton
    {
        void Initialize(IAbility ability, Action<IAbility> selectionDelegate);
    }

    public interface IAbilityButtonFactory
    {
        IAbilityButton Create(Transform parent, IAbility data, Action<IAbility> selectionDelegate);
    }

    public abstract class AbstractAbilityButtonFactory : SerializedScriptableObject, IAbilityButtonFactory
    {
        public abstract IAbilityButton Create(Transform parent, IAbility data, Action<IAbility> selectionDelegate);
    }

    [CreateAssetMenu(fileName = "New Ability Button Facotyr", menuName = "Factories/UI/Ability Button Factory")]
    public class AbilityButtonFactory : AbstractAbilityButtonFactory
    {
        [SerializeField, AssetsOnly]
        private IAbilityButton _buttonPrefab;

        public override IAbilityButton Create(Transform parent, IAbility data, Action<IAbility> selectionDelegate)
        {
            var buttonObject = Instantiate(_buttonPrefab as UnityEngine.Object, parent);
            var buttonBehaviour = buttonObject as IAbilityButton;
            buttonBehaviour.Initialize(data, selectionDelegate);
            return buttonBehaviour;
        }
    }
}