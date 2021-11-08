using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "Actions/Attack")]
public class AttackAction : CharacterActionData 
{
	public bool mustBeHidden = false;
	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		var ownerPosition = action.Owner.GetPosition();
		if(mustBeHidden && ownerPosition.Threshold != Threshold.Hidden) {
			return false;
		}

		await action.Owner.TryFireTriggers(Trigger.Attacking);
		await Controller.Attack((AbstractCharacter)action.Target);
		return true;
	}
}
