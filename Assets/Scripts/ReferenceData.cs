using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReferenceData : ScriptableObject, IReferenced {
	public virtual string GetUniqueID() => uniqueID;

	public string uniqueID;
}
