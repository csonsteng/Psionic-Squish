using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class Obstacle
{
    public string GUID = string.Empty;
    public float rotation;

    public Obstacle(AssetReferenceGameObject obstacle, float rotation)
	{
        GUID = obstacle.AssetGUID;
        this.rotation = rotation;
	}
}

