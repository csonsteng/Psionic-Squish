using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class ColorManager : MonoBehaviour {

	private List<string> colors = new List<string>();
	private ColorSaveData colorSaveData = new ColorSaveData();

	public IEnumerable<string> Colors() {
		colors.Sort();
		return colors;
	}
	public static IEnumerable<string> GetColors() => Get().Colors();

	private void OnEnable() {
		Load();
		var panels = FindObjectsOfType<ColorPanel>();
		foreach (var panel in panels) {
			panel.GetNewColor();
		}
		UpdateColorChoices();
	}

	private void UpdateColorChoices() {
		var panels = FindObjectsOfType<ColorPanel>();
		foreach (var panel in panels) {
			panel.UpdateChoices();
		}
	}

	public void SaveColor(ColorData colorStruct) {
		colorSaveData.AddColor(colorStruct);
		Save();
	}

	public void NewColor(string newColorName) {
		colors.Add(newColorName);
		UpdateColorChoices();
	}

	public static ColorManager Get() {
		return FindObjectOfType<ColorManager>();
	}

	public void Save() {
		string path = Application.persistentDataPath + "/ColorData.json";
		string json = JsonUtility.ToJson(colorSaveData);
		File.WriteAllText(path, json);
		
		Debug.Log($"Saved with {colorSaveData.colors.Count} color datas.");
	}

	public void Load() {
		colors.Clear();

		string[] colorList = {
		"Red",
		"Blue",
		"Green",
		"Yellow",
		"Purple",
		"Teal",
		"Brown",
		"Orange",
		"Pink",
		"Black",
		"White",
		"Cream",
		"Grey",
		"Indigo",
		"Gold",
		"Biege",
		"Silver",
		};
		colors.AddRange(colorList);

		string path = Application.persistentDataPath + "/ColorData.json";
		if (!File.Exists(path)) {
			string json = File.ReadAllText(path);
			colorSaveData = JsonUtility.FromJson<ColorSaveData>(json);
			
			Debug.Log($"Loaded with {colorSaveData.colors.Count} color datas.");

			foreach(var color in colorSaveData.AllColors()) {
				if (!colors.Contains(color)) {
					colors.Add(color);
				}
			}

		}
	}
}

[System.Serializable]
public class ColorSaveData {
	[SerializeField]
	public List<ColorData> colors = new List<ColorData>();

	public void AddColor(ColorData color) {
		colors.Add(color);
	}

	public List<string> AllColors() {
		var colorList = new List<string>();
		foreach(var color in colors) {
			if (!colorList.Contains(color.color)) {
				colorList.Add(color.color);
			}
		}
		return colorList;
	}
}
