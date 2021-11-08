using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party
{
    public List<PlayableCharacter> members = new List<PlayableCharacter>();
	public Inventory inventory;

    public void Clear() {
		foreach (var member in members) {
			if (member.GetGameObject() != null) {
				GameObject.Destroy(member.GetGameObject());
			}
		}
		members.Clear();
	}

	public void Add(PlayableCharacter character) {
		if (members.Contains(character)) {
			return;
		}
		members.Add(character);
	}

	public void Add(PlayableCharacterData characterData) {
		Add(new PlayableCharacter(characterData));
	}

	public void AddPlayers(List<PlayableCharacterData> characters) {
		foreach(var character in characters) {
			Add(character);
		}
	}

	public bool AllDead() {
		foreach(var member in members) {
			if (!member.isDead) {
				return false;
			}
		}
		return true;
	}

	public Party() {
		inventory = new Inventory();
	}

	public Party(SerializableParty party, SerializableLevel level) {
		foreach(var member in party.members) {
			var character = new PlayableCharacter(member);
			character.SetPosition(level.GetSpace(member.position));
			members.Add(character);
		}
		inventory = new Inventory(party.inventory);
	}
}
