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

	public static LevelLoader Get() 
	{
		LevelLoader loader = (LevelLoader)FindObjectOfType(typeof(LevelLoader));
		if (loader == null) 
			throw new System.Exception("Level Loader does not exist");

		return loader;
	}

	public async UniTask LoadFromSave(SaveData saveData) 
	{
		Spawner.UnloadLevel();
		Controller.SetState(LevelController.LevelState.Loading);

		loadingLevel = saveData.level;
		map = loadingLevel.GetMap();
		party = saveData.party;
		var configuration = loadingLevel.GetConfiguration();

		foreach(var space in loadingLevel.GetStartingSpaces())
			Controller.startingSpaces.Add(space);
		

		level = new SerializableLevel();
		level.SetConfiguration(configuration);
		level.SetStartingSpaces(Controller.startingSpaces);

		Controller.SetLevel(level);
		Controller.map = map;
		Controller.SetParty(party);
		Controller.turnNumber = loadingLevel.GetTurnNumber();
		Controller.rewindCount = loadingLevel.rewindCount;
		Spawner.Initialize(level, configuration, party);

		List<UniTask> tasks = new List<UniTask> 
		{
			LoadTiles(),
			LoadStructures(),
			LoadContainers(),
			LoadEnemies(),
			LoadPlayers()
		};

		await UniTask.WhenAll(tasks);

		Pathfinding.GenerateNodeMap(map);
		Spawner.StartGame();
	}

	private async UniTask LoadTiles() 
	{
		List<UniTask> tasks = new List<UniTask>();
		foreach (var space in map.spaces) {
			var assetReference = new AssetReferenceGameObject(space.tileObjectGUID);
			tasks.Add(Spawner.GenerateTileForSpace(assetReference, space));
			if (space.HasObstacle)
			{
				tasks.Add(Spawner.SpawnObstacleFromSpace(space));
			}
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadStructures() 
	{
		List<UniTask> tasks = new List<UniTask>();
		foreach (var structure in loadingLevel.GetStructures()) {
			tasks.Add(LoadStructure(structure));
			level.AddStructure(structure);
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadStructure(LevelStructure structure) 
	{
		var gameObject = await Spawner.SpawnStructureObject(structure);

		var space = structure.rootSpace;
		var spaces = structure.GetRotatedSpaces(structure.rotation);
		foreach (var structureSpace in spaces) 
		{
			var levelSpace = map.GetSpaceFromCoordinates(space.Row + structureSpace.row, space.Column + structureSpace.column);
			levelSpace.ClaimPositionPassable(gameObject, gameObject.layer);
			levelSpace.PlaceStructure(structureSpace, gameObject.GetComponent<StructureSceneObject>());
		}
	}

	private async UniTask LoadContainers() 
	{
		List<UniTask> tasks = new List<UniTask>();
		foreach (var container in loadingLevel.GetContainers()) 
		{
			var assetReference = new AssetReferenceGameObject(container.assetReference);
			tasks.Add(Spawner.SpawnContainer(container, assetReference));
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadEnemies() 
	{
		List<UniTask> tasks = new List<UniTask>();
		foreach (var enemy in loadingLevel.GetEnemies()) 
			tasks.Add(Spawner.SpawnEnemy(enemy));
		await UniTask.WhenAll(tasks);
	}

	private async UniTask LoadPlayers() 
	{
		List<UniTask> tasks = new List<UniTask>();
		foreach (var player in party.members) 
			tasks.Add(Spawner.SpawnPlayer(player));		
		await UniTask.WhenAll(tasks);
	}
}
