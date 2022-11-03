using SunsetSystems.Journal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems
{
    public class QuestView : MonoBehaviour
    {
        
        public void DisplayQuest(Quest quest)
        {
            Debug.Log("Displaying quest " + quest.Data.Name);
        }    
    }
}
