using System.Collections;
using System.Collections.Generic;
using Entities.Characters;
using SunsetSystems.Management;
using UnityEngine;

public class AreaEntryPoint : MonoBehaviour
{
    private void OnEnable()
    {
        MainCharacter.onMainCharacterInitialized += OnMainInitialized;
    }

    private void OnDisable()
    {
        MainCharacter.onMainCharacterInitialized -= OnMainInitialized;
    }

    private void OnMainInitialized()
    {
        GameManager.GetMainCharacter().ForceCreatureToPosition(this.transform.position);
        FindObjectOfType<CameraControlScript>().Initialize();
    }
}
