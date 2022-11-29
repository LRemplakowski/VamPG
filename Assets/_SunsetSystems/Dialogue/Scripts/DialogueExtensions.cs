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
            Debug.Log($"Fetching config of {speakerID} from the database!");
            if (CreatureDatabase.Instance.TryGetConfig(speakerID, out CreatureConfig config))
            {
                result = config.Portrait;
            }
            else
            {
                Debug.LogWarning($"Config database fetch failed for ID {speakerID}");
            }
            return result;
        }
    }
}
