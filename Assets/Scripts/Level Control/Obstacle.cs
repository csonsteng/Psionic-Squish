using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class Obstacle
{
    [SerializeField] public readonly string GUID = string.Empty;
    [SerializeField] public readonly float rotation;

    public Obstacle(AssetReferenceGameObject obstacle, float rotation)
	{
        GUID = obstacle.AssetGUID;
        this.rotation = rotation;
	}
}

