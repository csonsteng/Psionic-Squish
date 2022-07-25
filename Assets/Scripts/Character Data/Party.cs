using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Party
{
    [SerializeReference] public List<PlayableCharacter> members = new List<PlayableCharacter>();
	public Inventory inventory;

    public void Clear() {
		foreach (var member in members) 
			if (member.GameObject != null) 
				member.GameObject.Destroy();

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

}
