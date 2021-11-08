using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class PlayableCharacter : AbstractCharacter {

	//[SerializeField]
	//public List<CharacterAction> actions = new List<CharacterAction>();

	public int invisiblityDuration = 0;
	public InvisibilityType invisibilityType;

	public bool defend = false;
	public PlayableCharacter(PlayableCharacterData data, ColorScheme scheme=null): base(data, scheme) {
		//foreach (var action in data.characterClass.startingActions) {
		//	actions.Add(new CharacterAction(action, this));
		//}
	}

	public PlayableCharacter(SerializablePlayer player): base(ResourceLoader.GetCharacter(player.data)) {
		direction = player.direction;
		actionPointsRemaining = player.actionPointsRemaining;
		invisiblityDuration = player.invisiblityDuration;
		isActive = player.isActive;
		//foreach (var action in player.actions) {
		//	actions.Add(new CharacterAction(action, this));
		//}
		inventory = new Inventory(player.inventory, this);
	}

	#region turn management
	public override void StepActions() {
		UIController.Get().UpdateActionInteractability();
	}


	public override bool CanBeAttacked() {
		if (defend) {
			defend = false;
			return false;
		}
		return true;
	}


	public override async UniTask NewTurn() {
		await base.NewTurn();
		defend = false;
		RefreshActionPoints();
		inventory.ProcessCooldowns();
		if (Invisible) {
			invisiblityDuration--;
		}
	}

	public void SetInvisible(int duration, InvisibilityType type) {
		invisiblityDuration = duration;
		invisibilityType = type;
	}


	public bool Invisible => IsInvisible();
	public bool IsInvisible() {
		if(invisiblityDuration <= 0) {
			return false;
		}
		var state = LevelController.Get().currentState;
		switch (invisibilityType) {
			case InvisibilityType.EnemyTurn:
				return state == LevelController.LevelState.EnemyTurn;
			case InvisibilityType.PlayerTurn:
				return state == LevelController.LevelState.PlayerTurn; 
		}
		return true;

	}

	#endregion

	#region interfaces
	public override int GetLayer() {
		return LayerMask.NameToLayer("Player");
	}
	#endregion

	public override void AddAction(CharacterAction action) {
		inventory.TryAddAction(action);
		//actions.Add(action);
		if (LevelController.Get().currentState == LevelController.LevelState.PlayerTurn) {
			UIController.Get().PlayersTurn();
		}
	}

	public override async UniTask<bool> RemoveAction(CharacterAction action, bool canRemoveFixed = false) {
		if (!action.IsItem && !canRemoveFixed) {
			return false;
		}
		bool hadItem = inventory.RemoveAction(action.Data);
		//for(var i = actions.Count - 1; i>=0; i--) {
		//	if(actions[i].Data == action.Data) {
		//		await actions[i].OnBeforeRemoval();
		//		actions.RemoveAt(i);
		//		UIController.Get().SetActionPoints();
		//		break;
		//	}
		//}
		await UniTask.CompletedTask;
		return hadItem;
	}

	public void GivePellets(int count) {
		inventory.AddPellets(count);
	}

	public bool RemovePellets(int count) {
		return inventory.RemovePellets(count);
	}
}
