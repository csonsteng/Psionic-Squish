using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class LevelConfiguration
{
    public int difficulty;
    public float defendLevel;

    public LevelType type;
    public LevelEnvironment environment;

    public float ObstacleDensity => environment.obstacleDensity;
    public float EnemyCount => environment.enemyCount;
    public float ObjectiveDistance => environment.maxObjectiveDistance;
    public float EnemyDistance => environment.minEnemyDistance;
    public int RowCount => environment.baseRowCount;
    public int ColumnCount => environment.baseColumnCount;
    public Material TileHoverMaterial => environment.tileHoverMaterial;

	public EnemyCharacter GetEnemy() => environment.GetEnemy();

    public AssetReferenceGameObject GetObstacle() => environment.GetObstacle();

    public AssetReferenceGameObject GetTile() => environment.GetTile();

    public AssetReferenceGameObject GetObjective() => environment.GetContainer();

    public DistanceCriteria[] GetBaseCriteria(MapSpace startingSpace) => environment.GetBaseCriteria(startingSpace);

	public SerializableConfiguration Serialize() {
        return new SerializableConfiguration(this);
	}

    public LevelConfiguration(SerializableConfiguration serialized) {
        type = ResourceLoader.GetLevelType(serialized.typeReference);
        environment = ResourceLoader.GetEnvironment(serialized.environmentReference);
        difficulty = serialized.difficulty;
        defendLevel = serialized.defendLevel;
	}
}
