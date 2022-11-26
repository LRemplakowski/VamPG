using SunsetSystems.Entities;
using SunsetSystems.Entities.Characters;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class DialogueExtensions
    {
        public static Sprite GetSpeakerPortrait(this DialogueViewBase dialogueViewBase, string speakerID)
        {
            Sprite result = null;
            if (CreatureDatabase.Instance.TryGetConfig(speakerID, out CreatureConfig config))
            {
                result = config.Portrait;
            }
            return result;
        }
    }
}
