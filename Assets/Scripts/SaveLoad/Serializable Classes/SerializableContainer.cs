using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableContainer 
{
	public SerializeMapReference position;
	public string assetReference;
	public bool locked = false;
	public bool interacted = false;
	public float rotation;
	public int pelletsInside;
	public List<string> contents = new List<string>();

	public SerializableContainer(LevelInteractableContainer container) {
		position = container.position.Reference();
		assetReference = container.assetReference;
		locked = container.locked;
		interacted = container.interacted;
		pelletsInside = container.pelletsInside;
		rotation = container.rotation;

		foreach(var item in container.contents) {
			contents.Add(item.GetUniqueID());
		}
	}
}
