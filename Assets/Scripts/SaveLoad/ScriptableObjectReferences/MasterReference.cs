using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "References/Master")]
public class MasterReference: ScriptableObject {
	public LevelTypeReferences levelTypes;
	public LevelEnvironmentReferences levelEnvironment;
	public PlayableCharacterReferences characters;
	public EnemyCharacterReferences enemies;
	public StructureReferences structures;
	public ActionReferences actions;
	public ItemReferences items;
}




