using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public GameObject tooltip;
	public TextMeshProUGUI actionPoints;
	public TextMeshProUGUI tooltipDescription;
	public Button button;
	public void OnPointerEnter(PointerEventData eventData) {
		tooltip.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData) {
		tooltip.SetActive(false);
	}

	public void ConfigureButton(string label, int pointsCost, string description, bool available, UnityAction action) {
		if (available) {
			button.interactable = true;
			button.onClick.AddListener(() => {
				action?.Invoke();
			});
		}
		else {
			button.interactable = false;
		}
		button.GetComponentInChildren<TextMeshProUGUI>().text = label;
		actionPoints.text = "Action Points: " + pointsCost.ToString();
		tooltipDescription.text = description;
		tooltip.SetActive(false);
	}
}
