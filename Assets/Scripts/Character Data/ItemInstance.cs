using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemInstance : ReferenceInstance<ItemData>
{
	public bool Consumable => data.consumable;
	public int BaseCost => data.baseCost;

	public ItemInstance(ItemData data) : base(data)
	{

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

	protected override ItemData LoadReference() => ResourceLoader.GetItem(referenceID);
}
