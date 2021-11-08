using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGrouping : MonoBehaviour
{
	public GameObject colorObject;
	public float spacing = 1.05f;
	private string groupName;
	private float radius = 0;
	private int perimeterCount = 1;

	public float Radius => radius;
	public void NewColor(Color color) {
		var newObject = Instantiate(colorObject, transform);
		var renderer = newObject.GetComponent<MeshRenderer>();
		var material = new Material(renderer.material) {
			color = color
		};
		renderer.material = material;

		var angle = perimeterCount * PerimeterDivision();

		if (perimeterCount >= PerimeterDivision()) {
			perimeterCount = 1;
			radius += spacing;
		}
		else {
			perimeterCount++;
		}
		newObject.transform.position = new Vector3(radius * Mathf.Cos(angle), 0f, radius * Mathf.Sin(angle));
		newObject.SetActive(true);
	}

	private float Perimeter => 2 * Mathf.PI * radius;

	private float PerimeterDivision() {
		var divisions = Mathf.Floor(Perimeter/spacing);
		if(divisions == 0) {
			return 0f;
		}
		return 2*Mathf.PI / divisions;
	}



}
