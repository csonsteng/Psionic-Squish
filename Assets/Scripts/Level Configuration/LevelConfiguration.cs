using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class LevelConfiguration: ISerializationCallbackReceiver
{
    public int difficulty;
    public float defendLevel;


    [NonSerialized] public LevelTypeData type;
    [NonSerialized] public LevelEnvironmentData environment;

    private string typeID;
    private string environmentID;

    public LevelConfiguration(LevelTypeData type, LevelEnvironmentData environment, int difficulty, float defendLevel)
	{
        this.type = type;
        this.environment = environment;
        this.difficulty = difficulty;
        this.defendLevel = defendLevel;
	}
    

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

	public void OnBeforeSerialize()
    { 
        if(type != null)
            typeID = type.uniqueID;
        if(environment != null)
            environmentID = environment.uniqueID;
	}

	public void OnAfterDeserialize()
	{
        if (environmentID.IsNotNullOrEmpty())
            environment = ResourceLoader.GetEnvironment(environmentID);
        if (typeID.IsNotNullOrEmpty())
            type = ResourceLoader.GetLevelType(typeID);


	}
}
