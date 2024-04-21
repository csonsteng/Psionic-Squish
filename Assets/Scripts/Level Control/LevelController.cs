using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

public partial class LevelController : MonoBehaviour
{
	public LevelState currentState;
	public LevelConfiguration defaultConfiguration;
	public TextMeshProUGUI winText;

	public Material tileHoverMaterial;
	public GameObject soundIndicator;

	public List<PlayableCharacterData> defaultPlayers = new List<PlayableCharacterData>();
	public PlayableCharacterData mage;
	public PlayableCharacterData ranger;

	public AssetReferenceGameObject droppedItems;

	public List<MapSpace> startingSpaces = new List<MapSpace>();
	public List<StepActivatedEvent> stepActivatedEvents = new List<StepActivatedEvent>();
	public MapSpace basePoint;

	public LevelMap map;
	private Party party;
	private List<GameObject> spawnedObjects = new List<GameObject>();
	private ClickablesHandler clickHandler;
	private Dictionary<GameObject, ITargetable> targetables = new Dictionary<GameObject, ITargetable>();

	private List<AbstractCharacter> movingPlayers = new List<AbstractCharacter>();
	private List<EnemyCharacter> enemies = new List<EnemyCharacter>();
	private List<LevelInteractableContainer> containers = new List<LevelInteractableContainer>();

	//private MapSpace objective;
	private List<AbstractLevelObjective> objectives = new List<AbstractLevelObjective>();
	private CharacterAction activeAction;

	private SerializableLevel level;
	public int turnNumber = 0;
	public int rewindCount = 3;
	public AbstractCharacter movingCharacter;

	private PlayableCharacter lastPlayer;

	LevelOutcome outcome = LevelOutcome.Quit;
	
	public enum LevelState {
		Loading,
		PlayerTurn,
		EnemyTurn,
		Unloading,
		Pause
	}

	public enum LevelOutcome {
		Success,
		Failure,
		Quit
	}

	public async UniTask Play() {
		await LevelGenerator.Get().Generate(defaultConfiguration);
	}

	public async void GameStarted() {
		foreach(var player in party.members) {
			await player.TryFireTriggers(Trigger.LevelStart);
		}
		foreach(var enemy in enemies) {
			await enemy.TryFireTriggers(Trigger.LevelStart);
		}
	}

	private void Start() {
		clickHandler = GetComponent<ClickablesHandler>();
		if (clickHandler != null) {
			clickHandler.objectClicked.AddListener(ProcessClick);
			clickHandler.objectHovered.AddListener(ProcessHover);
		}
	}

	private void Update() {
		if (winText.gameObject.activeSelf) {
			if (winText.fontSize < 80) {
				winText.fontSize += 1;
			}
			else {
				winText.gameObject.SetActive(false);
				switch (outcome) {
					case LevelOutcome.Success:
						LevelGenerator.Get().GenerateLevel().Forget();
						break;
				}
				UIController.Get().ShowRestartButton();
			}
		}

		
	}

	public void ShowSound(GameObject parent) {
		var soundObject = Instantiate(soundIndicator, parent.transform);
		soundObject.transform.parent = this.transform;
	}

	private static LevelController _instance;
	public static LevelController Get() {
		if(_instance == null)
		{
			_instance = (LevelController)FindObjectOfType(typeof(LevelController));
		}
		return _instance;
	}

	public LevelMap GetMap() {
		return map;
	}

	public void ClearObjectives() {
		objectives.Clear();
	}

	public void AddObjective(AbstractLevelObjective objective) {
		objectives.Add(objective);
	}

	public List<AbstractLevelObjective> GetObjectives() {
		return objectives;
	}

	public void SetParty(Party party) {
		this.party = party;
	}

	public Party GetParty() {
		return party;
	}
	public void AddEnemy(EnemyCharacter enemy) {
		if(enemy == null) {
			return;
		}
		enemies.Add(enemy);
	}

	public IEnumerable<EnemyCharacter> GetEnemies() {
		foreach(var enemy in enemies) {
			yield return enemy;
		}
	}

	public void SetState(LevelState state) {
		currentState = state;
	}

	public void AddClickable(ITargetable clickable, GameObject gameObject) {
		if(targetables == null) {
			targetables = new Dictionary<GameObject, ITargetable>();
		}
		targetables.Add(gameObject,clickable);
	}

	public void AddContainer(LevelInteractableContainer container) {
		containers.Add(container);
	}

	public List<LevelInteractableContainer> GetContainers() {
		return containers;
	}

	public void SetLevel(SerializableLevel level) {
		this.level = level;
	}

	private void OnBeforeSave() {
		level.SetTurnNumber(turnNumber);
		level.ClearEnemies();
		foreach (var enemy in enemies) {
			level.AddEnemy(enemy);
		}
		level.ClearContainers();
		foreach (var container in containers) {
			level.AddContainer(container);
		}
		level.rewindCount = rewindCount;
		level.SetMap(map);
	}

	private void SaveRewind() {
		OnBeforeSave();
		SaveLoadUtility.SaveForRewind(level, party);
	}

	public void Save() {
		OnBeforeSave();
		SaveLoadUtility.SaveInActiveSlot(level, party);
	}

	public void Load() {
		var saveData = SaveLoadUtility.LoadFromActiveSlot();
		LevelLoader.Get().LoadFromSave(saveData).Forget();
	}

	public async void Rewind() {
		if(rewindCount <= 0 || turnNumber <= 1) {
			return;
		}
		var oldRewinds = rewindCount;
		var saveData = SaveLoadUtility.LoadRewind(turnNumber-1);
		await LevelLoader.Get().LoadFromSave(saveData);
		rewindCount = oldRewinds - 1;
		if(rewindCount <= 0) {
			UIController.Get().DisableRewinds();
		}
	}

	public void Reset() {
		winText.gameObject.SetActive(false);

		level = null;
		targetables.Clear();
		ActivePlayer = null;
		if (map != null) {
			map.Clear();
			map = null;
		}
		//if (party != null) {
		//	party.Clear();
		//	party = null;
		//}
		turnNumber = 0;
		rewindCount = 3;
		foreach (var gameObject in spawnedObjects) {
			Destroy(gameObject);
		}
		spawnedObjects.Clear();

		foreach (var enemy in enemies) {
			if (enemy.GetGameObject() != null) {
				Destroy(enemy.GetGameObject());
			}
		}
		enemies.Clear();
		containers.Clear();

		activeAction = null;
		UIController.Get().EnableRewinds();
	}

	public bool IsPaused => currentState == LevelState.Pause;

	//------------input handling------------------------------------

	public void UnHoverAll() {
		foreach (var ITarget in targetables) {
			ITarget.Value.HandleUnHover();
		}
	}
	private void ProcessHover() {
		UnHoverAll();

		//guard clauses for enemy turn, during movement, and if the hovered object isn't clickable
		if (currentState != LevelState.PlayerTurn) {
			return;
		}
		if(movingPlayers.Count > 0) {
			return;
		}
		if (!targetables.ContainsKey(clickHandler.hoveredObject)) {
			return;
		}


		var targetable = targetables[clickHandler.hoveredObject];
		if (activeAction != null && ActivePlayer != null) {
			activeAction.Target = targetable;
			if (activeAction.ValidTarget()) {
				activeAction.Preview();
			}
		}
		//if (targetable.GetType() == typeof(MapSpace)) {
		//	if (activePlayer == null) {
		//		return;
		//	}

			
		//}
		//else if(targetable.GetType() == typeof(PlayableCharacter)) {
		//	if (activeAction == null) {
		//		targetable.HandleHover();
		//	}
		//	else {
		//		activeAction.Target = (PlayableCharacter)targetable;
		//		if (activeAction.ValidTarget()) {
		//			targetable.HandleHover();
		//		}
		//	}
		//}
		//else if(targetable.GetType() == typeof(EnemyCharacter)) {
		//	if(activeAction == null) {
		//		return;
		//	}
		//	activeAction.Target = (EnemyCharacter)targetable;
		//	if (activeAction.ValidTarget()) {
		//		targetable.HandleHover();
		//	}
		//}
		//else {
		//	targetable.HandleHover();
		//}
			
		
	}

	private void ProcessClick() {
		if (currentState == LevelState.PlayerTurn && movingPlayers.Count == 0) {
			if (targetables.ContainsKey(clickHandler.hitObject)) {

				var targetable = targetables[clickHandler.hitObject];
				targetable.HandleClick();
			}
		}
	}

	public async void ProcessPlayerClick(PlayableCharacter player) {
		if(activeAction == null){
			SetActivePlayer(player);
			return;
		}
		if (activeAction.ValidTarget(player)) {
			await activeAction.Invoke();
			Save();
			return;
		}
		SetActivePlayer(player);
	}

	public async void ProcessEnemyClick(EnemyCharacter enemy) {
		if(activeAction == null) {
			return;
		}

		if (activeAction.ValidTarget(enemy)){
			await activeAction.Invoke();
			Save();
		}

	}

	public void SetActivePlayer(PlayableCharacter player) {
		SetActiveAction(null);
		if (ActivePlayer != null) {
			ActivePlayer.isActive = false;
			ActivePlayer.HandleUnHover();
		}
		foreach (var container in containers) {
			container.HideInterface();
		}
		ActivePlayer = player;
		if (ActivePlayer != null) {
			ActivePlayer.GetPosition().HandleHover();
			ActivePlayer.isActive = true;

			if (IsPositionNextToContainer(ActivePlayer.GetPosition(), out var adjacentContainers)) {
				foreach (var container in adjacentContainers) {
					container.ShowInterface(ActivePlayer);
				}
			}
			SetActiveAction(ActivePlayer.DefaultAction());
			UIController.Get().PlayersTurn();
		}
		Save();
	}

	public async void SetActiveAction(CharacterAction action) {
		if(activeAction != null) {
			activeAction.IsActive = false;
		}
		UnHoverAll();
		if(action == null || action.Owner != ActivePlayer) {
			activeAction = null;
			return;
		}
		if (!action.RequiresTargets) {
			await action.Invoke();
			Save();
			activeAction = null;
			return;
		}
		activeAction = action;
		action.IsActive = true;
		UIController.Get().UpdateActionInteractability();

	}

	public CharacterAction ActiveAction => activeAction;
	public PlayableCharacter ActivePlayer { get; private set; }

	public async void ProcessTileClick(MapSpace clickedTile) {
		if(activeAction == null) {
			return;
		}
		if (activeAction.ValidTarget(clickedTile)) {
			await activeAction.Invoke();
			Save();
		}

	}

	public bool IsPositionNextToContainer(MapSpace space, out List<LevelInteractableContainer> containers) {
		var adjacents = space.GetAdjacents();
		containers   = new List<LevelInteractableContainer>();
		foreach (var container in this.containers) {
			if (adjacents.Contains(container.position)) {
				containers.Add(container);
			}
		}
		if(containers.Count > 0) {
			return true;
		}
		return false;
	}

	public bool IsPositionAPlayer(MapSpace space, out PlayableCharacter returnPlayer) {
		foreach(var player in party.members) {
			if(player.GetPosition() == space) {
				returnPlayer = player;
				return true;
			}
		}
		returnPlayer = null;
		return false;
	}

	#region movement
	private async UniTask Move(MapSpace goalSpace) {
		if(goalSpace == null) {
			return;
		}
		if(ActivePlayer == null) {
			return;
		}
		if (movingPlayers.Contains(ActivePlayer)) {
			return;
		}
			
		await MoveToDestination(ActivePlayer, goalSpace, true);
		
	}
	private async UniTask MoveToDestination(AbstractCharacter movingPlayer, MapSpace goalSpace, bool avoidVision = false) {
		movingPlayers.Add(movingPlayer);
		var path = Pathfinding.GetPath(movingPlayer.GetPosition(), goalSpace, avoidVision);
		while (path.Count > 0 && movingPlayer.ActionPoints() > 0 && currentState != LevelState.Pause) {
			movingCharacter = movingPlayer;
			await TakeStep(path[0]);
			path.RemoveAt(0);
		}
		movingPlayer.Idle();
		movingPlayers.Remove(movingPlayer);
	}

	private bool CheckObjectives() {
		foreach(var objective in objectives) {
			if (!objective.ObjectiveComplete()) {
				return false;
			}
		}
		return true;
	}

	public async UniTask TakeStep(MapSpace step) {
		if (!step.Passable) {
			return;
		}
		step.ClaimPositionImpassable(movingCharacter.GetGameObject(), movingCharacter.GetLayer());
		var deltaStep = step.SubtractFrom(movingCharacter.GetPosition());
		var deltaRow = deltaStep.x;
		var deltaColumn = deltaStep.y;
		Vector3 nextLocation = movingCharacter.GetGameObject().transform.position + new Vector3(deltaRow, 0f, deltaColumn);
		movingCharacter.SetFacing(new Direction(deltaStep));

		var moveSpeed = movingCharacter.SetMoving();
		Tween tween = movingCharacter.GetGameObject().transform.DOMove(nextLocation, 0.25f);
		movingCharacter.GetPosition().RelenquishPosition(movingCharacter.GetGameObject());
		movingCharacter.SetPosition(step);
		movingCharacter.TakeStep();
		UpdateVisionIndicators();   //need to rework how this happens so it can do it based off of the destination space during the move
		await tween.AsyncWaitForCompletion();
		CheckVisionProfiles(movingCharacter);
		if (movingCharacter.GetType() == typeof(PlayableCharacter) && CheckObjectives()) {
			GameWin();
		}

		if(movingCharacter == ActivePlayer) {
			foreach(var container in containers) {
				container.HideInterface();
			}

			if (IsPositionNextToContainer(ActivePlayer.GetPosition(), out var adjacentContainers)) {
				foreach (var container in adjacentContainers) {
					container.ShowInterface(ActivePlayer);
				}
			}
		}

		foreach(var activatedEvent in stepActivatedEvents) {
			await activatedEvent.TryFireEvent(step);
		}

		Save();

	}
	#endregion

	public void CheckVisionProfiles(AbstractCharacter movedCharacter) {
		if(movedCharacter.GetType() == typeof(EnemyCharacter)) {
			CheckEnemyVisionProfile((movedCharacter as EnemyCharacter));
			return;
		}
		foreach(var enemy in enemies) {
			CheckEnemyVisionProfile(enemy);
		}
	}

	private void CheckVisiblity(VisionProfile profile) {
		if(profile == null) {
			Debug.Log("no profile");
			return;
		}
		foreach(KeyValuePair<GameObject, int> pair in profile.tileHits) {
			if (pair.Value < 2) {
				continue;
			}
			var space = map.GetSpaceFromObject(pair.Key);
			if(space == null) {
				Debug.Log("Object in vision profile does not belong to a tile");
				return;
			}
			space.ShowTileAndOccupants();
		}
		
	}

	public void UpdateVisionIndicators() {
		foreach(var mapSpace in map.spaces) {
			mapSpace.ClearVisionIndicator();
			mapSpace.HideTileAndOccupants();
		}
		foreach (var enemy in enemies) {
			if (enemy.isDead) {
				continue;
			}
			UpdateTileVisionIndicators(enemy.GetVisionProfile());
		}

		foreach (var player in party.members) {
			if (player.isDead) {
				continue;
			}
			CheckVisiblity(player.GetVisionProfile());
			player.GetPosition().ShowTileAndOccupants();
		}
	}

	public void UpdateTileVisionIndicators(VisionProfile profile) {
		foreach(var tileHit in profile.tileHits) {
			var mapSpace = map.GetSpaceFromObject(tileHit.Key);
			if(mapSpace == null) {
				continue;
			}
			mapSpace.SetThresholdIndicator(profile.GetThreshold(mapSpace), tileHit.Value);
		}
	}

	public void CheckEnemyVisionProfile(EnemyCharacter enemy) {
		if (enemy.isDead) {
			return;
		}
		var profile = enemy.GetVisionProfile();
		foreach(var player in party.members) {
			if (player.Invisible) {
				continue;
			}
			var threshold = profile.GetThreshold(player.GetPosition());
			switch (threshold) {
				case Threshold.Complete:
					HighAlert(player.GetPosition());
					enemy.SetChase(player);	//The person who spots the player will chase regardless of defense level
					break;
				case Threshold.Partial:
					enemy.SetChase(player);
					break;
			}
		}
	}


	public void HighAlert(MapSpace space) {
		float defenseLevel = defaultConfiguration.defendLevel;
		foreach (var enemy in enemies) {
			if (enemy.isDead) {
				continue;
			}
			float defenseRoll = Random.Range(0f, 1f);
			if (defenseRoll <= defenseLevel) {
				enemy.SetFallingBack(basePoint);
			}
			else {
				enemy.TrySetChase(space);
			}
		}
	}


	#region state management
	public async void EndTurn() {
		lastPlayer = ActivePlayer;
		SetActivePlayer(null);
		activeAction = null;
		foreach(var player in party.members) {
			if (player.isDead) {
				continue;
			}
			await player.TryFireTriggers(Trigger.TurnEnd);
		}
		UIController.Get().EnemyTurn();
		currentState = LevelState.EnemyTurn;
		ProcessEnemyTurns();
	}

	private async void ProcessEnemyTurns() {
		if (currentState == LevelState.EnemyTurn) {
			foreach (var enemy in enemies) {
				if (enemy.isDead) {
					continue;
				}
				await EnemyStateMachine.Get().ProcessEnemyTurn(enemy);
				await enemy.TryFireTriggers(Trigger.TurnEnd);
			}
		}
		NewTurn();
	}

	private async void NewTurn() {
		foreach(var character in party.members) {
			if (character.isDead) {
				continue;
			}
			await character.NewTurn();
		}
		currentState = LevelState.PlayerTurn;
		SetActivePlayer(lastPlayer);
		UIController.Get().PlayersTurn();
		turnNumber++;
		SaveRewind();
	}
	private void GameWin() {
		currentState = LevelState.Pause;
		outcome = LevelOutcome.Success;
		winText.text = "YOU WON!!!";
		winText.fontSize = 1;
		winText.gameObject.SetActive(true);
	}

	public void GameLoss() {
		currentState = LevelState.Pause;
		outcome = LevelOutcome.Failure;
		winText.text = "YOU LOST :(";
		winText.fontSize = 1;
		winText.gameObject.SetActive(true);
	}
	#endregion
}