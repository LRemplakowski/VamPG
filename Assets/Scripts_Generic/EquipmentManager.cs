using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Instance
    public static EquipmentManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public SkinnedMeshRenderer targetMesh;

    private EquipmentPiece[] currentEquipment;
    private SkinnedMeshRenderer[] currentMeshes;

    public delegate void OnEquipmentChanged(EquipmentPiece newItem, EquipmentPiece oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    private void OnEnable()
    {
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new EquipmentPiece[numSlots];
        currentMeshes = new SkinnedMeshRenderer[numSlots];
    }

    public EquipmentPiece GetItemInSlot(EquipmentSlot slot)
    {
        return currentEquipment[(int)slot];
    }

    public void Equip(EquipmentPiece newItem)
    {
        int slotIndex = (int)newItem.slot;

        EquipmentPiece oldItem = null;

        if(currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            GameManager.player.inventory.Add(currentEquipment[slotIndex]);
        }

        currentEquipment[slotIndex] = newItem;

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        if(targetMesh != null)
        {
            SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.meshRenderer, targetMesh.transform);

            newMesh.bones = targetMesh.bones;
            newMesh.rootBone = targetMesh.rootBone;
            currentMeshes[slotIndex] = newMesh;
        }
    }

    public void Unequip(int slotIndex)
    {
        if(currentEquipment[slotIndex] != null)
        {
            if (currentMeshes[slotIndex] != null)
                Destroy(currentMeshes[slotIndex].gameObject);

            EquipmentPiece oldItem = currentEquipment[slotIndex];
            GameManager.player.inventory.Add(oldItem);

            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
        }
    }

    public void UnequipAll()
    {
        for(int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }
}
