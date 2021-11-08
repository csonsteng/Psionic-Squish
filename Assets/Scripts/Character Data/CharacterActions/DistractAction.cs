using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Actions/Distract")]
public class DistractAction : CharacterActionData {
	public bool requireVisual = true;
	public int audioRange = 10;
	protected override async UniTask<bool> TakeAction(CharacterAction action) {
		await Controller.Distract(action.Target.GetPosition(), requireVisual, audioRange);
		return true;
	}
}
