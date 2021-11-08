using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class SerializableObjective
{
	public string objective;
	public bool isObjective;
	[SerializeField]
	public SerializeMapReference position;
	public float rotation;

	public SerializableObjective(AssetReferenceGameObject objective, MapSpace position, float rotation, bool isObjective) {
		this.objective = objective.AssetGUID;
		this.position = position.Reference();
		this.rotation = rotation;
		this.isObjective = isObjective;
	}
}
