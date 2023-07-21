using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class CreatureAttribute : BaseStat
{
    [SerializeField, Range(1, 5)]
    private int _baseValue = 1;
    [SerializeField, ReadOnly]
    private AttributeType _attributeType;
    public int Value { get => GetValue(); }
    public AttributeType AttributeType { get => _attributeType; }

    public override string Name => AttributeType.ToString();

    public CreatureAttribute(CreatureAttribute existing) : base(existing)
    {
        this._baseValue = existing._baseValue;
        this._attributeType = existing._attributeType;
    }

    protected override void SetValueImpl(int value)
    {
        this._baseValue = value;
    }

    public CreatureAttribute(AttributeType attributeType)
    {
        this._attributeType = attributeType;
    }

    public CreatureAttribute() : this(AttributeType.Invalid)
    {

    }

    public override int GetValue(ModifierType modifierTypesFlag)
    {
        int finalValue = _baseValue;
        Modifiers?.ForEach(m => finalValue += (modifierTypesFlag & m.Type) > 0 ? m.Value : 0);
        return finalValue;
    }

    public AttributeType GetAttributeType()
    {
        return this._attributeType;
    }
}
