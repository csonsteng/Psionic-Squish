using System;
using System.Collections.Generic;
using UnityEngine;

public class GenericScriptableObjectReference<T>: ScriptableObject 
{
	public List<T> items = new List<T>();

	public IReferenced Get(string uniqueID) {
		foreach(IReferenced item in items) {
			if(item.GetUniqueID() == uniqueID) {
				return item;
			}
		}
		Debug.Log("returning null items");
		return null;
	}
}
