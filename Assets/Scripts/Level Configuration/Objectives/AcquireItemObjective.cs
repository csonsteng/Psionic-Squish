using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objectives/Acquire Item")]
public class AcquireItemObjective : AbstractLevelObjective {

	public CharacterActionData itemToAcquire;
	public override bool ObjectiveComplete() {
		var party = LevelController.Get().GetParty();
		foreach(var player in party.members) {
			if (player.inventory.TryGetAction(itemToAcquire, out _)){
				qualifyingPlayer = player;
				return true;
			}
		}
		return party.inventory.TryGetAction(itemToAcquire, out _);
	}

	public override string ObjectiveText() {
		return "Acquire " + itemToAcquire.displayName;
	}
}
