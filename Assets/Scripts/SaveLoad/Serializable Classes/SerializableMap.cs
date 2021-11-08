using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SerializableMap 
{
	[SerializeField]
	public List<SerializableMapSpace> spaces = new List<SerializableMapSpace>();
	public int rowCount;
	public int columnCount;
}

[Serializable]
public class SerializableMapSpace {

	public string tileObjectGUID;

	public int row;
	public int column;
	public bool hasStructure = false;
	public bool hasObstacle = false;
	public bool hasContainer = false;
	public bool possibleEnemyLocation = false;
	public bool possibleObjectiveLocation = false;
	public bool occupied = false;
	public bool visible = true;

	public bool inOpenSpaces = false;
	public bool spotted;
	public Threshold threshold;
}


[Serializable]
public class SerializeMapReference {
	public int row;
	public int column;
}
