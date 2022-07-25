using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemData : ReferenceData
{

	public string displayName;
	public bool consumable;
	public bool stackable;
	public int baseCost;

	[Tooltip("Lower rarity is more rare")]
	public int rarity;

	public CharacterActionData action;
	[Tooltip("Empty list has no restrictions")]
	public List<CharacterClass> usableBy = new List<CharacterClass>();

}
