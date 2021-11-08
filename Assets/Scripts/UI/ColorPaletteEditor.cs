using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorPalette))]
public class ColorPaletteEditor : Editor {

	private bool editMode = true;
	public override void OnInspectorGUI() {
		editMode = EditorGUILayout.Toggle("Edit Mode", editMode);
		if (!editMode) {
			base.OnInspectorGUI();
			return;
		}

		var palette = (ColorPalette)serializedObject.targetObject;
		ColorPalette.ColorPairing removePairing = null;
		foreach(ColorPalette.ColorGroup group in Enum.GetValues(typeof(ColorPalette.ColorGroup))) {
			EditorGUILayout.LabelField(group.ToString(), EditorStyles.centeredGreyMiniLabel);
			foreach(var pairing in palette.GetPairingsInGroup(group)) {
				EditorGUILayout.BeginHorizontal();
				pairing.color = EditorGUILayout.ColorField(pairing.color);
				pairing.name = (ColorPalette.ColorGroup)EditorGUILayout.EnumPopup(pairing.name);
				if (GUILayout.Button("X")) {
					removePairing = pairing;
				}
				EditorGUILayout.EndHorizontal();
			}
			GUILayout.Space(10f);
		}

		if(removePairing != null) {
			palette.colors.Remove(removePairing);
			removePairing = null;
		}

		if (GUILayout.Button("Generate 10 Colors")) {
			for(var i = 0; i < 10; i++) {
				var color = ColorUtilities.RandomUnvalidatedColor();
				palette.AddColor(color);
			}
		}
	}
}
