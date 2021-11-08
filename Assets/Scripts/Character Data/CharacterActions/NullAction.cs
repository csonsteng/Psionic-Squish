using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Null")]
public class NullAction : CharacterActionData {
	public override bool IsAvailable(CharacterAction action) {
		return false;
	}

	public override bool ValidTarget(CharacterAction action) {
		return false;
	}
	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		await UniTask.CompletedTask;
		return false;
	}
}
