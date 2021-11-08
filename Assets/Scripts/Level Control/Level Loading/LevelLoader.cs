using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Structures;

public class LevelLoader : MonoBehaviour
{
	private SerializableLevel level;
	private SerializableLevel loadingLevel;
	private LevelMap map;
	private Party party;



	private LevelController Controller => LevelController.Get();
	private LevelObjectSpawner Spawner => LevelObjectSpawner.Get();

	public static LevelLoader Get() {
		LevelLoader loader = (LevelLoader)FindObjectOfType(typeof(LevelLoader));
		if (loader == null) {
			throw new System.Exception("Level Loader does not exist");
		}
		return loader;
	}

	public async UniTask LoadFromSave(SaveData saveData) {
		Spawner.UnloadLevel();
		Controller.SetState(LevelController.LevelState.Loading);

		loadingLevel = saveData.level;
		map = loadingLevel.GetMap();
		party = new Party(saveData.party, loadingLevel);
		var configuration = loadingLevel.GetConfiguration();

		foreach(var space in loadingLevel.GetStartingSpaces()) {
			Controller.startingSpaces.Add(space);
		}

		level = new SerializableLevel();
		level.SetConfiguration(configuration);
		level.SetStartingSpaces(Controller.startingSpaces);

		Controller.SetLevel(level);
		Controller.map = map;
		Controller.SetParty(party);
		Controller.turnNumber = loadingLevel.GetTurnNumber();
		Controller.rewindCount = loadingLevel.rewindCount;
		Spawner.Initialize(level, configuration, party);

		List<UniTask> tasks = new List<UniTask> {
			LoadTiles(),
			LoadObstacles(),
			LoadStructures(),
			LoadContainers(),
			LoadEnemies(),
			LoadPlayers()
		};

		await UniTask.WhenAll(tasks);

		Pathfinding.GenerateNodeMap(map);
		Spawner.StartGame();
	}

	private async UniTask LoadTiles() {
		List<UniTask> tasks = new List<UniTask>();
		foreach (var space in map.spaces) {
			var assetReference = new AssetReferenceGameObject(space.tileObjectGUID);
			tasks.Add(Spawner.GenerateTileForSpace(assetReference, space));
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadObstacles() {
		List<UniTask> tasks = new List<UniTask>();
		foreach (var obstacle in loadingLevel.GetObstacles()) {
			var assetReference = new AssetReferenceGameObject(obstacle.obstacleGUID);
			tasks.Add(Spawner.SpawnObstacleInSpace(assetReference, loadingLevel.GetSpace(obstacle.position), obstacle.rotation));
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadStructures() {
		List<UniTask> tasks = new List<UniTask>();
		foreach (var structure in loadingLevel.GetStructures()) {
			tasks.Add(LoadStructure(structure, loadingLevel.GetSpace(structure.position)));
			level.AddStructure(structure);
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadStructure(SerializableStructure runTimeStructure, MapSpace space) {
		var structure = new LevelStructure(ResourceLoader.GetStructure(runTimeStructure.structure));
		var gameObject = await Spawner.SpawnStructureObject(structure.Prefab, space, structure.PrefabOffset, runTimeStructure.rotation);

		var spaces = structure.GetRotatedSpaces(runTimeStructure.rotation);
		foreach (var structureSpace in spaces) {
			var levelSpace = map.GetSpaceFromCoordinates(space.row + structureSpace.row, space.column + structureSpace.column);
			if (levelSpace == null) {
				continue;
			}
			levelSpace.ClaimPositionPassable(gameObject, gameObject.layer);
			levelSpace.PlaceStructure(structureSpace, gameObject.GetComponent<StructureSceneObject>());
		}
	}

	private async UniTask LoadContainers() {
		List<UniTask> tasks = new List<UniTask>();
		foreach (var container in loadingLevel.GetContainers()) {
			var deserialized = new LevelInteractableContainer(container);
			deserialized.SetPosition(loadingLevel.GetSpace(container.position));
			var assetReference = new AssetReferenceGameObject(container.assetReference);
			tasks.Add(Spawner.SpawnContainer(deserialized, assetReference));
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadObjectives() {
		List<UniTask> tasks = new List<UniTask>();
		foreach (var objective in loadingLevel.GetObjectives()) {
			var assetReference = new AssetReferenceGameObject(objective.objective);
			//tasks.Add(Spawner.SpawnContainer(assetReference, loadingLevel.GetSpace(objective.position), objective.rotation));
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadEnemies() {
		List<UniTask> tasks = new List<UniTask>();
		foreach (var serializedEnemy in loadingLevel.GetEnemies()) {
			var enemy = new EnemyCharacter(serializedEnemy);
			enemy.SetPosition(loadingLevel.GetSpace(serializedEnemy.position));
			if (serializedEnemy.target != null) {
				enemy.target = loadingLevel.GetSpace(serializedEnemy.target);
			}
			foreach (var pathPoint in serializedEnemy.patrolPath) {
				enemy.patrolPath.Add(loadingLevel.GetSpace(pathPoint));
			}
			tasks.Add(Spawner.SpawnEnemy(enemy));
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadPlayers() {
		List<UniTask> tasks = new List<UniTask>();
		foreach (var player in party.members) {
			tasks.Add(Spawner.SpawnPlayer(player));
		}
		await UniTask.WhenAll(tasks);
	}
}
