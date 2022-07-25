using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Configuration/Type")]
public class LevelTypeData : ReferenceData
{

}


[Serializable]
public class LevelType : ReferenceInstance<LevelTypeData>
{
	protected override LevelTypeData LoadReference() => ResourceLoader.GetLevelType(referenceID);
	public LevelType(LevelTypeData data) : base(data) { }
}
