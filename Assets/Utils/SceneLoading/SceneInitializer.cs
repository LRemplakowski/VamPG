using SunsetSystems.GameData;
using System.Linq;
using UnityEngine;
using Utils.Singleton;

namespace Utils.Scenes
{
    internal static class SceneInitializer
    {
        private static GameData gameData;
        public static void InitializeScene(SceneInitializationData data)
        {
            gameData = Object.FindObjectOfType<GameData>();
            InitializeSingletons();
            gameData.InjectJournalData(data.journalData);
            InitializePlayableCharacters(data.areaEntryTag);
        }

        private static void InitializeSingletons()
        {
            Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IInitializable>().ToList().ForEach(o => o.Initialize());
        }

        private static void InitializePlayableCharacters(string tag)
        {
            AreaEntryPoint entryPoint = null;
            if (!tag.Equals(""))
            {
                GameObject obj = null;
                try
                {
                    obj = GameObject.FindGameObjectWithTag(tag);
                }
                catch (UnityException e)
                {
                    Debug.LogException(e);
                }
                if (obj)
                    entryPoint = obj.GetComponent<AreaEntryPoint>();
            }
            if (entryPoint == null)
            {
                entryPoint = Object.FindObjectOfType<AreaEntryPoint>();
            }

            if (gameData)
                gameData.InitializeParty(entryPoint != null ? entryPoint.transform.position : Vector3.zero);
        }
    }

    public class SceneInitializationData
    {
        public readonly string areaEntryTag;
        public readonly GameDataContainer journalData;

        private SceneInitializationData(string areaEntryTag, GameDataContainer journalData)
        {
            this.areaEntryTag = areaEntryTag;
            this.journalData = journalData;
        }

        public class SceneInitializationDataBuilder
        {
            private string areaEntryTag;
            private GameDataContainer journalData;

            public SceneInitializationDataBuilder SetAreaEntryTag(string areaEntryTag)
            {
                this.areaEntryTag = areaEntryTag;
                return this;
            }

            public SceneInitializationDataBuilder SetJournalData(GameDataContainer journalData)
            {
                this.journalData = journalData;
                return this;
            }

            public SceneInitializationData Build()
            {
                return new SceneInitializationData(areaEntryTag, journalData);
            }
        }
    }
}
