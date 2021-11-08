using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReferencableObject : ScriptableObject, IReferenced {
	public abstract string GetUniqueID();
}
