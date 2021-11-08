using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtilities
{


	//green and blue cannot be close if they are under 0.35 or so
	//green and red cannot be close if they are over .65
	private static float greyThreshold = 0.4f;
	private static float bwThreshold = 0.5f;
	private static float lerpAmount = 0.4f;
	private static float closeToAmount = 0.05f;
	public static Color RandomColor(List<Color> existingColors = null) {
		if(existingColors == null) {
			existingColors = new List<Color>();
		}
		Color color = new Color(RandomValue(), RandomValue(), RandomValue());
		while (!ValidColor(color, existingColors)) {
			color = new Color(RandomValue(), RandomValue(), RandomValue());
		}
		return color;
	}

	public static Color RandomUnvalidatedColor() {
		return new Color(RandomValue(), RandomValue(), RandomValue());
	}

	public static Color FindMonochromatic(Color color, bool primarySide = true) {
		Color white = Color.white;
		Color black = Color.black;
		Color monochromatic;
		bool lerpToWhite = CloserToBlack(color);
		if (lerpToWhite && primarySide || !lerpToWhite && !primarySide) {
			monochromatic = Color.Lerp(color, white, lerpAmount);
		}
		else {
			monochromatic = Color.Lerp(color, black, lerpAmount);
		}
		
		return monochromatic;
	}

	private static float RandomValue() {
		float value = Random.Range(0f, 1f);
		return value;
	}

	public static bool CloserToBlack(Color color) {
		var totalValues = color.r + color.g + color.b;
		return totalValues <= 1.5f;
	}

	private static bool ValidColor(Color color, List<Color> existingColors) {
		var totalValues = color.r + color.g + color.b;
		if(totalValues <= bwThreshold || totalValues+bwThreshold >= 3) {
			return false;
		}
		var max = Mathf.Max(color.r, color.g, color.b);
		var min = Mathf.Min(color.r, color.g, color.b);
		if(max-min <= greyThreshold) {
			return false;
		}
		if (IsBrown(color)) {
			return false;
		}
		return !SimilarColorExists(color, existingColors);
	}

	private static bool IsBrown(Color color) {
		if(Mathf.Abs(color.g - color.b) <= greyThreshold) {
			if (color.g <= greyThreshold || color.b <= greyThreshold) {
				return true;
			}
		}
		
		if(Mathf.Abs(color.r - color.g) <= greyThreshold ) {
			if (color.r > 0.5 && color.r <= 0.85 || color.g > 0.5 && color.g <= 0.85) {
				return true;
			}
		}
		return false;
	}

	private static bool SimilarColorExists(Color color, List<Color> existingColors) {
		foreach(var otherColor in existingColors) {
			if(CloseTo(color, otherColor)) {
				return true;
			}
		}
		return false;
	}

	private static bool CloseTo(Color color1, Color color2) {
		if(Mathf.Abs(color1.r - color2.r) > closeToAmount) {
			return false;
		}
		if (Mathf.Abs(color1.g - color2.g) > closeToAmount) {
			return false;
		}
		if (Mathf.Abs(color1.b - color2.b) > closeToAmount) {
			return false;
		}
		return true;
	}
}
