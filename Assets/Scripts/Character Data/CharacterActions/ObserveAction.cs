using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "Actions/Observe")]
public class ObserveAction : CharacterActionData {

	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		await Controller.ObserveEnemy((EnemyCharacter)(AbstractCharacter)action.Target);

		return true;
	}
}