using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public partial class LevelController
{

	public async UniTask Sleep(EnemyCharacter enemy) {
		enemy.Status = EnemyStatus.Sleeping;
		await UniTask.CompletedTask;
	}

	public async UniTask ModifyAP(AbstractCharacter character, int deltaAP) {
		character.ModifyAP(deltaAP);
		await UniTask.CompletedTask;
	}

	public async UniTask PlaceStepActivatedEvent(StepActivatedEvent activatedEvent, AssetReferenceGameObject gameObject) {

		if (gameObject == null) {
			stepActivatedEvents.Add(activatedEvent);
			return;
		}
		await LevelObjectSpawner.Get().SpawnStepActivatedEvent(activatedEvent, gameObject);
	}

	public async UniTask MoveCharacterToSpace(AbstractCharacter character, MapSpace space) {
		if (space == null) {
			return;
		}
		if (movingPlayers.Contains(character)) {
			return;
		}
		await MoveToDestination(ActivePlayer, space, true);
	}

	public async UniTask ObserveEnemy(EnemyCharacter enemy) {
		await EnemyStateMachine.Get().ShowEnemyStatus(enemy);
	}

	public async UniTask Peek(AbstractCharacter character) {
		character.UpdateVisionProfile(false);
		CheckVisiblity(character.GetVisionProfile());
		await UniTask.CompletedTask;
	}
	public async UniTask Defend(PlayableCharacter character) {
		character.defend = true;
		await UniTask.CompletedTask;
	}
	public async UniTask Attack(AbstractCharacter character) {
		await character.TryFireTriggers(Trigger.Attacked);
		if (character.GetType() == typeof(EnemyCharacter)) {
			HighAlert(character.GetPosition());
			if (!character.CanBeAttacked()) {
				return;
			}
		}
		if (!character.CanBeAttacked()) {
			return;
		}
		character.Die();
		if (character.GetType() == typeof(PlayableCharacter)) {
			if (party.AllDead()) {
				GameLoss();
			}
		}
		await DropItems(character);
		UpdateVisionIndicators();
	}

	public async UniTask DropItems(AbstractCharacter character) {
		var rotation = LevelGenerator.Get().GetRandomRotatation();
		var container = new LevelInteractableContainer(droppedItems.AssetGUID, character, rotation);
		
		await LevelObjectSpawner.Get().SpawnContainer(container, droppedItems);
	}

	public async UniTask Distract( MapSpace space, bool requiresVisual, int audioRange = 0) {
		if (!requiresVisual) {
			foreach (var enemy in enemies) {
				var distace = space.SubtractFrom(enemy.GetPosition());
				if(distace.magnitude <= audioRange) {
					enemy.SetChase(space);
				}
			}
			await UniTask.CompletedTask;
			return;
		}



		foreach (var enemy in enemies) {
			var profile = enemy.GetVisionProfile();
			var threshold = profile.GetThreshold(space);
			if(threshold == Threshold.Hidden) {
				continue;
			}
			enemy.SetChase(space);
		}
		await UniTask.CompletedTask;
	}

	public async UniTask Invisibility(PlayableCharacter player, int duration, InvisibilityType type) {
		player.SetInvisible(duration, type);
		await UniTask.CompletedTask;
	}
}
