using SunsetSystems.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : ContextAction
{
    public override void OnClick()
    {
        Debug.Log("Dropping item " + item.name);
        item.DropOnGround(GameManager.Instance.GetMainCharacter().transform.position);
        GameManager.Instance.GetMainCharacter().GetInventory().Remove(item);
    }
}
