using System.Collections.Generic;
using UnityEngine;

namespace Structures {
    [System.Serializable]
    public class EnemyConfiguration  {
        public int minCount=2;
        public int maxCount=2;
        public int maxRange = 8;


        public List<EnemyCharacterData> requiredEnemies = new List<EnemyCharacterData>();
        public List<EnemyCharacterData> enemyPool = new List<EnemyCharacterData>();
        [SerializeField]
        public List<EnemyLocation> possibleLocations = new List<EnemyLocation>();
        public List<EnemyStatus> possibleStatuses = new List<EnemyStatus>();

        private int enemyCount = 0;
        public EnemyConfiguration Copy() {
            var configuration = new EnemyConfiguration() {
                minCount = minCount,
                maxCount = maxCount,
                maxRange = maxRange,
                requiredEnemies = requiredEnemies,
                enemyPool = enemyPool,
                possibleStatuses = possibleStatuses
            };
            foreach(var location in possibleLocations) {
                configuration.possibleLocations.Add(location.Copy());
			}

            return configuration;
		}

        public void AddPossibleLocation(StructureSpace space) {
            var location = new EnemyLocation(space);
            possibleLocations.Add(location);
		}

        public void RemovePossibleLocation(StructureSpace space) {
            EnemyLocation removeLocation = null;
            foreach(var location in possibleLocations) {
                if(location.location == space) {
                    removeLocation = location;
				}
			}
            if(removeLocation == null) {
                return;
			}
            possibleLocations.Remove(removeLocation);
		}

        public void ReassignSpaces(List<StructureSpace> spaces) {
            foreach(var location in possibleLocations) {
                location.ReassignSpace(spaces);
			}
		}

        public IEnumerable<EnemyCharacter> GetEnemies(LevelMap map, MapSpace structSpace) {
            if (enemyCount == 0) {
                enemyCount = Random.Range(minCount, maxCount + 1);
            }

            for(var i=0; i< enemyCount; i++) {
                var enemy = new EnemyCharacter(ChooseEnemy());
                var location = ChooseLocation();
                MapSpace enemySpot;
                if (location == null) {
                    DistanceCriteria[] criteria = { new DistanceCriteria(structSpace, maxRange, DistanceCriteria.Comparison.LessThanOrEqual) };
                    enemySpot = map.GetRandomOpenSpaceWithDistanceCriteria(criteria);
                    SetEnemyStatus(enemy, possibleStatuses);

                }
				else {
                     enemySpot = map.GetSpaceWithStructure(location.location);
                    SetEnemyStatus(enemy, location.possibleStatuses);
                }
                if(enemySpot == null) {
                    yield return null;
				}
                enemy.SetPosition(enemySpot);
                yield return enemy;

            }
		}

        private void SetEnemyStatus(EnemyCharacter enemy, List<EnemyStatus> statuses) {
            int randomStatus = Random.Range(0, statuses.Count);
            enemy.Status = statuses[randomStatus];
        }


        private EnemyCharacterData ChooseEnemy() {
            if(requiredEnemies.Count > 0) {
                var enemy = requiredEnemies[0];
                requiredEnemies.RemoveAt(0);
                return enemy;
			}
            int randomEnemy = Random.Range(0, enemyPool.Count);
            return enemyPool[randomEnemy];
		}

        private EnemyLocation ChooseLocation() {
            float locationWeight = 0;
            foreach(var location in possibleLocations) {
                locationWeight += location.spawnChance;
			}
            float roll = Random.Range(0, Mathf.Max(locationWeight, 1));
            foreach(var location in possibleLocations) {
                roll -= location.spawnChance;
                if(roll <= 0) {
                    return location;
				}
			}
            return null;

		}
    }
}