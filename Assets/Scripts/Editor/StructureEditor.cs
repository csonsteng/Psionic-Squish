using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Structures {

	[CustomEditor(typeof(LevelStructureData))]
	public class StructureEditor : Editor {
		StructureSpace activeSpace;
		LevelStructureData structure;
		bool openShape = true;
		bool openMarks = false;
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			structure = (LevelStructureData)serializedObject.targetObject;
			//structure.spaces.Clear();
			if (structure.spaces.Count == 0) {
				if (GUILayout.Button("Initialize")) {
					structure.AddStartingSpace();
				}
				return;
			}

			int size = structure.size;

			openShape = EditorGUILayout.BeginFoldoutHeaderGroup(openShape, "Shape Design");
			if (openShape) {
				for (var i = 0; i < size; i++) {
					EditorGUILayout.BeginHorizontal();
					for (var j = 0; j < size; j++) {
						var space = GetSpace(i, j);
						DrawShapeMenu(space);
					}
					EditorGUILayout.EndHorizontal();
				}


				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Add")) {
					structure.Add();
				}
				if (GUILayout.Button("Remove")) {
					structure.Remove();
				}
				EditorGUILayout.EndHorizontal();

				if (GUILayout.Button("Rotate")) {
					structure.Rotate90(structure.spaces);
				}
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
			openMarks = EditorGUILayout.BeginFoldoutHeaderGroup(openMarks, "Assign Marks");
			if (openMarks) {
				for (var i = 0; i < size; i++) {
					EditorGUILayout.BeginHorizontal();
					for (var j = 0; j < size; j++) {
						var space = GetSpace(i, j);
						DrawMarkMenu(space);
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

		}

		void DrawShapeMenu(StructureSpace space) {
			if (space == null) {
				return;
			}
			if (space.row == 0 && space.column == 0) {
				GUI.color = Color.green;
			}
			if (GUILayout.Button(space.Indicator, GUILayout.Width(25), GUILayout.Height(25))) {
				activeSpace = space;
				GenericMenu menu = new GenericMenu();
				NewShapeMenuItem(menu, "O");
				NewShapeMenuItem(menu, "X");
				NewShapeMenuItem(menu, "[");
				NewShapeMenuItem(menu, "]");
				NewShapeMenuItem(menu, "^");
				NewShapeMenuItem(menu, "_");
				NewShapeMenuItem(menu, "[^");
				NewShapeMenuItem(menu, "[_");
				NewShapeMenuItem(menu, "[]");
				NewShapeMenuItem(menu, "^]");
				NewShapeMenuItem(menu, "_]");
				NewShapeMenuItem(menu, "=");
				NewShapeMenuItem(menu, "U");
				NewShapeMenuItem(menu, "n");
				NewShapeMenuItem(menu, "<");
				NewShapeMenuItem(menu, ">");
				menu.ShowAsContext();
			}

			GUI.color = Color.white;
		}

		void DrawMarkMenu(StructureSpace space) {
			if (space == null) {
				return;
			}
			if (space.row == 0 && space.column == 0) {
				GUI.color = Color.green;
			}
			if (GUILayout.Button(space.Marks, GUILayout.Width(25), GUILayout.Height(25))) {
				activeSpace = space;
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Interior"), activeSpace.isInterior, ToggleInterior);
				menu.AddItem(new GUIContent("Enemy"), activeSpace.enemyLocation, ToggleEnemy);
				menu.AddItem(new GUIContent("Objective"), activeSpace.objectiveLocation, ToggleObjective);
				menu.ShowAsContext();
			}

			GUI.color = Color.white;
		}

		void NewShapeMenuItem(GenericMenu menu, string item) {
			menu.AddItem(new GUIContent(item), activeSpace.Indicator == item, SetShape, item);
		}

		void SetShape(object newLabel) {
			activeSpace.SetBoolsFromIndicators((string)newLabel);
			EditorUtility.SetDirty(structure);
		}

		void ToggleInterior() {
			activeSpace.isInterior = !activeSpace.isInterior;
			EditorUtility.SetDirty(structure);
		}

		void ToggleEnemy() {
			activeSpace.enemyLocation = !activeSpace.enemyLocation;
			if (activeSpace.enemyLocation) {
				structure.AddPossibleEnemyPosition(activeSpace);
			}
			else {
				structure.RemovePossibleEnemyPosition(activeSpace);
			}
			EditorUtility.SetDirty(structure);
		}

		void ToggleObjective() {
			activeSpace.objectiveLocation = !activeSpace.objectiveLocation;
			EditorUtility.SetDirty(structure);
		}

		StructureSpace GetSpace(int row, int column) {
			return structure.GetSpace(row, column);
			//var spacesProperty = serializedObject.FindProperty("spaces");
			//for (var i = 0; i < spacesProperty.arraySize; i++) {
			//	StructureSpace space = (StructureSpace)spacesProperty.GetArrayElementAtIndex(i).objectReferenceValue;
			//	if (space != null) {
			//		if (space.column == column && space.row == row) {
			//			return space;
			//		}
			//	}
			//}
			//return null;
		}
	}
}