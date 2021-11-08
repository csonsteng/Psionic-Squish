using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Structures;

[System.Serializable]
public class SerializableLevel 
{
	[SerializeField]
	private SerializableConfiguration configuration;
	[SerializeField]
	private readonly List<SerializableObstacle> obstacles = new List<SerializableObstacle>();
	[SerializeField]
	private SerializableMap map;
	[SerializeField]
	private readonly List<SerializableEnemy> enemies = new List<SerializableEnemy>();
	[SerializeField]
	private readonly List<SerializableStructure> structures = new List<SerializableStructure>();
	[SerializeField]
	private readonly List<SerializableObjective> objectives = new List<SerializableObjective>();
	[SerializeField]
	private readonly List<SerializableContainer> containers = new List<SerializableContainer>();
	[SerializeField]
	private List<SerializeMapReference> startingSpaces = new List<SerializeMapReference>();

	private LevelMap deserializedMap;

	[SerializeField]
	private int turnNumber;
	[SerializeField]
	public int rewindCount;

	public void SetConfiguration(LevelConfiguration configuration) {
		this.configuration = configuration.Serialize(); 
	}

	public void SetTurnNumber(int turnNumber) {
		this.turnNumber = turnNumber;
	}

	public int GetTurnNumber() {
		return turnNumber;
	}

	public LevelConfiguration GetConfiguration() {
		return new LevelConfiguration(configuration);
	}

	public void AddObstacle(MapSpace space, AssetReferenceGameObject gameObject, float rotation) {
		obstacles.Add(new SerializableObstacle(space, gameObject, rotation));
	}

	public void AddEnemy(EnemyCharacter enemy) {
		enemies.Add(enemy.Serialize());
	}

	public void AddStructure(LevelStructure structure, MapSpace space, float rotation) {
		structures.Add(new SerializableStructure(structure, space, rotation));
	}

	public void AddStructure(SerializableStructure structure) {
		structures.Add(structure);
	}

	public void AddObjective(AssetReferenceGameObject objective, MapSpace position, float rotation, bool isObjective = false) {
		objectives.Add(new SerializableObjective(objective, position, rotation, isObjective));
	}

	public void AddContainer(LevelInteractableContainer container) {
		containers.Add(new SerializableContainer(container));
	}

	public void RemoveEnemy(EnemyCharacter enemy) {
		//if (!enemies.Contains(enemy)) {
		//	return;
		//}
		//enemies.Remove(enemy);
	}

	public void SetMap(LevelMap map) {
		this.map = map.Serialize();
	}

	public LevelMap GetMap() {
		deserializedMap = new LevelMap(map);
		return deserializedMap;
	}

	public void SetStartingSpaces(List<MapSpace> spaces) {
		startingSpaces.Clear();
		foreach (var space in spaces) {
			startingSpaces.Add(space.Reference());
		}
	}

	public IEnumerable<MapSpace> GetStartingSpaces() {
		foreach (var space in startingSpaces) {
			yield return GetSpace(space);
		}
	}

	public IEnumerable<SerializableObstacle> GetObstacles() {
		foreach(var obstacle in obstacles) {
			yield return obstacle;
		}
	}

	public IEnumerable<SerializableEnemy> GetEnemies() {
		foreach (var enemy in enemies) {
			yield return enemy;
		}
	}
	public void ClearEnemies() {
		enemies.Clear();
	}

	public IEnumerable<SerializableStructure> GetStructures() {
		foreach (var structure in structures) {
			yield return structure;
		}
	}
	public IEnumerable<SerializableObjective> GetObjectives() {
		foreach (var objective in objectives) {
			yield return objective;
		}
	}

	public IEnumerable<SerializableContainer> GetContainers() {
		foreach(var container in containers) {
			yield return container;
		}
	}

	public void ClearContainers() {
		containers.Clear();
	}

	public int ContainerCount() {
		return containers.Count;
	}

	public MapSpace GetSpace(SerializeMapReference reference) {
		return deserializedMap.GetSpaceFromCoordinates(reference.row, reference.column);
	}
}
