using Entities.Characters;
using SunsetSystems.Management;

public class GameManager : Manager
{
    private static Player player;
    private static GridController gridController;

    // Start is called before the first frame update
    void Start()
    {
        gridController = FindObjectOfType<GridController>();
    }

    public void Initialize()
    {
        gridController = FindObjectOfType<GridController>();
    }

    public static Player GetPlayer()
    {
        if (player == null)
            player = FindObjectOfType<Player>(true);
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
