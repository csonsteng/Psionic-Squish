using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Structures;
public class LevelGenerator : MonoBehaviour
{
	private SerializableLevel level;
	private LevelMap map;
	private Party party;
	private LevelConfiguration configuration;
	//private List<PlayableCharacterData> players;
	public List<MapSpace> potentialContainerLocation = new List<MapSpace>();
	MapSpace basePoint;
	private System.DateTime time;
	private LevelController Controller => LevelController.Get();
	private LevelObjectSpawner Spawner => LevelObjectSpawner.Get();

	public static LevelGenerator Get() {
		LevelGenerator loader = (LevelGenerator)FindObjectOfType(typeof(LevelGenerator));
		if (loader == null) {
			throw new System.Exception("Level Generator does not exist");
		}
		return loader;
	}

	public async UniTask Generate(LevelConfiguration configuration) {
		time = System.DateTime.Now;
		//this.players = players;
		this.configuration = configuration;
		Spawner.UnloadLevel();
		//if (party == null) {
		//	party = new Party();
		//}
		//party.Clear();
		//party.AddPlayers(players);
		//Controller.SetParty(party);
		party = Controller.GetParty();

		await GenerateLevel();
	}

	public async UniTask Regenerate() {
		await Generate(configuration);
	}

	public async UniTask GenerateLevel() {

		Controller.SetState(LevelController.LevelState.Loading);
		if (configuration == null) {
			throw new System.Exception("level configuration cannot be null");
		}

		level = new SerializableLevel();
		Controller.SetLevel(level);
		Controller.startingSpaces.Clear();
		level.SetConfiguration(configuration);


		Spawner.Initialize(level, configuration, party);



		int baseRowCount = configuration.RowCount;
		int baseColumnCount = configuration.ColumnCount;

		//do other stuff like randomize final values based on type and difficulty


		List<Vector2Int> startingSpots = DetermineRandomStartingLocation(baseRowCount, baseColumnCount);

		Controller.map = new LevelMap(baseRowCount, baseColumnCount);
		map = Controller.GetMap();
		List<UniTask> tasks = new List<UniTask>();
		foreach (var space in map.spaces) {
			tasks.Add(Spawner.GenerateTileForSpace(configuration.GetTile(), space));

			if (map.SpaceOnEdge(space)) {
				Vector2Int positionVector = new Vector2Int(space.row, space.column);
				if (!startingSpots.Contains(positionVector)) {
					tasks.Add(SpawnObstacleInSpace(space));
				}
			}
		}
		await tasks;
		var startSpace = map.GetSpaceFromVector2Int(startingSpots[0]);
		var basePointCriteria = configuration.GetBaseCriteria(startSpace);
		basePoint = map.GetRandomOpenSpaceWithDistanceCriteria(basePointCriteria);
		Debug.Log("Base Point:" + basePoint.GetName());
		Controller.basePoint = basePoint;
		await SpawnStructures();

		await SpawnContainers();

		await SpawnPlayers(startingSpots);

		var playerPath = GetPlayerPath();

		await FillMapWithObstacles(playerPath);

		level.SetStartingSpaces(Controller.startingSpaces);
		Pathfinding.GenerateNodeMap(map);
		InitializeEnemyStatuses();
		Spawner.StartGame();
		

		var deltaTime = System.DateTime.Now - time;
		Debug.Log("took " + deltaTime.TotalSeconds + " seconds to load");
		Controller.GameStarted();
	}

	private async UniTask SpawnContainers() {
		int floatingContainerCount = Mathf.RoundToInt(potentialContainerLocation.Count / configuration.environment.floatingContainerChance);
		for (var i = 0; i < floatingContainerCount; i++) {
			DistanceCriteria[] containerCriteria = { new DistanceCriteria(basePoint, configuration.ObjectiveDistance, DistanceCriteria.Comparison.LessThanOrEqual) , new DistanceCriteria(potentialContainerLocation)};
			var floatSpace = map.GetRandomOpenSpaceWithDistanceCriteria(containerCriteria);
			if (floatSpace == null) {
				continue;
			}
			potentialContainerLocation.Add(floatSpace);
		}
		var containerLocation = ChooseRandomPotentialContainerLocation();
		List<UniTask> tasks = new List<UniTask> {
			SpawnContainerInSpace(containerLocation)
		};

		for (var i = 0; i < configuration.environment.dummyObjectiveCount; i++) {
			if (potentialContainerLocation.Count == 0) {
				break;
			}
			containerLocation = ChooseRandomPotentialContainerLocation();
			tasks.Add(SpawnContainerInSpace(containerLocation));
		}

		await UniTask.WhenAll(tasks);
		var containers = Controller.GetContainers();
		foreach (var item in configuration.environment.requiredItems) {
			int randomContainerIndex = Random.Range(0, containers.Count);
			var randomContainer = containers[randomContainerIndex];
			randomContainer.contents.Add(item);
			Debug.Log("Placed " + item.displayName);
		}
	}

	private async UniTask SpawnContainerInSpace(MapSpace space) {
		var gameObject = configuration.environment.GetContainer();
		var rotation = GetRandomRotatation();
		var container = new LevelInteractableContainer(gameObject.AssetGUID, space, rotation);
		await Spawner.SpawnContainer(container, gameObject);

	}

	private MapSpace ChooseRandomPotentialContainerLocation() {
		int randomIndex = Random.Range(0, potentialContainerLocation.Count);
		var randomSpace = potentialContainerLocation[randomIndex];
		potentialContainerLocation.RemoveAt(randomIndex);
		return randomSpace;
	}

	private async UniTask SpawnStructures() {
		foreach (var structure in configuration.environment.structures) {
			await SpawnStructure(structure);
		}
	}

	public float GetRandomRotatation() {
		float[] rotations = { 0f, 90f, 180f, 270f };
		int index = Random.Range(0, 4);
		return rotations[index];
	}

	private async UniTask SpawnStructure(LevelStructureData structureData) {
		var structure = new LevelStructure(structureData);
		DistanceCriteria[] criteria = structure.GetCriteria(basePoint);
		var structSpace = map.GetRandomOpenSpaceWithDistanceCriteria(criteria);
		if (structSpace == null) {
			return;
		}

		float rotation = GetRandomRotatation();


		var gameObject = await Spawner.SpawnStructureObject(structure.Prefab, structSpace, structure.PrefabOffset, rotation);

		level.AddStructure(structure, structSpace, rotation);

		var spaces = structure.GetRotatedSpaces(rotation);
		foreach (var space in spaces) {
			var levelSpace = map.GetSpaceFromCoordinates(structSpace.row + space.row, structSpace.column + space.column);
			if (levelSpace == null) {
				continue;
			}
			levelSpace.ClaimPositionPassable(gameObject, gameObject.layer);
			levelSpace.PlaceStructure(space, gameObject.GetComponent<StructureSceneObject>());

			if (space.objectiveLocation) {
				if (!map.SpaceOnEdge(levelSpace)) {
					potentialContainerLocation.Add(levelSpace);
				}
			}
		}
		foreach (var enemy in structure.enemyConfiguration.GetEnemies(map, structSpace)) {
			if (enemy == null) {
				continue;
			}
			await Spawner.SpawnEnemy(enemy);
		}
	}

	private void InitializeEnemyStatuses() {
		foreach (var enemy in Controller.GetEnemies()) {
			switch (enemy.Status) {
				case EnemyStatus.Patrol:
					SetEnemyPatrol(enemy);
					break;
				case EnemyStatus.Sleeping:
					enemy.Sleep();
					enemy.SetFacing(new Direction().Random());
					break;
				default:
					enemy.SetFacing(new Direction().Random());
					break;
			}
			enemy.GetPosition().Passable = false;
		}
	}

	private static void SetEnemyLookDirection(EnemyCharacter enemy) {
		var patrolPath = enemy.GetPath();
		if (patrolPath.Count > 0) {
			Vector2 direction = patrolPath[0].SubtractFrom(enemy.GetPosition());
			enemy.SetFacing(new Direction(direction));
		}
		else {
			enemy.SetFacing(new Direction().Random());   //if there is no patrol path they face a random way
		}
	}

	private List<MapSpace> GetPlayerPath() {
		var startTime = System.DateTime.Now;
		List<MapSpace> objectives = new List<MapSpace>();
		foreach(var enemy in Spawner.GetEnemies) {
			objectives.Add(enemy.GetPosition());
		}
		foreach (var container in Spawner.GetContainers) {
			objectives.Add(container.position);
		}


		var player = party.members[0];
		List<MapSpace> path = new List<MapSpace>();
		var currentPosition = player.GetPosition();
		var backSteps = 1;
		var count = 0;
		Debug.Log(objectives.Count);
		while (objectives.Count > 0 && count < 500000) {
			if (!path.Contains(currentPosition)) {
				path.Add(currentPosition);
			}
			var adjacents = currentPosition.GetAdjacents();
			for(var i = adjacents.Count - 1; i>=0; i--) {
				var adjacent = adjacents[i];
				if (path.Contains(adjacent)) {
					adjacents.Remove(adjacent);
					continue;
				}
				if (objectives.Contains(adjacent)) {
					adjacents.Remove(adjacent);
					objectives.Remove(adjacent);
				}
				//we don't want to take a step to a spot that's adjacent to our existing path
				var twoStepsAway = adjacent.GetAdjacents();
				bool onEdge = false;
				twoStepsAway.Remove(currentPosition);
				foreach (var twoStep in twoStepsAway) {
					if (objectives.Contains(twoStep) || map.SpaceOnEdge(twoStep)) {
						onEdge = true;
						break;
					}	
				}
				if (onEdge) {
					continue;
				}
				foreach (var twoStep in twoStepsAway) {
					if (path.Contains(twoStep)){
						adjacents.Remove(adjacent);
						break;
					}
				}
			}
			if (adjacents.Count > 0) {
				var randomIndex = Random.Range(0, adjacents.Count);
				currentPosition = adjacents[randomIndex];
				backSteps = 1;
			}
			else {
				if(backSteps >= path.Count) {
					Debug.Log("How??? in " + count + " iterations (" + path.Count + " steps)");
					Debug.Log(objectives.Count + " objectives left");
					var eTime = System.DateTime.Now;
					Debug.Log("Pathfinding took " + (eTime - startTime).TotalSeconds + " to finish");
					return path;
				}
				currentPosition = path[path.Count - 1 - backSteps];
				backSteps++;
			}
			count++;
		}
		Debug.Log("Found all objectives in " + count + " iterations (" + path.Count + " steps)");

		var endTime = System.DateTime.Now;
		Debug.Log("Pathfinding took " + (endTime - startTime).TotalSeconds + " to finish");
		return path;

	}

	private async UniTask SpawnPlayers(List<Vector2Int> startingSpots) {
		foreach (var player in party.members) {
			await SpawnPlayer(player, startingSpots);
		}
	}

	private async UniTask FillMapWithObstacles(List<MapSpace> playerPath) {
		List<UniTask> tasks = new List<UniTask>();
		List<MapSpace> openSpaces = new List<MapSpace>(map.OpenSpaces());
		foreach (var space in openSpaces) {
			if (playerPath.Contains(space)) {
				continue;
			}
			tasks.Add(TrySpawnObstacle(space));
		}
		await UniTask.WhenAll(tasks);
	}

	private async UniTask SpawnPlayer(PlayableCharacter player, List<Vector2Int> availableSpots) {
		int randomSpot = Random.Range(0, availableSpots.Count);
		var space = map.GetSpaceFromVector2Int(availableSpots[randomSpot]);
		player.SetPosition(space);
		Controller.startingSpaces.Add(space);
		availableSpots.RemoveAt(randomSpot);
		await Spawner.SpawnPlayer(player);
	}

	private void SetEnemyPatrol(EnemyCharacter enemy) {
		DistanceCriteria[] criteria = { new DistanceCriteria(enemy.GetPosition(), enemy.Data.actionPoints, DistanceCriteria.Comparison.LessThanOrEqual) };
		enemy.patrolPath.Add(enemy.GetPosition());
		var target = map.GetRandomOpenSpaceWithDistanceCriteria(criteria);
		enemy.GetPosition().Passable = false;
		if (target == null) {
			Debug.Log("null enemy target");
			enemy.Status = EnemyStatus.Stationary;
			SetEnemyLookDirection(enemy);
			return;
		}
		enemy.target = target;
		enemy.patrolPath.Add(enemy.target);
		SetEnemyLookDirection(enemy);
	}

	private async UniTask<GameObject> SpawnObstacleInSpace(MapSpace space) {
		var obstacleReference = configuration.GetObstacle();
		var rotation = GetRandomRotatation();
		return await Spawner.SpawnObstacleInSpace(obstacleReference, space, rotation);
	}

	private List<Vector2Int> DetermineRandomStartingLocation(int numberOfRows, int numberOfColumns) {
		//set starting points for player party
		int rowOriented = Random.Range(0, 2);
		int nearOrFar = Random.Range(0, 2);
		int startRow = 0;
		int startColumn = 0;
		int partySize = party.members.Count;
		List<Vector2Int> startingSpots = new List<Vector2Int>();
		if (rowOriented == 1) {
			if (nearOrFar == 1) {
				startRow = numberOfRows - 1;
			}
			startColumn = Random.Range(partySize, numberOfColumns - 1);
			for (var i = 0; i < partySize; i++) {
				startingSpots.Add(new Vector2Int(startRow, startColumn - i));
			}
		}
		else {
			if (nearOrFar == 1) {
				startColumn = numberOfColumns - 1;
			}
			startRow = Random.Range(partySize, numberOfRows - 1);
			for (var i = 0; i < partySize; i++) {
				startingSpots.Add(new Vector2Int(startRow - i, startColumn));
			}
		}

		return startingSpots;
	}

	private async UniTask TrySpawnObstacle(MapSpace space) {
		float populationChance = Random.Range(0f, 1f);
		if (populationChance < configuration.ObstacleDensity) {
			await SpawnObstacleInSpace(space);
		}
	}


}
