using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using Structures;

public class ObsoleteLevelLoader : MonoBehaviour
{
	//Party party;
	//LevelController controller;
	//List<GameObject> spawnedObjects = new List<GameObject>();
	//LevelMap map;
	//List<PlayableCharacterData> players = new List<PlayableCharacterData>();
	//LevelConfiguration configuration;
	//List<EnemyCharacter> enemies = new List<EnemyCharacter>();
	//public List<MapSpace> potentialObjectiveLocations = new List<MapSpace>();
	//MapSpace basePoint;

	//SerializableLevel level;
	//SerializableLevel loadingLevel;
	//public static ObsoleteLevelLoader Get() {
	//	ObsoleteLevelLoader loader = (ObsoleteLevelLoader)FindObjectOfType(typeof(ObsoleteLevelLoader));
	//	if (loader == null) {
	//		throw new System.Exception("Level Loader does not exist");
	//	}
	//	return loader;
	//}

	//public async void LoadFromSave(SaveData saveData) {
	//	controller = LevelController.Get();
	//	UnloadLevel();

	//	controller.SetState(LevelController.LevelState.Loading);


	//	level = new SerializableLevel();
	//	controller.SetLevel(level);
	//	loadingLevel = saveData.level;
	//	configuration = loadingLevel.GetConfiguration();
	//	level.SetConfiguration(configuration);
	//	map = loadingLevel.GetMap();
	//	controller.map = map;
	//	party = new Party(saveData.party, loadingLevel);
	//	controller.SetParty(party);
	//	var objectiveLocation = loadingLevel.GetObjectiveLocation();
	//	controller.SetObjective(objectiveLocation);
	//	level.SetObjectiveLocation(objectiveLocation);
	//	List<UniTask> tasks = new List<UniTask>();
		
	//	foreach(var space in map.spaces) {
	//		var assetReference = new AssetReferenceGameObject(space.tileObjectGUID);
	//		tasks.Add(GenerateTileForSpace(assetReference, space));
	//	}
	//	foreach(var obstacle in loadingLevel.GetObstacles()) {
	//		var assetReference = new AssetReferenceGameObject(obstacle.obstacleGUID);
	//		tasks.Add(SpawnObstacleInSpace(assetReference, loadingLevel.GetSpace(obstacle.position)));
	//	}
	//	foreach(var structure in loadingLevel.GetStructures()) {
	//		tasks.Add(LoadStructure(structure, loadingLevel.GetSpace(structure.position)));
	//		level.AddStructure(structure);
	//	}
	//	foreach (var objective in loadingLevel.GetObjectives()) {
	//		var assetReference = new AssetReferenceGameObject(objective.objective);
	//		tasks.Add(SpawnObjective(assetReference, loadingLevel.GetSpace(objective.position), objective.rotation));
	//	}
	//	foreach (var serializedEnemy in loadingLevel.GetEnemies()) {
	//		var enemy = new EnemyCharacter(serializedEnemy);
	//		enemy.SetPosition(loadingLevel.GetSpace(serializedEnemy.position));
	//		if (serializedEnemy.target != null) {
	//			enemy.target = loadingLevel.GetSpace(serializedEnemy.target);
	//		}
	//		foreach(var pathPoint in serializedEnemy.patrolPath) {
	//			enemy.patrolPath.Add(loadingLevel.GetSpace(pathPoint));
	//		}
	//		tasks.Add(SpawnEnemy(enemy));
	//	}
	//	foreach (var player in party.members) {
	//		tasks.Add(SpawnPlayer(player));
	//		if (player.isActive) {
	//			controller.SetActivePlayer(player);
	//		}
	//	}
	//	await UniTask.WhenAll(tasks);

	//	Pathfinding.GenerateNodeMap(map);
	//	controller.SetState(LevelController.LevelState.PlayerTurn);
	//	UIController.Get().ShowGame();
	//	StartGame();
	//}

	//#region level generation
	//public void Generate(List<PlayableCharacterData> players, LevelConfiguration configuration) {
	//	controller = LevelController.Get();
	//	this.players = players;
	//	this.configuration = configuration;
	//	UnloadLevel();
	//	GenerateLevel();
	//}

	//public void Regenerate() {
	//	Generate(players, configuration);
	//}

	//private void UnloadLevel() {
	//	controller.SetState(LevelController.LevelState.Unloading);
	//	controller.Reset();

	//	foreach (var gameObject in spawnedObjects) {
	//		Destroy(gameObject);
	//	}
	//	spawnedObjects.Clear();
	//}

	//public async void GenerateLevel() {

	//	controller.SetState(LevelController.LevelState.Loading);
	//	if (configuration == null) {
	//		throw new System.Exception("level configuration cannot be null");
	//	}

	//	level = new SerializableLevel();
	//	controller.SetLevel(level);

	//	level.SetConfiguration(configuration);
	//	if (party == null) {
	//		party = new Party();
	//	}
	//	party.Clear();
	//	party.AddPlayers(players);
	//	controller.SetParty(party);

	//	int baseRowCount = configuration.RowCount;
	//	int baseColumnCount = configuration.ColumnCount;

	//	//do other stuff like randomize final values based on type and difficulty


	//	List<Vector2Int> startingSpots = DetermineRandomStartingLocation(baseRowCount, baseColumnCount);

	//	controller.map = new LevelMap(baseRowCount, baseColumnCount);
	//	map = controller.GetMap();
	//	List<UniTask> tasks = new List<UniTask>();
	//	foreach (var space in map.spaces) {
	//		tasks.Add(GenerateTileForSpace(configuration.GetTile(), space));

	//		if (map.SpaceOnEdge(space)) {
	//			Vector2Int positionVector = new Vector2Int(space.row, space.column);
	//			if (!startingSpots.Contains(positionVector)) {
	//				tasks.Add(SpawnObstacleInSpace(space));
	//			}
	//		}
	//	}
	//	await tasks;
	//	var startSpace = map.GetSpaceFromVector2Int(startingSpots[0]);
	//	var basePointCriteria = configuration.GetBaseCriteria(startSpace);
	//	basePoint = map.GetRandomOpenSpaceWithDistanceCriteria(basePointCriteria);
	//	Debug.Log("Base Point:" + basePoint.GetName());


	//	//spawn objective
	//	//DistanceCriteria[] objectiveCriteria = { new DistanceCriteria(basePoint, configuration.ObjectiveDistance, DistanceCriteria.Comparison.LessThanOrEqual) };
	//	//controller.SetObjective(await SpawnObjectWithDistanceCriteria(configuration.environment.GetObjective(), objectiveCriteria, true));
	//	await SpawnStructures();
	//	//await SpawnEnemies(startingSpots);

	//	await SpawnObjectives();

	//	await SpawnPlayers(startingSpots);
	//	await FillMapWithObstacles();


	//	Pathfinding.GenerateNodeMap(map);
	//	bool validConfiguration = CheckForValidConfiguration();
	//	if (validConfiguration) {
	//		InitializeEnemyStatuses();
	//		StartGame();
	//	}
	//	else {
	//		Regenerate();
	//	}
	//}

	//private async void StartGame() {
	//	FindObjectOfType<CameraController>().AlignCamera(party.members[0].GetGameObject().transform.position);
	//	//wait for the end of the fixedupdate frame to ensure colliders are in place before calcing vision
	//	await UniTask.WaitForFixedUpdate();
	//	await UniTask.WaitForEndOfFrame();
	//	foreach (var player in party.members) {
	//		player.UpdateVisionProfile();
	//	}
	//	foreach(var enemy in enemies) {
	//		enemy.UpdateVisionProfile();
	//	}
	//	controller.UpdateVisionIndicators();

	//	controller.SetState(LevelController.LevelState.PlayerTurn);
	//	UIController.Get().ShowGame();
	//}

	//private async UniTask SpawnObjectives() {
	//	int floatingObjectivePoints = Mathf.RoundToInt(potentialObjectiveLocations.Count / configuration.environment.floatingObjectiveChance);
	//	for(var i=0; i< floatingObjectivePoints; i++) {
	//		DistanceCriteria[] objectiveCriteria = { new DistanceCriteria(basePoint, configuration.ObjectiveDistance, DistanceCriteria.Comparison.LessThanOrEqual) };
	//		var floatSpace = map.GetRandomOpenSpaceWithDistanceCriteria(objectiveCriteria);
	//		if(floatSpace == null) {
	//			continue;
	//		}
	//		potentialObjectiveLocations.Add(floatSpace);
	//	}
	//	var objectiveLocation = ChooseRandomPotentialObjectiveLocation();
	//	controller.SetObjective(objectiveLocation);
	//	level.SetObjectiveLocation(objectiveLocation);
	//	List<UniTask> tasks = new List<UniTask>();
	//	tasks.Add(SpawnObjectiveInSpace(objectiveLocation));
	//	for (var i=0; i < configuration.environment.dummyObjectiveCount; i++) {
	//		if(potentialObjectiveLocations.Count == 0) {
	//			break;
	//		}
	//		objectiveLocation = ChooseRandomPotentialObjectiveLocation();
	//		tasks.Add(SpawnObjectiveInSpace(objectiveLocation));
	//	}
	//	await UniTask.WhenAll(tasks);
	//}

	//private async UniTask SpawnObjectiveInSpace(MapSpace space) {
	//	var gameObject = configuration.environment.GetObjective();
	//	var rotation = GetRandomRotatation();
	//	await SpawnObjective(gameObject, space, rotation);

	//}

	//private async UniTask SpawnObjective(AssetReferenceGameObject objective, MapSpace space, float rotation) {
	//	level.AddObjective(objective, space, rotation);

	//	var returnObject = await SpawnObjectInSpace(objective, space, true);
	//	returnObject.transform.Rotate(new Vector3(0f, -rotation, 0f));

	//}

	//private MapSpace ChooseRandomPotentialObjectiveLocation() {
	//	int randomIndex = Random.Range(0, potentialObjectiveLocations.Count);
	//	var randomSpace = potentialObjectiveLocations[randomIndex];
	//	potentialObjectiveLocations.RemoveAt(randomIndex);
	//	return randomSpace;
	//}

	//private async UniTask SpawnStructures() {
	//	enemies.Clear();
	//	foreach (var structure in configuration.environment.structures) {
	//		await SpawnStructure(structure);
	//	}
	//}

	//private float GetRandomRotatation() {
	//	float[] rotations = { 0f, 90f, 180f, 270f };
	//	int index = Random.Range(0, 4);
	//	return rotations[index];
	//}

	//private async UniTask LoadStructure(SerializableStructure runTimeStructure, MapSpace space) {
	//	var structure = new LevelStructure(ResourceLoader.GetStructure(runTimeStructure.structure));
	//	var gameObject = await SpawnStructureObject(structure.Prefab, space, structure.PrefabOffset, runTimeStructure.rotation);

	//	var spaces = structure.GetRotatedSpaces(runTimeStructure.rotation);
	//	foreach (var structureSpace in spaces) {
	//		var levelSpace = map.GetSpaceFromCoordinates(space.row + structureSpace.row, space.column + structureSpace.column);
	//		if (levelSpace == null) {
	//			continue;
	//		}
	//		levelSpace.ClaimPositionPassable(gameObject, gameObject.layer);
	//		levelSpace.PlaceStructure(structureSpace, gameObject.GetComponent<StructureSceneObject>());
	//	}
	//}

	//private async UniTask<GameObject> SpawnStructureObject(AssetReferenceGameObject gameObject, MapSpace space, Vector3 offset, float rotation) {
	//	GameObject spawnedObject = await Addressables.InstantiateAsync(gameObject, this.transform);
	//	spawnedObject.transform.Translate(space.row + offset.x, 0f, space.column + offset.z);
	//	spawnedObject.transform.Rotate(new Vector3(0f, -rotation, 0f));
	//	spawnedObjects.Add(spawnedObject);
	//	return spawnedObject;
	//}
	//private async UniTask SpawnStructure(LevelStructureData structureData) {
	//	var structure = new LevelStructure(structureData);
	//	DistanceCriteria[] criteria = structure.GetCriteria(basePoint);
	//	var structSpace = map.GetRandomOpenSpaceWithDistanceCriteria(criteria);
	//	if(structSpace == null) {
	//		return;
	//	}

	//	float rotation = GetRandomRotatation();


	//	var gameObject = await SpawnStructureObject(structure.Prefab, structSpace, structure.PrefabOffset, rotation);

	//	level.AddStructure(structure, structSpace, rotation);

	//	var spaces = structure.GetRotatedSpaces(rotation);
	//	foreach(var space in spaces) {
	//		var levelSpace = map.GetSpaceFromCoordinates(structSpace.row + space.row, structSpace.column + space.column);
	//		if(levelSpace == null) {
	//			continue;
	//		}
	//		levelSpace.ClaimPositionPassable(gameObject, gameObject.layer);
	//		levelSpace.PlaceStructure(space, gameObject.GetComponent<StructureSceneObject>());

	//		if (space.objectiveLocation) {
	//			potentialObjectiveLocations.Add(levelSpace);
	//		}
	//	}
	//	foreach(var enemy in structure.enemyConfiguration.GetEnemies(map, structSpace)) {
	//		if(enemy == null) {
	//			continue;
	//		}
	//		await SpawnEnemy(enemy);
	//	}

	//}

	//private void InitializeEnemyStatuses() {
	//	foreach (var enemy in enemies) {
	//		switch (enemy.Status) {
	//			case EnemyStatus.Patrol:
	//				SetEnemyPatrol(enemy);
	//				break;
	//			default:
	//				enemy.GetPosition().Passable = false;
	//				enemy.SetFacing(new Direction().Random());
	//				break;
	//		}
	//	}
	//}

	//private static void SetEnemyLookDirection(EnemyCharacter enemy) {
	//	var patrolPath = enemy.GetPath();
	//	if (patrolPath.Count > 0) {
	//		Vector2 direction = patrolPath[0].SubtractFrom(enemy.GetPosition());
	//		enemy.SetFacing(new Direction(direction));
	//	}
	//	else {
	//		enemy.SetFacing(new Direction().Random());   //if there is no patrol path they face a random way
	//	}
	//}

	//private bool CheckForValidConfiguration() {
	//	//return true;
	//	foreach (var player in party.members) {
	//		var path = Pathfinding.GetPath(player.GetPosition(), controller.GetObjective());
	//		if (path.Count > 0) {
	//			return true;
	//		}
	//	}
	//	return false;
	//}

	//private async UniTask SpawnPlayers(List<Vector2Int> startingSpots) {
	//	List<UniTask> tasks = new List<UniTask>();
	//	foreach (var player in party.members) {
	//		tasks.Add(SpawnPlayer(player, startingSpots));
	//	}
	//	await UniTask.WhenAll(tasks);
	//}

	//private async UniTask FillMapWithObstacles() {
	//	List<UniTask> tasks = new List<UniTask>();
	//	List<MapSpace> openSpaces = new List<MapSpace>(map.OpenSpaces());
	//	foreach (var space in openSpaces) {
	//		tasks.Add(TrySpawnObstacle(space));
	//	}
	//	await UniTask.WhenAll(tasks);
	//}

	//private async UniTask SpawnPlayer(PlayableCharacter player, List<Vector2Int> availableSpots) {
	//	int randomSpot = Random.Range(0, availableSpots.Count);
	//	player.SetPosition(map.GetSpaceFromVector2Int(availableSpots[randomSpot]));
	//	availableSpots.RemoveAt(randomSpot);
	//	await SpawnPlayer(player);
	//}

	//private async UniTask SpawnPlayer(PlayableCharacter player) {
		
	//	GameObject playerObject = await Addressables.InstantiateAsync(player.Data.prefab, this.transform);
	//	playerObject.transform.Translate(player.GetPosition().row, 0f, player.GetPosition().column);
	//	playerObject.name = player.GetName();
	//	playerObject.layer = player.GetLayer();
	//	foreach (Transform t in playerObject.transform) {
	//		t.gameObject.layer = player.GetLayer();
	//	}
	//	player.GetPosition().ClaimPositionImpassable(playerObject, player.GetLayer());
	//	player.SetGameObject(playerObject);
	//	controller.AddClickable(player,playerObject);
	//	player.onClick = new UnityAction(() => {
	//		controller.ProcessPlayerClick(player);
	//	});
	//}

	//private async UniTask SpawnEnemy(EnemyCharacter enemy) {
	//	var enemySpot = enemy.GetPosition();
	//	GameObject enemyObject = await Addressables.InstantiateAsync(enemy.Data.prefab, this.transform);

	//	enemySpot.ClaimPositionPassable(enemyObject, enemy.GetLayer());
	//	enemyObject.transform.Translate(enemySpot.row, 0f, enemySpot.column);
	//	enemyObject.name = enemy.GetName();
	//	enemy.SetGameObject(enemyObject);
	//	enemies.Add(enemy);
	//	controller.AddEnemy(enemy);
	//	controller.AddClickable(enemy, enemyObject);
	//	enemy.onClick += delegate {
	//		controller.ProcessEnemyClick(enemy);
	//	};
	//}

	//private void SetEnemyPatrol(EnemyCharacter enemy) {
	//	DistanceCriteria[] criteria = { new DistanceCriteria(enemy.GetPosition(), enemy.Data.speed, DistanceCriteria.Comparison.Equals) };
	//	enemy.patrolPath.Add(enemy.GetPosition());
	//	var target = map.GetRandomOpenSpaceWithDistanceCriteria(criteria);
	//	enemy.GetPosition().Passable = false;
	//	if (target == null) {
	//		SetEnemyLookDirection(enemy);
	//		return;
	//	}
	//	enemy.target = target;
	//	enemy.patrolPath.Add(enemy.target);
	//	SetEnemyLookDirection(enemy);
	//}

	//private async UniTask GenerateTileForSpace(AssetReferenceGameObject gameObject, MapSpace space) {

	//	space.tileObjectGUID = gameObject.AssetGUID;
	//	GameObject tileObject = await Addressables.InstantiateAsync(gameObject, this.transform);
	//	tileObject.transform.Translate(space.row, 0f, space.column);
	//	tileObject.name = "Tile " + space.GetName();
	//	BoxCollider lineOfSightCollider = tileObject.AddComponent<BoxCollider>();
	//	lineOfSightCollider.size = new Vector3(1f, 2f, 1f);
	//	lineOfSightCollider.center = new Vector3(0f, 1f, 0f);
	//	lineOfSightCollider.isTrigger = true;
	//	space.SetTileObject(tileObject);
	//	space.hoverMaterial = configuration.TileHoverMaterial;
	//	space.tileClicked += delegate {
	//		controller.ProcessTileClick(space);
	//	};
	//	controller.AddClickable(space, tileObject);
	//}

	//private async UniTask<GameObject> SpawnObstacleInSpace(AssetReferenceGameObject gameObject, MapSpace space) {

	//	level.AddObstacle(space, gameObject);
	//	space.hasObstacle = true;
	//	return await SpawnObjectInSpace(gameObject, space, false);
	//}

	//private async UniTask<GameObject> SpawnObstacleInSpace(MapSpace space) {
	//	var obstacleReference = configuration.GetObstacle();
	//	return await SpawnObstacleInSpace(obstacleReference, space);
	//}

	//private async UniTask<GameObject> SpawnObjectInSpace(AssetReferenceGameObject gameObject, MapSpace space, bool passable) {
		
	//	GameObject spawnedObject = await Addressables.InstantiateAsync(gameObject, this.transform);
	//	spawnedObject.transform.Translate(space.row, 0f, space.column);
	//	spawnedObjects.Add(spawnedObject);
	//	if (passable) {
	//		space.ClaimPositionPassable(spawnedObject, spawnedObject.layer);
	//	}
	//	else {
	//		space.ClaimPositionImpassable(spawnedObject, spawnedObject.layer);
	//	}
	//	return spawnedObject; 
	//}

	//private List<Vector2Int> DetermineRandomStartingLocation(int numberOfRows, int numberOfColumns) {
	//	//set starting points for player party
	//	int rowOriented = Random.Range(0, 2);
	//	int nearOrFar = Random.Range(0, 2);
	//	int startRow = 0;
	//	int startColumn = 0;
	//	int partySize = party.members.Count;
	//	List<Vector2Int> startingSpots = new List<Vector2Int>();
	//	if (rowOriented == 1) {
	//		if (nearOrFar == 1) {
	//			startRow = numberOfRows - 1;
	//		}
	//		startColumn = Random.Range(partySize, numberOfColumns - 1);
	//		for (var i = 0; i < partySize; i++) {
	//			startingSpots.Add(new Vector2Int(startRow, startColumn - i));
	//		}
	//	}
	//	else {
	//		if (nearOrFar == 1) {
	//			startColumn = numberOfColumns - 1;
	//		}
	//		startRow = Random.Range(partySize, numberOfRows - 1);
	//		for (var i = 0; i < partySize; i++) {
	//			startingSpots.Add(new Vector2Int(startRow - i, startColumn));
	//		}
	//	}

	//	return startingSpots;
	//}

	//private async UniTask TrySpawnObstacle(MapSpace space) {
	//	float populationChance = Random.Range(0f, 1f);
	//	if (populationChance < configuration.ObstacleDensity) {
	//		await SpawnObstacleInSpace(space);
	//	}
	//}
	//#endregion

}
