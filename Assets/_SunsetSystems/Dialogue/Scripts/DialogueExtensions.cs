using System.Threading.Tasks;
using SunsetSystems.Core.AddressableManagement;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Characters;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class DialogueExtensions
    {
        private static string lastID;
        private static Sprite lastSprite;

        public static Sprite GetSpeakerPortrait(this DialogueViewBase dialogueViewBase, string speakerID)
        {
            Debug.Log($"Fetching config of {speakerID} from the database!");
            if (lastID == speakerID && lastSprite != null)
                return lastSprite;
            if (CreatureDatabase.Instance.TryGetConfig(speakerID, out CreatureConfig config))
            {
                lastID = speakerID;
                lastSprite = config.Portrait;
            }
            return lastSprite;
        }
    }
}
