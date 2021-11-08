using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Structures;

[CreateAssetMenu(menuName = "Level Configuration/Environment")]
public class LevelEnvironment : ScriptableObject, IReferenced {
	public string GetUniqueID() {
		return uniqueID;
	}
	public string uniqueID;
	public int baseRowCount=15;
	public int baseColumnCount=15;

	public float obstacleDensity = 0.1f;
	public float minBaseDistanceFromStart = 7;
	public float maxObjectiveDistance = 4;
	public float minEnemyDistance = 5;
	public float enemyCount = 3;

	public float dummyObjectiveCount = 5;
	public float floatingContainerChance = 0.5f;

	public Material tileHoverMaterial;

	public List<AssetReferenceGameObject> obstacles = new List<AssetReferenceGameObject>();
	public List<AssetReferenceGameObject> groundTiles = new List<AssetReferenceGameObject>();
	public List<AssetReferenceGameObject> containers = new List<AssetReferenceGameObject>();

	public List<EnemyCharacterData> potentialEnemies = new List<EnemyCharacterData>();

	public List<LevelStructureData> structures = new List<LevelStructureData>();

	public List<CharacterActionData> requiredItems = new List<CharacterActionData>();

	public List<AbstractLevelObjective> objectives = new List<AbstractLevelObjective>();

	public EnemyCharacter GetEnemy() {
		if (potentialEnemies.Count > 0) {
			int randomEnemy = Random.Range(0, potentialEnemies.Count);
			return new EnemyCharacter(potentialEnemies[randomEnemy]);
		}
		return null;
	}

	public AssetReferenceGameObject GetObstacle() {
		return GetRandomObject(obstacles);
	}

	public AssetReferenceGameObject GetTile() {
		return GetRandomObject(groundTiles);
	}

	public AssetReferenceGameObject GetContainer() {
		return GetRandomObject(containers);
	}

	private AssetReferenceGameObject GetRandomObject(List<AssetReferenceGameObject> list) {
		if (list.Count > 0) {
			int randomObject = Random.Range(0, list.Count);
			return list[randomObject];
		}
		return null;
	}

	public DistanceCriteria[] GetBaseCriteria(MapSpace startSpace) {

		DistanceCriteria[] baseCriteria = { 
			new DistanceCriteria(startSpace, minBaseDistanceFromStart, DistanceCriteria.Comparison.GreaterThanOrEqual) };
		return baseCriteria;
	}
}
