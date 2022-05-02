using UnityEngine;
using SunsetSystems.Inventory;
using System.Collections.Generic;

namespace SunsetSystems.Inventory.Data
{
	[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Items/Consumable")]
	public class Consumable : BaseItem
	{
		[SerializeField]
		private string itemName;
		[SerializeField, TextArea]
		private string itemDescription;
		[SerializeField]
		private float weight;
		[SerializeField, RequireInterface(typeof(IScriptableItemAttribute))]
		private Object usable;
	}
}
