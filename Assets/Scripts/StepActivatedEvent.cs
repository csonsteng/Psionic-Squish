using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class StepActivatedEvent 
{
	public MapSpace activationLocation;
	private TriggerAction activatedEvent;
	private StepActivatedEventObject activatedObject;
	private FireTiming fireTiming;

	public StepActivatedEvent(MapSpace location, TriggerAction activatedEvent) {
		activationLocation = location;
		this.activatedEvent = activatedEvent;
	}

	public async UniTask TryFireEvent(MapSpace space) {
		if(space != activationLocation) {
			return;
		}

		switch (fireTiming) {
			case FireTiming.BeforeEvent:
				await TryActivateObject();
				await activatedEvent.Invoke();
				break;
			case FireTiming.WithEvent:
				List<UniTask> activations = new List<UniTask> {
					TryActivateObject(),
					activatedEvent.Invoke()
				};
				await UniTask.WhenAll(activations);
				break;
			case FireTiming.AfterEvent:
				await activatedEvent.Invoke();
				await TryActivateObject();
				break;
		}
	}

	private async UniTask TryActivateObject() {
		if (activatedObject != null) {
			await activatedObject.Activate();
		}
	}

	public bool TrySetActivatedObject(GameObject gameObject, FireTiming fireTiming = FireTiming.WithEvent) {
		activatedObject = gameObject.GetComponent<StepActivatedEventObject>();
		this.fireTiming = fireTiming;
		return activatedObject != null;
	}

	public enum FireTiming {
		BeforeEvent,
		WithEvent,
		AfterEvent,
	}
}
