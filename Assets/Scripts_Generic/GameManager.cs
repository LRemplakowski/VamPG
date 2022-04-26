using Entities.Characters;
using SunsetSystems.Data;
using SunsetSystems.Management;
using UnityEngine;

public class GameManager : Manager
{
    private static Creature _player;
    private static GridController _gridController;

    // Start is called before the first frame update
    void Start()
    {
        _gridController = FindObjectOfType<GridController>();
    }

    public static Creature GetMainCharacter()
    {
        if (_player == null)
            _player = GameRuntimeData.Instance.MainCharacterData.CreatureComponent;
        return _player;
    }

    public static GridController GetGridController()
    {
        return _gridController;
    }

    public static string GetLanguage()
    {
        return "PL";
    }
}
