using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ContainerContentsDisplay : MonoBehaviour
{
	public ContainerContentsSlot slotTemplate;
	public int slotCount = 4;

	private List<GameObject> buttons = new List<GameObject>();

	public void ShowContents(LevelInteractableContainer container) {
		UIController.Get().EnableClickCanceller(() => {
			Close();
			container.ShowInterface(container.interactingPlayer);
		});
		GenerateButtons(container);
	}

	private void GenerateButtons(LevelInteractableContainer container) {
		ClearButtons();

		bool hasPellets = container.pelletsInside > 0;
		var slotIndex = 0;
		for (var i = 1; i<= slotCount; i++) {
			var slotButton = Instantiate(slotTemplate.gameObject, transform);
			slotButton.SetActive(true);
			buttons.Add(slotButton);
			var slot = slotButton.GetComponent<ContainerContentsSlot>();
			UnityAction action;
			if (hasPellets) {
				action = () => {
					TakePellets(container);
				};

				slot.SetButton(action, container.pelletsInside + " Pellets");
				hasPellets = false;
				continue;
			}
			
			if (slotIndex < container.contents.Count) {
				var index = slotIndex;
				action = () => {
					
					TakeItem(container, index);
				};
				slot.SetButton(action, container.contents[slotIndex].displayName);
				slotIndex++;
				continue;
			}

			slot.SetButton(null, "");
		}
	}

	private void Close() {
		gameObject.SetActive(false);
	}

	private void ClearButtons() {
		foreach (var button in buttons) {
			Destroy(button);
		}
		buttons.Clear();
	}

	private void TakePellets(LevelInteractableContainer container) {
		int pelletsTaken = container.pelletsInside;
		container.pelletsInside = 0;
		LevelController.Get().GetParty().inventory.AddPellets(pelletsTaken);
		GenerateButtons(container);
	}

	private void TakeItem(LevelInteractableContainer container, int index) {
		var item = container.contents[index];
		container.interactingPlayer.AddAction(new CharacterAction(item, container.interactingPlayer));
		container.contents.Remove(item);
		GenerateButtons(container);
	}



}
