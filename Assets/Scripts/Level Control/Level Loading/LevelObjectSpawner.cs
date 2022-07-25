using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Structures;

public class LevelObjectSpawner : MonoBehaviour {

	private Party party;
	private LevelConfiguration configuration;
	private List<EnemyCharacter> enemies = new List<EnemyCharacter>();
	private List<LevelInteractableContainer> containers = new List<LevelInteractableContainer>();
	private List<GameObject> spawnedObjects = new List<GameObject>();
	private SerializableLevel level;


	LevelController Controller => LevelController.Get();
	private static LevelObjectSpawner spawner;
	public static LevelObjectSpawner Get() {
		if(spawner != null)
		{
			return spawner;
		}
		spawner = FindObjectOfType<LevelObjectSpawner>();
		if (spawner == null) {
			throw new System.Exception("Level Spawner does not exist");
		}
		return spawner;
	}

	public void Initialize(SerializableLevel level, LevelConfiguration configuration, Party party) {
		this.party = party;
		this.configuration = configuration;
		this.level = level;

		Controller.ClearObjectives();
		foreach (var objective in configuration.environment.objectives) {
			Controller.AddObjective(objective);
		}
	}

	public void UnloadLevel() {
		Controller.SetState(LevelController.LevelState.Unloading);
		Controller.Reset();
		Despawn();
	}


	private void Despawn() {
		foreach(var spawnedObject in spawnedObjects) {
			Destroy(spawnedObject);
		}
		spawnedObjects.Clear();
		containers.Clear();
		enemies.Clear();
	}

	public List<EnemyCharacter> GetEnemies => enemies;
	public List<LevelInteractableContainer> GetContainers => containers;


	public async UniTask GenerateTileForSpace(AssetReferenceGameObject gameObject, MapSpace space) {

		space.tileObjectGUID = gameObject.AssetGUID;
		GameObject tileObject = await Addressables.InstantiateAsync(gameObject, this.transform);
		tileObject.transform.Translate(space.Row, 0f, space.Column);
		tileObject.name = space.ToString();
		BoxCollider lineOfSightCollider = tileObject.AddComponent<BoxCollider>();
		lineOfSightCollider.size = new Vector3(1f, 2f, 1f);
		lineOfSightCollider.center = new Vector3(0f, 1f, 0f);
		lineOfSightCollider.isTrigger = true;
		space.SetTileObject(tileObject);
		space.hoverMaterial = configuration.TileHoverMaterial;
		space.tileClicked += delegate {
			Controller.ProcessTileClick(space);
		};
		Controller.AddClickable(space, tileObject);
	}

	public async UniTask<GameObject> SpawnObstacleFromSpace(MapSpace space)
	{
		var obstacle = space.obstacle;
		var gameObject = new AssetReferenceGameObject(obstacle.GUID);
		return await SpawnObstacle(gameObject, space, obstacle.rotation);
	}

	public async UniTask<GameObject> SpawnObstacleInSpace(AssetReferenceGameObject gameObject, MapSpace space, float rotation) {

		space.AddObstacle(gameObject, rotation);
		return await SpawnObstacle(gameObject, space, rotation);
	}
	private async UniTask<GameObject> SpawnObstacle(AssetReferenceGameObject gameObject, MapSpace space, float rotation)
	{
		var spawnedObject = await SpawnObjectInSpace(gameObject, space, false);
		spawnedObject.transform.Rotate(new Vector3(0f, rotation, 0f));
		return spawnedObject;

	}


	public async UniTask SpawnContainer(LevelInteractableContainer container, AssetReferenceGameObject assetReference) {
		Controller.AddContainer(container);
		containers.Add(container);
		var returnObject = await SpawnObjectInSpace(assetReference, container.position, false);
		returnObject.transform.Rotate(new Vector3(0f, -container.rotation, 0f));
		returnObject.GetComponentInChildren<BillboardCanvas>().UpdateRotation(new Vector3(0f, container.rotation, 0f));
		container.SetGameObject(returnObject);

	}

	public async UniTask<GameObject> SpawnStructureObject(LevelStructure structure)
		=> await SpawnStructureObject(structure.Prefab, structure.rootSpace, structure.PrefabOffset, structure.rotation);
	


	public async UniTask<GameObject> SpawnStructureObject(AssetReferenceGameObject gameObject, MapSpace space, Vector3 offset, float rotation) {
		GameObject spawnedObject = await Addressables.InstantiateAsync(gameObject, this.transform);
		spawnedObject.transform.Translate(space.Row + offset.x, 0f, space.Column + offset.z);
		spawnedObject.transform.Rotate(new Vector3(0f, -rotation, 0f));
		spawnedObjects.Add(spawnedObject);
		return spawnedObject;
	}

	private async UniTask<GameObject> SpawnObjectInSpace(AssetReferenceGameObject gameObject, MapSpace space, bool passable) {

		GameObject spawnedObject = await Addressables.InstantiateAsync(gameObject, this.transform);
		spawnedObject.transform.Translate(space.Row, 0f, space.Column);
		spawnedObjects.Add(spawnedObject);
		if (passable) {
			space.ClaimPositionPassable(spawnedObject, spawnedObject.layer);
		}
		else {
			space.ClaimPositionImpassable(spawnedObject, spawnedObject.layer);
		}
		return spawnedObject;
	}

	public async UniTask SpawnStepActivatedEvent(StepActivatedEvent activatedEvent, AssetReferenceGameObject gameObject) {
		Controller.stepActivatedEvents.Add(activatedEvent);
		var activatedObject = await SpawnObjectInSpace(gameObject, activatedEvent.activationLocation, true);
		activatedEvent.TrySetActivatedObject(activatedObject);

	}

	public async UniTask SpawnEnemy(EnemyCharacter enemy) {
		var enemySpot = enemy.GetPosition();
		GameObject enemyObject = await Addressables.InstantiateAsync(enemy.Data.prefab, this.transform);

		enemySpot.ClaimPositionPassable(enemyObject, enemy.GetLayer());
		enemyObject.transform.Translate(enemySpot.Row, 0f, enemySpot.Column);
		enemyObject.name = enemy.GetName();
		enemy.SetGameObject(enemyObject);
		enemies.Add(enemy);
		Controller.AddEnemy(enemy);
		Controller.AddClickable(enemy, enemyObject);
		enemy.onClick += delegate {
			Controller.ProcessEnemyClick(enemy);
		};
	}

	public async UniTask SpawnPlayer(PlayableCharacter player) {

		GameObject playerObject = await Addressables.InstantiateAsync(player.Data.prefab, this.transform);
		playerObject.transform.Translate(player.GetPosition().Row, 0f, player.GetPosition().Column);
		playerObject.name = player.GetName();
		playerObject.layer = player.GetLayer();
		foreach (Transform t in playerObject.transform) {
			t.gameObject.layer = player.GetLayer();
		}
		player.GetPosition().ClaimPositionImpassable(playerObject, player.GetLayer());
		player.SetGameObject(playerObject);
		Controller.AddClickable(player, playerObject);
		player.onClick = new UnityAction(() => {
			Controller.ProcessPlayerClick(player);
		});
	}

	public async void StartGame() {
		FindObjectOfType<CameraController>().AlignCamera(party.members[0].GameObject.transform.position);
		//wait for the end of the fixedupdate frame to ensure colliders are in place before calcing vision
		await UniTask.WaitForFixedUpdate();
		await UniTask.WaitForEndOfFrame();
		foreach (var player in party.members) {
			player.UpdateVisionProfile();
		}
		foreach (var enemy in enemies) {
			enemy.UpdateVisionProfile();
		}
		Controller.UpdateVisionIndicators();

		Controller.SetState(LevelController.LevelState.PlayerTurn);
		UIController.Get().ShowGame();
	}
}