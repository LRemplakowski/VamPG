using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public PickableItem pickablePrefab;

    public void SpawnPlaceable(Vector3 position)
    {
        if(pickablePrefab != null)
        {
            PickableItem dropped = Instantiate(pickablePrefab, position, Quaternion.identity);
            dropped.item = this;
        }
        else
        {
            Debug.LogError("InventoryItem " + this + " has no placeable reference!");
        }
    }
}
