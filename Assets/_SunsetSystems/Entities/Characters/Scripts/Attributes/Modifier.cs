public struct Modifier
{
    public ModifierType Type { get; private set; }
    public int Value { get; private set; }
    public string Name { get; private set; }

    public Modifier(int value, ModifierType type, string name)
    {
        this.Type = type;
        this.Value = value;
        this.Name = name;
    }
}
