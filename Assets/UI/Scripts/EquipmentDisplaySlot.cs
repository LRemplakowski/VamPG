using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDisplaySlot : ExposableMonobehaviour
{
    public Image equippedIcon;
    public CustomContextMenu contextMenu;
    public CharacterSheetUI characterSheetWindow;
    public EquipmentSlot equipmentSlot;

    [ReadOnly]
    [SerializeField]
    private EquipmentPiece item;

    private void Awake()
    {
        if (equippedIcon == null)
        {
            List<Image> images = new List<Image>(GetComponentsInChildren<Image>(true));
            foreach (Image image in images)
            {
                if (image.CompareTag("Equipment icon"))
                {
                    equippedIcon = image;
                    break;
                }
            }
        }
        if (characterSheetWindow == null)
        {
            characterSheetWindow = GetComponentInParent<CharacterSheetUI>();
        }
    }

    public void OnLeftClick()
    {
        UnequipItem();
    }    

    public void UnequipItem()
    {
        if(item != null)
            EquipmentManager.instance.Unequip((int)item.slot);
    }

    public void DisplayItem(EquipmentPiece item)
    {
        if(item != null)
        {
            Debug.Log(item + " assigned to slot " + equipmentSlot);
            this.item = item;
            equippedIcon.sprite = this.item.icon;
            equippedIcon.enabled = true;
        }
        else
        {
            Debug.Log(equipmentSlot + " set to null!");
            this.item = null;
            equippedIcon.sprite = null;
            equippedIcon.enabled = false;
        }
    }
}
