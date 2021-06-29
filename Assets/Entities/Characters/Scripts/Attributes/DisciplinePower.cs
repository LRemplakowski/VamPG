using UnityEngine;

[System.Serializable]
public abstract class DisciplinePower : ScriptableObject
{
    [SerializeField, Tooltip("Drzewo dyscyplin do którego należy dyscyplina.")]
    protected DisciplineType type = DisciplineType.Invalid;
    [SerializeField, Range(1, 5), Tooltip("Minimalny poziom potrzebny do wykupienia dyscypliny.")]
    protected int level;
    [SerializeField, Tooltip("Wymagany przy amalgamatach.")]
    protected DisciplineType secondaryType = DisciplineType.Invalid;
    [SerializeField, Range(0, 5), Tooltip("Wymagany przy amalgamatach.")]
    protected int secondaryLevel;

    public abstract void Activate(Creature target);
}