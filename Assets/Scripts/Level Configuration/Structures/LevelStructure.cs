using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Structures {
	public class LevelStructure {
        [SerializeReference]
        public LevelStructureData data;

        public AssetReferenceGameObject Prefab => data.prefab;
        public Vector3 PrefabOffset => data.prefabOffset;
        public int enemyCount;
        public int maxEnemyDistance;
        public EnemyConfiguration enemyConfiguration;
        //public List<EnemyCharacterData> specialEnemies = new List<EnemyCharacterData>();

        [HideInInspector]
        [SerializeField]
        public List<StructureSpace> spaces = new List<StructureSpace>();
        public int size = 0;
        public LevelStructure(LevelStructureData data) {
            this.data = data;
            size = data.size;
            foreach(var space in data.spaces) {
                spaces.Add(space.Copy());
			}
            enemyConfiguration = data.enemyConfiguration.Copy();
            enemyConfiguration.ReassignSpaces(spaces);
		}


        public DistanceCriteria[] GetCriteria(MapSpace basePoint) {
            DistanceCriteria criteriaMin = new DistanceCriteria(basePoint, data.minRangeFromBase, DistanceCriteria.Comparison.GreaterThanOrEqual);
            DistanceCriteria criteriaMax = new DistanceCriteria(basePoint, data.maxRangeFromBase, DistanceCriteria.Comparison.LessThanOrEqual);
            DistanceCriteria fromEdge = new DistanceCriteria(data.minDistanceFromEdge, DistanceCriteria.CheckType.MapEdge);
            DistanceCriteria fromOtherStructures = new DistanceCriteria(data.minDistanceFromOtherStructures, DistanceCriteria.CheckType.Structure);
            DistanceCriteria[] criteria = { criteriaMin, criteriaMax, fromEdge, fromOtherStructures };
            return criteria;
        }
        public List<StructureSpace> GetRotatedSpaces(float angle) {
            while (angle >= 90) {
                spaces = Rotate90();
                angle -= 90;

            }
            return spaces;
        }

        public List<StructureSpace> Rotate90() {
            for (int x = 0; x < size / 2; x++) {
                for (int y = x; y < size - x - 1; y++) {
                    var temp = GetSpace(x, y);
                    GetSpace(y, size - 1 - x).Reassign(x, y);
                    GetSpace(size - 1 - x, size - 1 - y).Reassign(y, size - 1 - x);
                    GetSpace(size - 1 - y, x).Reassign(size - 1 - x, size - 1 - y);
                    temp.Reassign(size - 1 - y, x);
                }
            }
            foreach (var space in spaces) {
                space.Rotate();
            }
            return spaces;
        }

        public StructureSpace GetSpace(int row, int column) {
            foreach (var space in spaces) {
                if (space.column == column && space.row == row) {
                    return space;
                }
            }
            return null;
        }
    }
}