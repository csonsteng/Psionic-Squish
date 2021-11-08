using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structures {
	[System.Serializable]
	public class StructureSpace {
		public bool excludeFromStructure;
		public bool blockedOnLeft;
		public bool blockedOnTop;
		public bool blockedOnRight;
		public bool blockedOnBottom;

		public int row;
		public int column;

		public bool isInterior = false;
		public bool enemyLocation = false;
		public bool objectiveLocation = false;

		public StructureSpace Copy() {
			var space = new StructureSpace {
				row = row,
				column = column,
				excludeFromStructure = excludeFromStructure,
				blockedOnBottom = blockedOnBottom,
				blockedOnLeft = blockedOnLeft,
				blockedOnRight = blockedOnRight,
				blockedOnTop = blockedOnTop,
				isInterior = isInterior,
				enemyLocation = enemyLocation,
				objectiveLocation = objectiveLocation
			};

			return space;
		}

		public void Reassign(int row, int column) {
			this.row = row;
			this.column = column;
		}

		public void Rotate() {
			var temp = blockedOnLeft;
			blockedOnLeft = blockedOnTop;
			blockedOnTop = blockedOnRight;
			blockedOnRight = blockedOnBottom;
			blockedOnBottom = temp;
		}
		public string Marks => GetMarksFromBools();

		public string GetMarksFromBools() {
			string marks = "";
			if (isInterior) {
				marks += "I";
			}
			if (enemyLocation) {
				marks += "E";
			}
			if (objectiveLocation) {
				marks += "O";
			}
			return marks;

		}

		public string Indicator => GetIndicatorFromBools();

		public string GetIndicatorFromBools() {
			if (excludeFromStructure) {
				return ".";
			}
			if (AllBlocked) {
				return "X";
			}
			if (NoneBlocked) {
				return "O";
			}
			if (OnlyLeftOpen) {
				return ">";
			}
			if (OnlyRightOpen) {
				return "<";
			}
			if (OnlyTopOpen) {
				return "U";
			}
			if (OnlyBottomOpen) {
				return "n";
			}
			if (OnlySidesOpen) {
				return "=";
			}
			string compoundString = "";
			if (blockedOnLeft) {
				compoundString += "[";
			}
			if (blockedOnBottom) {
				compoundString += "_";
			}
			if (blockedOnTop) {
				compoundString += "^";
			}
			if (blockedOnRight) {
				compoundString += "]";
			}
			return compoundString;
		}

		public void SetBoolsFromIndicators(string indicator) {
			excludeFromStructure = false;
			blockedOnLeft = false;
			blockedOnTop = false;
			blockedOnRight = false;
			blockedOnBottom = false;
			switch (indicator) {
				case ".":
					excludeFromStructure = true;
					break;
				case "X":
					blockedOnLeft = true;
					blockedOnTop = true;
					blockedOnRight = true;
					blockedOnBottom = true;
					break;
				case ">":
					blockedOnTop = true;
					blockedOnRight = true;
					blockedOnBottom = true;
					break;
				case "<":
					blockedOnLeft = true;
					blockedOnTop = true;
					blockedOnBottom = true;
					break;
				case "U":
					blockedOnLeft = true;
					blockedOnRight = true;
					blockedOnBottom = true;
					break;
				case "n":
					blockedOnLeft = true;
					blockedOnTop = true;
					blockedOnRight = true;
					break;
				case "=":
					blockedOnTop = true;
					blockedOnBottom = true;
					break;
				default:
					if (indicator.Contains("[")) blockedOnLeft = true;
					if (indicator.Contains("]")) blockedOnRight = true;
					if (indicator.Contains("_")) blockedOnBottom = true;
					if (indicator.Contains("^")) blockedOnTop = true;
					break;
			}
		}

		private bool AllBlocked => blockedOnLeft && blockedOnRight && blockedOnTop && blockedOnBottom;

		private bool NoneBlocked => !(blockedOnLeft || blockedOnRight || blockedOnTop || blockedOnBottom);

		private bool OnlyLeftOpen => !blockedOnLeft && blockedOnRight && blockedOnTop && blockedOnBottom;
		private bool OnlyRightOpen => blockedOnLeft && !blockedOnRight && blockedOnTop && blockedOnBottom;
		private bool OnlyTopOpen => blockedOnLeft && blockedOnRight && !blockedOnTop && blockedOnBottom;
		private bool OnlyBottomOpen => blockedOnLeft && blockedOnRight && blockedOnTop && !blockedOnBottom;
		private bool OnlySidesOpen => !blockedOnLeft && !blockedOnRight && blockedOnTop && blockedOnBottom;
	}
}