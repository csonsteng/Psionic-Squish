using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Structures {
    [CreateAssetMenu]
    public class LevelStructureData : ReferenceData {
        public int minRangeFromBase = 0;
        public int maxRangeFromBase = 10;
        public int minDistanceFromEdge = 3;
        public int minDistanceFromOtherStructures = 3;
        public AssetReferenceGameObject prefab;
        public Vector3 prefabOffset;
        //public int enemyCount;
        //public int maxEnemyDistance;
        public EnemyConfiguration enemyConfiguration;
        public List<EnemyCharacterData> specialEnemies = new List<EnemyCharacterData>();

        [HideInInspector]
        [SerializeField]
        public List<StructureSpace> spaces = new List<StructureSpace>();
        public int size = 0;

        public void AddStartingSpace() {
			var newSpace = new StructureSpace {
				column = 0,
				row = 0,
			};
			spaces.Add(newSpace);
            size++;
            Save();
        }

        public void AddPossibleEnemyPosition(StructureSpace space) {
            enemyConfiguration.AddPossibleLocation(space);
		}

        public void RemovePossibleEnemyPosition(StructureSpace space) {
            enemyConfiguration.RemovePossibleLocation(space);
		}

        public void Add() {
            AddRow();
            AddColumn();
            size++;
            Save();
        }

        public void Remove() {
            size--;
            RemoveRow();
            RemoveColumn();
            Save();
        }

        private void AddRow() {
            for (var i = 0; i < size; i++) {
				var newSpace = new StructureSpace {
					column = i,
					row = size,
				};
				spaces.Add(newSpace);
            }
        }

        private void AddColumn() {
            for (var i = 0; i <= size; i++) {
				var newSpace = new StructureSpace {
					column = size,
					row = i,
				};
				spaces.Add(newSpace);
            }
        }

        private void RemoveRow() {
            for (var i = 0; i <= size; i++) {
                var space = GetSpace(size, i);
                spaces.Remove(space);
            }
        }

        private void RemoveColumn() {
            for (var i = 0; i < size; i++) {
                var space = GetSpace(i, size);
                spaces.Remove(space);
            }
        }

        public DistanceCriteria[] GetCriteria(MapSpace basePoint) {
            DistanceCriteria criteriaMin = new DistanceCriteria(basePoint, minRangeFromBase, DistanceCriteria.Comparison.GreaterThanOrEqual);
            DistanceCriteria criteriaMax = new DistanceCriteria(basePoint, maxRangeFromBase, DistanceCriteria.Comparison.LessThanOrEqual);
            DistanceCriteria fromEdge = new DistanceCriteria(minDistanceFromEdge, DistanceCriteria.CheckType.MapEdge);
            DistanceCriteria fromOtherStructures = new DistanceCriteria(minDistanceFromOtherStructures, DistanceCriteria.CheckType.Structure);
            DistanceCriteria[] criteria = { criteriaMin, criteriaMax, fromEdge, fromOtherStructures };
            return criteria;
        }
        public List<StructureSpace> GetRotatedSpaces(float angle) {
            List<StructureSpace> rotatedList = new List<StructureSpace>();
            foreach (var space in spaces) {
                rotatedList.Add(space.Copy());
            }
            while (angle >= 90) {
                rotatedList = Rotate90(rotatedList);
                angle -= 90;

            }
            return rotatedList;
        }

        public List<StructureSpace> Rotate90(List<StructureSpace> rotateList) {
            for (int x = 0; x < size / 2; x++) {
                for (int y = x; y < size - x - 1; y++) {
                    var temp = GetSpace(rotateList, x, y);
                    GetSpace(rotateList, y, size - 1 - x).Reassign(x, y);
                    GetSpace(rotateList, size - 1 - x, size - 1 - y).Reassign(y, size - 1 - x);
                    GetSpace(rotateList, size - 1 - y, x).Reassign(size - 1 - x, size - 1 - y);
                    temp.Reassign(size - 1 - y, x);
                }
            }
            foreach (var space in rotateList) {
                space.Rotate();
            }
            return rotateList;
        }

        public StructureSpace GetSpace(List<StructureSpace> otherList, int row, int column) {
            foreach (var space in otherList) {
                if (space.column == column && space.row == row) {
                    return space;
                }
            }
            return null;
        }

        public StructureSpace GetSpace(int row, int column) {
            foreach (var space in spaces) {
                if (space.column == column && space.row == row) {
                    return space;
                }
            }
            return null;
        }

        private void Save() {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}
