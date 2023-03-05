namespace SunsetSystems.UI
{
    public interface ICharacterSelector
    {
        string SelectedCharacterKey { get; }

        void NextCharacter();
        void PreviousCharacter();
    }
}
