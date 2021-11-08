using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/React")]
public class ReactAction : CharacterActionData {
	public Trigger trigger;
	public enum Trigger {
		EnemyAttack,
		ActionUsed,
	}
	protected override UniTask<bool> TakeAction(CharacterAction action) {
		throw new System.NotImplementedException();
	}
}
