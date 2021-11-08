using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Configuration/Type")]
public class LevelType : ScriptableObject, IReferenced {
	public string GetUniqueID() {
		return uniqueID;
	}
	public string uniqueID;


}
