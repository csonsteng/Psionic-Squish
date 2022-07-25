using System;
using System.Collections.Generic;
using UnityEngine;
using Structures;

public static class ResourceLoader 
{
	private static MasterReference references;

	public static MasterReference References => GetReferences();

	private static MasterReference GetReferences() {
		if(references == null) {
			references = Resources.Load<MasterReference>("Master Reference");
		}
		return references;
	}
	
	public static LevelTypeData GetLevelType(string uniqueID) {
		return (LevelTypeData)References.levelTypes.Get(uniqueID);
	}
	public static LevelEnvironmentData GetEnvironment(string uniqueID) {
		return (LevelEnvironmentData)References.levelEnvironment.Get(uniqueID);
	}
	public static CharacterActionData GetAction(string uniqueID) {
		return (CharacterActionData)References.actions.Get(uniqueID);
	}
	public static PlayableCharacterData GetCharacter(string uniqueID) {
		return (PlayableCharacterData)References.characters.Get(uniqueID);
	}
	public static EnemyCharacterData GetEnemy(string uniqueID) {
		return (EnemyCharacterData)References.enemies.Get(uniqueID);
	}
	public static LevelStructureData GetStructure(string uniqueID) {
		return (LevelStructureData)References.structures.Get(uniqueID);
	}
	public static ItemData GetItem(string uniqueID) {
		return (ItemData)References.items.Get(uniqueID);
	}
}
