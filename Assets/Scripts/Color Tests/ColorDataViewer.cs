using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ColorDataViewer : MonoBehaviour
{

	public Dictionary<string, GameObject> groupings = new Dictionary<string, GameObject>();
	public GameObject grouping;
	private void Start() {
		string path = Application.persistentDataPath + "/ColorData.json";
		if (!File.Exists(path)) {
			return;
		}
		string json = File.ReadAllText(path);
		ColorSaveData colorSaveData = JsonUtility.FromJson<ColorSaveData>(json);

		Debug.Log($"Loaded with {colorSaveData.colors.Count} color datas.");
		var colors = new List<string>();
		foreach(var color in colorSaveData.colors) {
			if (!groupings.ContainsKey(color.color)) {
				var newGrouping = Instantiate(grouping, transform);
				newGrouping.SetActive(true);
				groupings.Add(color.color, newGrouping);
				colors.Add(color.color);
			}
			var groupComponent = groupings[color.color].GetComponent<ColorGrouping>();
			groupComponent.NewColor(color.Color());
		}
		PositionGroupings();



		ColorPredicter.Initialize(colors);
		ColorPredicter.Teach(colorSaveData.colors);
	}

	private void PositionGroupings() {
		float maxRadius = 0f;
		foreach(var grouping in groupings.Values) {
			var groupRadius = grouping.GetComponent<ColorGrouping>().Radius;
			if(groupRadius > maxRadius) {
				maxRadius = groupRadius;
			}
		}
		var spacing = maxRadius;
		var radius = spacing;
		int perimeterCount = 1;

		foreach (var grouping in groupings.Values) {
			float perimeter = 2 * Mathf.PI * radius;

			float division = 0f;
			var divisions = Mathf.Floor(perimeter / spacing);
			if (divisions != 0) {
				division = 2 * Mathf.PI / divisions;
			}

			var angle = perimeterCount * division;

			if (perimeterCount >= division) {
				perimeterCount = 1;
				radius += spacing;
			}
			else {
				perimeterCount++;
			}


			grouping.transform.position = new Vector3(radius * Mathf.Cos(angle), 0f, radius * Mathf.Sin(angle));
		}

	}
}
