using SunsetSystems.SaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSaveLoadButtonScript : MonoBehaviour
{
    public void DoSave()
    {
        SaveLoadManager.Save();
    }

    public void DoLoad()
    {
        SaveLoadManager.Load();
    }
}
