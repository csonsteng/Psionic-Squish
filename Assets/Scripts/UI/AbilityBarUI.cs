using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBarUI : MonoBehaviour
{
	public GameObject squareSlotTemplate;
	public GameObject cappedSlotTemplate;
	public GameObject transitionSlotTemplate;
	public GameObject spacer;

	public RectTransform playerDisplay;

	private List<GameObject> slots = new List<GameObject>();

	public void ShowActions() {
		var character = LevelController.ActivePlayer;
		ClearButtons();
		if (character == null) {
			return;
		}

		float xOffset = playerDisplay.rect.width/2;
		var rectTransform = GetComponent<RectTransform>();
		float yOffset = rectTransform.rect.height / 2;
		rectTransform.offsetMin = new Vector2(xOffset, -yOffset);
		AddRemovableActions(character);
		AddFixedActions(character.colorScheme, character.inventory.GetFixedActions());


		float spacerWidth = spacer.GetComponent<RectTransform>().rect.width;
		int spacerCount = Mathf.RoundToInt(xOffset / spacerWidth);
		for (var i = 0; i < spacerCount; i++) {
			SpawnSlot(spacer, character.colorScheme, false);
		}
	}

	public void UpdateActionInteractability() {
		foreach(var slot in slots) {
			var slotComponent = slot.GetComponent<AbilitySlotUI>();
			slotComponent.UpdateActionInteractability();
		}
	}

	private void ClearButtons() {
		foreach(var slot in slots) {
			Destroy(slot);
		}
		slots.Clear();
	}

	private LevelController LevelController => LevelController.Get();

	public void AddRemovableActions(AbstractCharacter character) {
		var scheme = character.colorScheme;
		var inventory = character.inventory;
		var actions = inventory.GetRemovableActions();
		List<AbilitySlotUI> slots = new List<AbilitySlotUI>();
		for (var i = 0; i < inventory.itemSlots - 1; i++) {
			AbilitySlotUI slot;
			if (i == 0) {
				slot = SpawnCappedSlot(scheme, true);
			}
			else {
				slot = SpawnSquareSlot(scheme, true);
			}
			slots.Add(slot);
		}
		var transitionSlot = SpawnTransitionSlot(scheme, true);
		slots.Add(transitionSlot);

		var actionIndex = 0;
		for(var i = slots.Count-1; i>=0; i--) {
			if (actionIndex >= actions.Count) {
				slots[i].AddAction(null);
				continue;
			}
			slots[i].AddAction(actions[actionIndex]);
			actionIndex++;
		}

	}

	public void AddFixedActions(ColorScheme scheme, List<CharacterAction> actions) {

		List<AbilitySlotUI> slots = new List<AbilitySlotUI>();
		for (var i=0; i < actions.Count; i++) {
			if (i == 0) {
				slots.Add(SpawnCappedSlot(scheme));
				continue;
			}
			slots.Add(SpawnSquareSlot(scheme));
		}
		var actionIndex = 0;
		for (var i = slots.Count - 1; i >= 0; i--) {
			slots[i].AddAction(actions[actionIndex]);
			actionIndex++;
		}
	}

	private AbilitySlotUI SpawnTransitionSlot(ColorScheme scheme, bool reversed = false) {
		return SpawnSlot(transitionSlotTemplate, scheme, reversed);
	}

	private AbilitySlotUI SpawnSquareSlot(ColorScheme scheme, bool reversed = false) {
		return SpawnSlot(squareSlotTemplate, scheme, reversed);
	}
	private AbilitySlotUI SpawnCappedSlot(ColorScheme scheme, bool reversed = false) {
		return SpawnSlot(cappedSlotTemplate, scheme, reversed);
	}

	private AbilitySlotUI SpawnSlot(GameObject template, ColorScheme scheme, bool reversed) {
		var slotObject = Instantiate(template, transform);
		slots.Add(slotObject);
		var slot = slotObject.GetComponent<AbilitySlotUI>();
		slot.SetScheme(scheme, reversed);
		slotObject.SetActive(true);
		return slot;
	}


}
