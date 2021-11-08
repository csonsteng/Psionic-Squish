using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class SerializableObstacle 
{
	[SerializeReference]
	public SerializeMapReference position;
	[SerializeField]
	public string obstacleGUID;
	public float rotation;
	public SerializableObstacle(MapSpace space, AssetReferenceGameObject gameObject, float rotation) {
		position = space.Reference();
		obstacleGUID = gameObject.AssetGUID;
		this.rotation = rotation;
	}
}
