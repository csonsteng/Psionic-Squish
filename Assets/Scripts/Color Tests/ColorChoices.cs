using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorChoices : MonoBehaviour {
	public GameObject buttonTemplate;

	private List<GameObject> buttons = new List<GameObject>();
	public void UpdateChoices(ColorPanel panel) {
		ClearButtons();
		foreach(var colorChoice in ColorManager.GetColors()) {
			var buttonObject = Instantiate(buttonTemplate, transform);
			buttonObject.SetActive(true);
			var button = buttonObject.GetComponent<Button>();
			button.onClick.AddListener(() => {
				panel.NameColor(colorChoice);
			});
			var buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
			buttonText.text = colorChoice;
			buttons.Add(buttonObject);
		}
		var newColorObject = Instantiate(buttonTemplate, transform);
		newColorObject.SetActive(true);
		var newColorButton = newColorObject.GetComponent<Button>();
		newColorButton.onClick.AddListener(() => {
			ColorNamer.Get().Open(panel);
		});
		var newColorText = newColorObject.GetComponentInChildren<TextMeshProUGUI>();
		newColorText.text = "New Color";
		buttons.Add(newColorObject);

	}

	private void ClearButtons() {
		foreach(var button in buttons) {
			Destroy(button);
		}
		buttons.Clear();
	}
}
