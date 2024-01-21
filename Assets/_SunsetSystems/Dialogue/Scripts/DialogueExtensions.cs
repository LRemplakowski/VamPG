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
        private static Sprite lastLoadedSprite;

        public async static Task<Sprite> GetSpeakerPortrait(this DialogueViewBase dialogueViewBase, string speakerID)
        {
            Debug.Log($"Fetching config of {speakerID} from the database!");
            if (lastID == speakerID && lastLoadedSprite != null)
                return lastLoadedSprite;
            if (CreatureDatabase.Instance.TryGetConfig(speakerID, out CreatureConfig config))
            {
                if (CreatureDatabase.Instance.TryGetConfig(lastID, out CreatureConfig previousConfig))
                {
                    if (previousConfig.PortraitAssetRef == config.PortraitAssetRef && lastLoadedSprite != null)
                        return lastLoadedSprite;
                    lastLoadedSprite = null;
                    //AddressableManager.Instance.ReleaseAsset(previousConfig.PortraitAssetRef);
                }
                lastID = speakerID;
                lastLoadedSprite = await AddressableManager.Instance.LoadAssetAsync(config.PortraitAssetRef);
                return lastLoadedSprite;
            }
            else
            {
                Debug.LogWarning($"Config database fetch failed for ID {speakerID}");
            }
            return null;
        }
    }
}
