using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbstractLevelObjective: ScriptableObject 
{
	public AbstractLevelObjective prerequisiteObjective;
	public PlayableCharacter qualifyingPlayer;
	public abstract string ObjectiveText();
	public abstract bool ObjectiveComplete();
}
