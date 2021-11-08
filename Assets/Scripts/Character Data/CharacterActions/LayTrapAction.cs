using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Actions/LayTrap")]
public class LayTrapAction : CharacterActionData {
	public AssetReferenceGameObject gameObject;
	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		var activatedEvent = new StepActivatedEvent((MapSpace)action.Target, ActivatedEffect);
		await Controller.PlaceStepActivatedEvent(activatedEvent, gameObject);
		return true;
	}

	private async UniTask ActivatedEffect() {
		var activatingCharacter = Controller.movingCharacter;
		await Controller.Attack(activatingCharacter);
	}
}
