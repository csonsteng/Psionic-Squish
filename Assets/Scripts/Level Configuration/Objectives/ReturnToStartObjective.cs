using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objectives/Return To Start")]
public class ReturnToStartObjective : AbstractLevelObjective {


	private List<MapSpace> StartingSpaces() {
		return LevelController.Get().startingSpaces;
	}

	public override bool ObjectiveComplete() {
		if(prerequisiteObjective != null) {
			return CheckQualifyingPlayer();
		}
		var party = LevelController.Get().GetParty();
		foreach (var player in party.members) {
			if(StartingSpaces().Contains(player.GetPosition())) {
				return true;
			}
		}
		return false;
	}

	private bool CheckQualifyingPlayer() {
		if (prerequisiteObjective.ObjectiveComplete()) {
			var player = prerequisiteObjective.qualifyingPlayer;
			var qualifyingPosition = player.GetPosition();
			return StartingSpaces().Contains(qualifyingPosition);
		}
		return false;
	}

	public override string ObjectiveText() {
		if (prerequisiteObjective != null) {
			if (!prerequisiteObjective.ObjectiveComplete()) {
				return prerequisiteObjective.ObjectiveText();
			}
		}
		return "Return to start";
	}
}
