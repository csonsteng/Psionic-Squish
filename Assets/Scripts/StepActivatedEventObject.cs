using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class StepActivatedEventObject : MonoBehaviour
{

	private Animator animator;
	public bool setToDestroy = true;

	private void OnEnable() {
		animator = GetComponent<Animator>();
	}

	public async UniTask Activate() {
		animator.SetBool("Activated", true);
		var completionEvent = animator.GetBehaviour<FireCompletionEvent>();
		await UniTask.WaitUntil(completionEvent.AnimationFinished);
		if (setToDestroy) {
			Destroy(this);
		}
	}
}
