using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class EnemyStateMachine: MonoBehaviour
{

	private delegate UniTask Callback(EnemyCharacter enemy);

	public static EnemyStateMachine Get() {
		return FindObjectOfType<EnemyStateMachine>();
	}
	private LevelController LevelController => LevelController.Get();
	public async UniTask ProcessEnemies(List<EnemyCharacter> enemies) {
		foreach(var enemy in enemies) {
			await ProcessEnemyTurn(enemy);
		}
	}
	public async UniTask ProcessEnemyTurn(EnemyCharacter enemy) {
		await enemy.NewTurn();
		if (enemy.target == null) {
			if (enemy.Status != EnemyStatus.Sleeping && enemy.Status != EnemyStatus.Stationary) {
				enemy.Status = EnemyStatus.Stationary;
			}
		}


		while (enemy.ActionPoints() > 0 && !LevelController.IsPaused && !enemy.isDead) {

			LevelController.movingCharacter = enemy;
			switch (enemy.Status) {
				case EnemyStatus.Patrol:
					await Patrol(enemy);

					break;
				case EnemyStatus.Chase:
					await Chase(enemy);
					break;
				case EnemyStatus.FallingBack:
					await FallBack(enemy);
					break;
				case EnemyStatus.Defend:
					await Defend(enemy);
					break;
				default:
					enemy.EndTurn();
					break;
			}
		}
		if(enemy.Status != EnemyStatus.Sleeping) {
			enemy.Idle();
		}

		if(enemy.Status == EnemyStatus.Chase) {

			enemy.Status = EnemyStatus.Patrol;
			enemy.ClearStatus();
			if (enemy.targetDirection != null && enemy.GetPosition() == enemy.target) {
				enemy.SetFacing(enemy.targetDirection);
			}
			LevelController.UpdateVisionIndicators();
			LevelController.CheckVisionProfiles(enemy);
		}


		if (enemy.patrolPath.Count < 2) {
			return;
		}

		if (enemy.GetPosition() == enemy.target) {
			if (enemy.patrolPath[0] == enemy.target) {
				enemy.target = enemy.patrolPath[1];
			}
			else {
				enemy.target = enemy.patrolPath[0];
			}

		}
	}



	public async UniTask ShowEnemyStatus(EnemyCharacter enemy) {
		enemy.ShowStatus();
		if (enemy.Status == EnemyStatus.Stationary || enemy.Status == EnemyStatus.Sleeping) {
			return;
		}
		if(enemy.target == null) {
			enemy.Status = EnemyStatus.Stationary;
		}
		enemy.ClearTileIndicators();
		var path = Pathfinding.GetPath(enemy.GetPosition(), enemy.target);
		int steps = Mathf.Min(enemy.Data.actionPoints,path.Count);
		var lastSpot = enemy.GetPosition();
		for (var i=0; i< steps; i++) {
			var step = path[i];
			var deltaStep = step.SubtractFrom(lastSpot);
			var facing = new Direction(deltaStep);
			step.SetStepIndicator(facing);
			enemy.tileIndicators.Add(step);
			lastSpot = step;
		}


		await UniTask.CompletedTask;
	}

	private async UniTask Patrol(EnemyCharacter enemy) {
		await MoveTowardsTarget(enemy, EndTurn);

	}

	private async UniTask MoveTowardsTarget(EnemyCharacter enemy, Callback pathCompletedCallback) {
		var path = Pathfinding.GetPath(enemy.GetPosition(), enemy.target);
		if (path.Count == 0 || !path[0].Passable) {
			//if we have no path left but we are in this position, we may be next to an player to attack with at least one step left
			var distanceToTarget = enemy.GetPosition().SubtractFrom(enemy.target);
			if(distanceToTarget.magnitude == 1 && LevelController.IsPositionAPlayer(enemy.target, out var player)) {
				//adjacent to a player
				await LevelController.Attack(player);
			}
			await pathCompletedCallback.Invoke(enemy);
			LevelController.Save();
			return;
		}
		await LevelController.TakeStep(path[0]);
	}


	private async UniTask Chase(EnemyCharacter enemy) {
		await MoveTowardsTarget(enemy, EndTurn);

	}

	private async UniTask FallBack(EnemyCharacter enemy) {
		await MoveTowardsTarget(enemy, ReachedDefensePoint);
	}

	private async UniTask Defend(EnemyCharacter enemy) {
		await MoveTowardsTarget(enemy, EndTurn);
	}

	private UniTask ReachedDefensePoint(EnemyCharacter enemy) {
		if (enemy.GetPosition() != enemy.target) {
			var defenseVector = enemy.GetPosition().SubtractFrom(enemy.target);
			var directionToDefendable = new Direction(defenseVector);
			var faceAway = directionToDefendable.Opposite();
			enemy.SetFacing(faceAway);
		}
		DistanceCriteria[] distanceCriteria = { new DistanceCriteria(enemy.GetPosition(), 3, DistanceCriteria.Comparison.LessThanOrEqual) };
		var patrolPoint = LevelController.map.GetRandomOpenSpaceWithDistanceCriteria(distanceCriteria);
		enemy.SetDefend(patrolPoint);
		return UniTask.CompletedTask;
	}

	private UniTask EndTurn(EnemyCharacter enemy) {
		enemy.EndTurn();
		return UniTask.CompletedTask;
	}
}
