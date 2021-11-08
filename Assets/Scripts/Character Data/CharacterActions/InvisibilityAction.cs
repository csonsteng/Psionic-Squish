using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Actions/Invisibility")]
public class InvisibilityAction : CharacterActionData {
	public int duration;

	public InvisibilityType invisibilityType = InvisibilityType.All;

	public override bool RequiresTargets() => false;
	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		await Controller.Invisibility((PlayableCharacter)action.Owner, duration, invisibilityType);
		return true;
	}
}

public enum InvisibilityType {
	PlayerTurn,
	EnemyTurn,
	All
}