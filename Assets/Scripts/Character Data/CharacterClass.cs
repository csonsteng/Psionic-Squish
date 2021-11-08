using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CharacterClass: ScriptableObject
{
	public string displayName;
	public int startingActionPoints;
	public List<CharacterActionData> startingActions = new List<CharacterActionData>();

}
