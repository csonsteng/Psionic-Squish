using System;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceObjectList<T>: ScriptableObject where T: ReferenceData
{
	public List<T> items = new List<T>();

	public ReferenceData Get(string uniqueID) {
		foreach(var item in items) {
			if(item.GetUniqueID() == uniqueID) {
				return item;
			}
		}
		Debug.Log("returning null items");
		return null;
	}
}
