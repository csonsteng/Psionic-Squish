using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstance 
{
	private ItemData data;
	public ItemData Data => data;
	public bool Consumable => data.consumable;
	public int BaseCost => data.baseCost;
	public ItemInstance(ItemData itemData) {
		data = itemData;
	}
	public ItemInstance(SerializableItem item) {
		data = ResourceLoader.GetItem(item.itemData);
	}
	public SerializableItem Serialize() {
		return new SerializableItem(this);
	}
	public bool GrantsAction() {
		return data.action != null;
	}
	public CharacterActionData GetAction() {
		if (!GrantsAction()) {
			return null;
		}

		return data.action;
	}
}
