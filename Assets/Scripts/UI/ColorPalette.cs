using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ColorPalette : ScriptableObject
{
	public List<ColorGroup> neutral = new List<ColorGroup>();
	public List<ColorGroup> overlook = new List<ColorGroup>();
	public List<ColorGroup> displayOrder = new List<ColorGroup>();

	public List<ColorPairing> colors = new List<ColorPairing>();

	public void AddColor(Color color) {
		colors.Add(new ColorPairing(color));
	}

	public IEnumerable<Color> GetDisplayColors() {
		foreach(var group in displayOrder) {
			foreach(var pairing in GetPairingsInGroup(group)) {
				yield return pairing.color;
			}
		}
	}
	public IEnumerable<Color> GetVibrantColors() {
		foreach(var color in colors) {
			if(neutral.Contains(color.name) || overlook.Contains(color.name)) {
				continue;
			}
			yield return color.color;
		}
	}

	public IEnumerable<ColorPairing> GetPairingsInGroup(ColorGroup group) {
		foreach(var color in colors) {
			if(color.name == group) {
				yield return color;
			}
		}
	}

	public IEnumerable<Color> GetNeutralColors() {
		foreach (var color in colors) {
			if (neutral.Contains(color.name)) {
				yield return color.color;
			}
		}
	}

	[System.Serializable]
	public class ColorPairing {
		public Color color;
		public ColorGroup name;
		public ColorPairing(Color color) {
			this.color = color;
		}
	}

	public enum ColorGroup {
		Unassigned,
		Red,
		Yellow,
		Blue,
		Orange,
		Green,
		Violet,
		RedOrange,
		YellowOrange,
		YellowGreen,
		BlueGreen,
		BlueViolet,
		RedViolet,
		White,
		Black,
		Grey,
		Tan,
		Brown,
	}
}
