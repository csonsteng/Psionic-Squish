using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class SerializableEnemy 
{
	public string enemyData;

	[SerializeField]
	public SerializeMapReference position;
	public Direction direction;

	[SerializeField]
	public SerializeMapReference target;
	public Direction targetDirection;
	[SerializeField]
	public List<SerializeMapReference> patrolPath = new List<SerializeMapReference>();

	public EnemyStatus status;
	public bool statusShowing;

	public SerializableEnemy(EnemyCharacter enemy) {
		enemyData = enemy.Data.uniqueID;
		position = enemy.GetPosition().Reference();
		direction = enemy.GetFacing();
		if (enemy.target != null) {
			target = enemy.target.Reference();
		}
		foreach(var point in enemy.patrolPath) {
			patrolPath.Add(point.Reference());
		}

		status = enemy.Status;
		statusShowing = enemy.statusShowing;
	}
}
