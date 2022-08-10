using Entities.Interactable;
using SunsetSystems.Game;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public PickableItem placeablePrefab;

    public void DropOnGround(Vector3 position)
    {
        if (placeablePrefab != null)
        {
            PickableItem dropped = Instantiate(placeablePrefab, position, Quaternion.identity);
            dropped.item = this;
        }
        else
        {
            Debug.LogError("InventoryItem " + this + " has no placeable reference!");
        }
    }

    public virtual void Use()
    {
        //Handle item usage here

        Debug.Log("Player used item " + this);
    }

    public void RemoveFromInventory()
    {
        GameManager.Instance.GetMainCharacter().GetInventory().Remove(this);
    }
}
