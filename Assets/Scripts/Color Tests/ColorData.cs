using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorData
{
	public float r;
	public float g;
	public float b;
	public string color;

	public ColorData(Color color, string colorName) {
		r = color.r;
		g = color.g;
		b = color.b;
		this.color = colorName;
	}

	public Color Color() {
		return new Color(r, g, b);
	}

	public Vector3 AsVector3() {
		return new Vector3(r, g, b);
	}
}
