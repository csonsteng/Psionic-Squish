using System;
using System.Collections.Generic;
using UnityEngine;

namespace Structures {
    [Serializable]
    public class EnemyLocation  {
        public float spawnChance;
        public StructureSpace location;
        public List<EnemyStatus> possibleStatuses = new List<EnemyStatus>();

        public EnemyLocation(StructureSpace space) {
            location = space;
		}

        public EnemyLocation Copy() {
            var newLocation = new EnemyLocation(location) {
                spawnChance = spawnChance,
                possibleStatuses = possibleStatuses,
            };
            return newLocation;
		}

        public void ReassignSpace(List<StructureSpace> spaces) {
            foreach(var space in spaces) {
                if(space.row == location.row && space.column == location.column) {
                    location = space;
                    return;
				}
			}
		}
    }
}
