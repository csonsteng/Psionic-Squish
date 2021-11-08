using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInteractableContainer 
{
	public MapSpace position;
	public float rotation;
	public string assetReference;
	public bool Empty => contents.Count == 0 && pelletsInside == 0;
	public bool locked = false;
	public bool interacted = false;
	public int pelletsInside;
	public List<CharacterActionData> contents = new List<CharacterActionData>();
	public PlayableCharacter interactingPlayer;
	private GameObject gameObject;
	private ContainerInterface Interface => gameObject.GetComponentInChildren<ContainerInterface>();

	private readonly int[] itemCounts = {1, 1, 1, 1, 1, 1, 1, 2, 2};
	private readonly float pelletsRarity = 0.75f;
	

	public LevelInteractableContainer(string assetReference, MapSpace position, float rotation) {
		this.assetReference = assetReference;
		this.rotation = rotation;
		SetPosition(position);
		int randomCountIndex = Random.Range(0, itemCounts.Length);
		int count = itemCounts[randomCountIndex];
		List<CharacterActionData> actions = ResourceLoader.References.actions.items;
		List<CharacterActionData> rollTable = new List<CharacterActionData>();
		int totalWeight = 0;
		foreach(var action in actions) {
			if (!action.IsItem) {
				continue;
			}
			for(var i=0; i< action.rarity; i++) {
				rollTable.Add(action);
			}
			totalWeight += action.rarity;
		}
		for(var i=0; i<count; i++) {
			int pelletsRoll = Mathf.RoundToInt(pelletsRarity * rollTable.Count);
			int roll = Random.Range(0, rollTable.Count + pelletsRoll);
			if (roll >= rollTable.Count) {
				pelletsInside = 100;
			}
			else {
				contents.Add(rollTable[roll]);
			}
		}
	}

	public LevelInteractableContainer(string assetReference, AbstractCharacter character, float rotation) {
		var position = character.GetPosition();
		this.assetReference = assetReference;
		this.rotation = rotation;
		SetPosition(position);
		var inventory = character.inventory;
		foreach(var item in inventory.GetRemovableActions()) {
			contents.Add(item.Data);
		}
		pelletsInside = inventory.GetPelletCount();
	}

	public LevelInteractableContainer(SerializableContainer container) {
		assetReference = container.assetReference;
		rotation = container.rotation;
		locked = container.locked;
		interacted = container.interacted;
		pelletsInside = container.pelletsInside;
		foreach(var item in container.contents) {
			contents.Add(ResourceLoader.GetAction(item));
		}
	}

	public void SetPosition(MapSpace space) {
		position = space;
		space.PlaceContainer(this);
	}

	public GameObject GetGameObject() {
		return gameObject;
	}
	public void SetGameObject(GameObject gameObject) {
		this.gameObject = gameObject;
	}

	public void ShowInterface(PlayableCharacter interactingPlayer) {
		this.interactingPlayer = interactingPlayer;
		Interface.ShowInteract(() => {
			this.interacted = true;
			Interface.ShowContents(this);
		});
	}

	public void HideInterface() {
		Interface.HideInteract();
	}
}
