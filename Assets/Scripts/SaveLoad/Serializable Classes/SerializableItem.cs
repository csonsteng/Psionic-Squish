using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableItem 
{
	public string itemData;

	public SerializableItem(ItemInstance item) {
		itemData = item.Data.GetUniqueID();
	}
}
