using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableInventory 
{
    [SerializeField ]
    public List<SerializableAction> actions = new List<SerializableAction>();
    public int pelletCount;
    public int itemSlots;

    public SerializableInventory(Inventory inventory) {
        pelletCount = inventory.GetPelletCount();
        itemSlots = inventory.itemSlots;
        foreach(var item in inventory.GetAllActions()) {
            actions.Add(item.Serialize());
		}
	}
}
