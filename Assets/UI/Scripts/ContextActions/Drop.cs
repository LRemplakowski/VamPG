using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : ContextAction
{
    public override void OnClick()
    {
        Debug.Log("Dropping item "+item.name);
        item.SpawnPlaceable(GameManager.player.transform.position);
        GameManager.player.GetComponent<PlayerInventory>().Remove(item);
    }
}
