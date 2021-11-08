using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Actions/Sleep")]
public class SleepAction : CharacterActionData {
	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		await Controller.Sleep((EnemyCharacter)action.Target);
		return true;
	}
}
