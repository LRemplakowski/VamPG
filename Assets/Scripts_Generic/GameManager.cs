using Entities.Characters;
using SunsetSystems.GameData;
using SunsetSystems.Management;
using UnityEngine;

public class GameManager : Manager
{
    private static Creature player;
    private static GridController gridController;

    // Start is called before the first frame update
    void Start()
    {
        gridController = FindObjectOfType<GridController>();
    }

    public static Creature GetMainCharacter()
    {
        if (player == null)
            player = FindObjectOfType<GameData>().MainCharacterData.CreatureComponent;
        return player;
    }

    public static GridController GetGridController()
    {
        return gridController;
    }

    public static string GetLanguage()
    {
        return "PL";
    }
}
