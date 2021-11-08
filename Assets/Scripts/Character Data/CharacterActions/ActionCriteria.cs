using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionCriteria 
{
	public Targets targets;
	public float range;
	public bool requireLineOfSight;
	public bool targetsLiving = true;
	public bool targetsDead = false;

	public enum Targets {
		Allies,
		Enemies,
		Characters,
		Spaces,
		Self
	}

	public Type GetTargetType() {
		if(targets == Targets.Spaces) {
			return typeof(MapSpace);
		}
		return typeof(AbstractCharacter);
	}

	private bool IsValidTarget(AbstractCharacter source, ITargetable target) {

		if (targets == Targets.Allies && target.GetType() != source.GetType()) {
			return false;
		}
		if (targets == Targets.Enemies && target.GetType() == source.GetType()) {
			return false;
		}
		if (target.GetType() == GetTargetType()) {
			if(target.GetType() == typeof(MapSpace)) {
				return true;
			}
			var isDead = (target as AbstractCharacter).isDead;
			if(isDead && targetsDead || !isDead && targetsLiving) {
				return true;
			}
			return false;
		}
		if(target.GetType() != typeof(MapSpace) && GetTargetType() != typeof(MapSpace)) {
			return true;
		}
		return false;
	}

	public bool Assess(CharacterAction action) {
		
		var target = action.Target;
		var source = action.Owner;
		if(!IsValidTarget(source, target)) {
			return false;
		}

		Vector2 distance = source.GetPosition().SubtractFrom(target.GetPosition());
		if(distance.magnitude > range && range != 0) {
			return false;
		}

		//TODO Line of Sight
		//set up line of sight on ICharacter

		if (requireLineOfSight) {
			var visionProfile = source.GetVisionProfile();
			if (!visionProfile.tileHits.ContainsKey(target.GetPosition().GetTileObject())) {
				return false;
			}
		}

		return true;
	}
}
