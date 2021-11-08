using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableParty 
{
	[SerializeField]
	public List<SerializablePlayer> members = new List<SerializablePlayer>();
	[SerializeField]
	public SerializableInventory inventory;

	public SerializableParty(Party party) {
		foreach(var player in party.members) {
			members.Add(new SerializablePlayer(player));
		}
		inventory = party.inventory.Serialize();
	}
}

[Serializable]
public class SerializablePlayer {

	[SerializeReference]
	public SerializeMapReference position;
	public Direction direction;
	public string data;
	public bool isActive;

	[SerializeField]
	public SerializableInventory inventory;

	[SerializeField]
	public List<SerializableAction> actions = new List<SerializableAction>();
	public int stepsRemaining;

	public int actionPointsRemaining;

	public int invisiblityDuration = 0;

	public SerializablePlayer(PlayableCharacter player) {
		position = player.GetPosition().Reference();
		direction = player.GetFacing();
		data = player.Data.uniqueID;
		isActive = player.isActive;

		actionPointsRemaining = player.ActionPoints();
		invisiblityDuration = player.invisiblityDuration;
		inventory = player.inventory.Serialize();
	}
}

[Serializable]
public class SerializableAction {
	public string actionData;
	public int turnsSinceUse;
	public SerializableAction(string actionData, int turnsSinceUse) {
		this.actionData = actionData;
		this.turnsSinceUse = turnsSinceUse;
	}
}
