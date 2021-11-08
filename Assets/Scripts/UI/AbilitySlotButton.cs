using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class AbilitySlotButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler  {


	public GameObject tooltip;
	public TextMeshProUGUI title;
	public TextMeshProUGUI tooltipDescription;

	bool toolTipInitialized = false;
	private CharacterAction action;


	public Image icon;
	public TextMeshProUGUI label;

	private UnityAction clickEvent;


	public Button button;

	//private void Update() {
	//	var mouse = Mouse.current;
	//	if(mouse == null) {
	//		return;
	//	}
	//	var mousePosition = mouse.position.ReadValue();
	//	if(target.Raycast(mousePosition, Camera.current)) {
	//		OnPointerEnter();
	//	}
	//	else {
	//		OnPointerExit();
	//	}
	//}
	public void OnPointerEnter(PointerEventData eventData) {
		if (!toolTipInitialized) {
			return;
		}
		if (tooltip.activeSelf) {
			return;
		}
		LevelController.Get().UnHoverAll();
		tooltip.SetActive(true);
		action.SetPending();

	}

	public void OnPointerExit(PointerEventData eventData) {
		if (!toolTipInitialized) {
			return;
		}
		if (!tooltip.activeSelf) {
			return;
		}
		tooltip.SetActive(false);
		if (action != LevelController.Get().ActiveAction) {
			action.ClearPending();
		}
	}


	public void OnPointerClick(PointerEventData eventData) {
		clickEvent?.Invoke();
	}

	public void SetTooltipColor(Color color) {
		var image = tooltip.GetComponent<Image>();
		color.a = 0.5f;
		image.color = color;
	}

	public void UpdateInteractability() {
		bool interactable = action != null && action.IsAvailable && action != LevelController.Get().ActiveAction;
		Color color = Color.white;
		if (interactable) {
			button.interactable = true;
			clickEvent = () => {
				LevelController.Get().SetActiveAction(action);
			};
			color.a = 0.85f;
		}
		else {
			button.interactable = false;
			color.a = 0.35f;
		}
		icon.color = color;
	}

	public async void AddAction(CharacterAction action) {
		this.action = action;


		//bool interactable = action.IsAvailable() && action != LevelController.ActiveAction;
		//bool interactable = LevelController.ActiveAction != null && character.ActionPoints() > 1;


		UpdateInteractability();

		if(action == null) {
			return;
		}

		title.text = action.DisplayName;
		tooltipDescription.text = action.Description;
		toolTipInitialized = true;


		var sprite = await action.AwaitLoadSprite();
		if (sprite == null) {
			label.text = action.DisplayName;
			label.gameObject.SetActive(true);
			return;
		}
		icon.sprite = sprite;
		icon.gameObject.SetActive(true);

	}
}
