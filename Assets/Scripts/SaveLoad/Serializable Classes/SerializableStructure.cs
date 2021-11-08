using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Structures;

[Serializable]
public class SerializableStructure
{
	public string structure;
	[SerializeField]
	public SerializeMapReference position;
	public float rotation;

	public SerializableStructure(LevelStructure structure, MapSpace space, float rotation) {
		this.structure = structure.data.GetUniqueID();// structure.Prefab.AssetGUID;
		position = space.Reference();
		this.rotation = rotation;
	}
}
