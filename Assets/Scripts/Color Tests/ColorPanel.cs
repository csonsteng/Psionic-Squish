using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ColorPanel : MonoBehaviour
{
	public Color color;
	public Image colorDisplay;
	public ColorChoices choices;

	public void GetNewColor() {
		color = ColorUtilities.RandomUnvalidatedColor();
		colorDisplay.color = color;
	}

	public void UpdateChoices() {
		choices.UpdateChoices(this);
	}

	public void NameColor(string colorName) {
		ColorData colorData = new ColorData(color, colorName);
		ColorManager.Get().SaveColor(colorData);

		GetNewColor();
	}

	public void NewColor(string colorName) {
		ColorManager.Get().NewColor(colorName);
		NameColor(colorName);
	}

}
