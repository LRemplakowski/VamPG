namespace SunsetSystems.Animation
{
    public interface IAnimationManager
    {
        void PlayFireWeaponAnimation();
        void PlayTakeHitAnimation();
        void SetCoverAnimationsEnabled(bool enabled);
    }
}