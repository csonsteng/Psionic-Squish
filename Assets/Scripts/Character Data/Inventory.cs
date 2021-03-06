using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
	//private List<ItemInstance> items = new List<ItemInstance>();
	private List<CharacterAction> actions = new List<CharacterAction>();
	public int itemSlots = 3;
	private int pelletCount=0;

	public Inventory() { }
	public Inventory(SerializableInventory inventory) {
		pelletCount = inventory.pelletCount;
		foreach (var item in inventory.actions) {
			actions.Add(new CharacterAction(item));
		}
	}
	public Inventory(SerializableInventory inventory, AbstractCharacter owner) {
		pelletCount = inventory.pelletCount;
		itemSlots = inventory.itemSlots;
		foreach(var item in inventory.actions) {
			actions.Add(new CharacterAction(item, owner));
		}
	}

	public SerializableInventory Serialize() {
		return new SerializableInventory(this);
	}

	//public void GiveItem(ItemInstance item) {
	//	items.Add(item);
	//}

	//public IEnumerable<ItemInstance> GetItems() {
	//	foreach (var item in items) {
	//		yield return item;
	//	}
	//}

	public CharacterAction GetDefaultAction() {
		foreach(var action in actions) {
			if (action.IsDefault) {
				return action;
			}
		}
		return null;
	}
	public IEnumerable<CharacterAction> GetAllActions() {
		foreach (var action in actions) {
			yield return action;
		}
	}
	public List<CharacterAction> GetRemovableActions() {
		var removableActions = new List<CharacterAction>();
		foreach(var action in actions) {
			if (action.IsItem) {
				removableActions.Add(action);
			}
		}
		return removableActions;
	}
	public List<CharacterAction> GetFixedActions() {
		var fixedActions = new List<CharacterAction>();
		foreach (var action in actions) {
			if (!action.IsItem) {
				fixedActions.Add(action);
			}
		}
		return fixedActions;
	}

	public void ProcessCooldowns() {
		foreach(var action in actions) {
			action.Cooldown();
		}
	}

	public bool TryAddAction(CharacterAction action) {
		if(action.IsItem && GetRemovableActions().Count >= itemSlots) {
			return false;
		}
		actions.Add(action);
		return true;
	}

	public bool TryGetAction(CharacterActionData actionData, out CharacterAction action) {
		foreach(var a in actions) {
			if(a.Data == actionData) {
				action = a;
				return true;
			}
		}
		action = null;
		return false;
	}

	public bool RemoveAction(CharacterActionData actionData) {
		if(TryGetAction(actionData, out var action)) {
			actions.Remove(action);
			return true;
		}
		return false;
	}

	//public bool TakeItem(ItemInstance item) {
	//	if (!items.Contains(item)) {
	//		return false;
	//	}
	//	items.Remove(item);
	//	return true;
	//}

	//public bool HasItem(ItemData itemData) {
	//	foreach(var item in items) {
	//		if (item.Data == itemData) {
	//			return true;
	//		}
	//	}
	//	return false;
	//}

	public void AddPellets(int count) {
		if(count < 0) {
			throw new System.Exception("Cannot add negative pellets");
		}
		pelletCount += count;
	}

	public bool RemovePellets(int count) {
		if (count < 0) {
			throw new System.Exception("Cannot remove negative pellets");
		}
		if(pelletCount < count) {
			return false;
		}
		pelletCount -= count;
		return true;
	}

	public int GetPelletCount() {
		return pelletCount;
	}
}
