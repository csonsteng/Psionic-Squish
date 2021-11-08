using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/PassiveStatModifier")]
public class PassiveStatModifierAction : CharacterActionData {
	public int deltaActionPoints;

	public override bool RequiresTargets() => false;
	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		await Controller.ModifyAP(action.Owner, deltaActionPoints);
		return true;
	}

	protected override async UniTask<bool> UndoAction(CharacterAction action) {
		await Controller.ModifyAP(action.Owner, -deltaActionPoints);
		return true;
	}
}
