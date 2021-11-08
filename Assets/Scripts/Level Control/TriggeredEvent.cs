using Cysharp.Threading.Tasks;
public class TriggeredEvent {


	public Trigger trigger;
	public TriggerAction action;
	public string id;

	public TriggeredEvent(Trigger trigger, TriggerAction action, string triggerID) {
		this.action = action;
		this.trigger = trigger;
		id = triggerID;
	}

	public async UniTask TryFireTrigger(Trigger trigger) {
		if(trigger == this.trigger && trigger != Trigger.None) {
			await action.Invoke();
		}
	}
}

public enum Trigger
{
	None,
	LevelStart,
	TurnStart,
	Attacked,
	Attacking,
	TurnEnd,
	Permanent,
}


public delegate UniTask TriggerAction();
