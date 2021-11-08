using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRepresentation : MonoBehaviour
{
    public MeshRenderer mainRenderer;
    public MeshRenderer monoRenderer;

    public Material defaultMaterial;
	public Color Initialize(List<Color> existingColors) {
        var mainColor = ColorUtilities.RandomColor(existingColors);
		mainRenderer.material = new Material(defaultMaterial) {
			color = mainColor
		};
        monoRenderer.material = new Material(defaultMaterial) {
            color = ColorUtilities.FindMonochromatic(mainColor)
        };
        return mainColor;
    }
}
