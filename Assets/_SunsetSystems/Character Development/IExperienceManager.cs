namespace SunsetSystems.Experience
{
    public interface IExperienceManager
    {
        void AddCreatureToExperienceManager(string creatureID);

        bool TryAwardExperience(string creatureID, int amount, ExperienceType experienceType);

        bool TryRemoveExperience(string creatureID, int amount, ExperienceType experienceType);
    }
}
