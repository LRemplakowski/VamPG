﻿using System.Collections;
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

    public const float BLEND_SHAPES_MIN_WEIGHT = 0.0f;
    public const float BLEND_SHAPES_MAX_WEIGHT = 100.0f;

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

        EquipmentPiece oldItem = Unequip(slotIndex);

        currentEquipment[slotIndex] = newItem;

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        SetEquipmentBlendShapes(newItem, BLEND_SHAPES_MAX_WEIGHT);

        if(targetMesh != null)
        {
            SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.meshRenderer, targetMesh.transform);

            newMesh.bones = targetMesh.bones;
            newMesh.rootBone = targetMesh.rootBone;
            currentMeshes[slotIndex] = newMesh;
        }
    }

    public EquipmentPiece Unequip(int slotIndex)
    {
        if(currentEquipment[slotIndex] != null)
        {
            if (currentMeshes[slotIndex] != null)
                Destroy(currentMeshes[slotIndex].gameObject);

            EquipmentPiece oldItem = currentEquipment[slotIndex];
            GameManager.GetPlayer().inventory.Add(oldItem);

            SetEquipmentBlendShapes(oldItem, BLEND_SHAPES_MIN_WEIGHT);

            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke(null, oldItem);

            return oldItem;
        }

        return null;
    }

    public void UnequipAll()
    {
        for(int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }

    private void SetEquipmentBlendShapes(EquipmentPiece item, float weight)
    {
        Debug.LogWarning("SetEquipmentBlendShapes called!");
        foreach(EquipmentMeshRegion blendShape in item.coveredMeshRegions)
        {
            Debug.LogWarning("Initial blendShape = " + targetMesh.GetBlendShapeWeight((int) blendShape));
            Debug.LogWarning("Changing blend shape " + blendShape + " for item " + item + " to " + weight);
            targetMesh.SetBlendShapeWeight((int)blendShape, weight);
            Debug.LogWarning(targetMesh.GetBlendShapeWeight((int)blendShape));
        }
    }
}
