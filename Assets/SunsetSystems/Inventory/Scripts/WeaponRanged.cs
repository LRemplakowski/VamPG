using UnityEngine;
using SunsetSystems.Inventory;
using System.Collections.Generic;

namespace SunsetSystems.Inventory.Data
{
	[CreateAssetMenu(fileName = "New Weapon Ranged", menuName = "Inventory/Items/Weapon Ranged")]
	public class WeaponRanged : BaseItem
	{
		[SerializeField]
		private string itemName;
		[SerializeField, TextArea]
		private string itemDescription;
		[SerializeField]
		private float weight;
		[SerializeField, Range(0f, 10f)]
		private float damage;
	}
}
