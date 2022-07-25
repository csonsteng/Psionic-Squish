using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Structures {
    [Serializable]
	public class LevelStructure: ReferenceInstance<LevelStructureData> {

        [SerializeReference] public MapSpace rootSpace;

        protected override LevelStructureData LoadReference() => ResourceLoader.GetStructure(referenceID);

        public AssetReferenceGameObject Prefab => data.prefab;
        public Vector3 PrefabOffset => data.prefabOffset;
        public float rotation;
        public int enemyCount;
        public int maxEnemyDistance;
        public EnemyConfiguration enemyConfiguration;

        [HideInInspector]
        [SerializeReference] public List<StructureSpace> spaces = new List<StructureSpace>();
        public int size = 0;
        public LevelStructure(LevelStructureData data, MapSpace mapSpace, float rotation): base(data) {
            this.rotation = rotation;
            size = data.size;
            rootSpace = mapSpace;
            foreach(var space in data.spaces) {
                spaces.Add(space.Copy());
			}
            enemyConfiguration = data.enemyConfiguration.Copy();
            enemyConfiguration.ReassignSpaces(spaces);
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