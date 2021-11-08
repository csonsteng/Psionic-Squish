using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Actions/Move")]
public class MoveAction : CharacterActionData {
	protected async override UniTask<bool> TakeAction(CharacterAction action) {
		await Controller.MoveCharacterToSpace(action.Owner, (MapSpace)action.Target);
		return true;
	}

	public override async UniTask<int> PreviewAction(CharacterAction action) {
		var path = Pathfinding.GetPath(action.Owner.GetPosition(), (MapSpace)action.Target, true);
		int maxPathSize = Mathf.Min(path.Count, action.Owner.ActionPoints());
		for (var i = 0; i < maxPathSize; i++) {
			path[i].HandleHover();
			//path[i].Indicator.SetStepCount(i + 1);
		}
		await UniTask.CompletedTask;
		return maxPathSize;
	}
}
