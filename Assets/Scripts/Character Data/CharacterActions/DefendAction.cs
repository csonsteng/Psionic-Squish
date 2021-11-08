using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Actions/Defend")]
public class DefendAction : CharacterActionData {

	public override bool RequiresTargets() => false;
	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		await Controller.Defend((PlayableCharacter)action.Owner);
		return true;
	}
}
