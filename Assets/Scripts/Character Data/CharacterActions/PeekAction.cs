using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName ="Actions/Peek")]
public class PeekAction : CharacterActionData {
	public override bool RequiresTargets() => false;
	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		await LevelController.Get().Peek(action.Owner);
		return true;
	}
}
