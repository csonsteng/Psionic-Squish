using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class CharacterAction {
	public CharacterActionData Data { get; }
	public AbstractCharacter Owner { get; }
	public string DisplayName => Data.displayName;
	public string Description => Data.description;

	public bool IsDefault => Data.defaultAction;

	public bool RequiresTargets => Data.RequiresTargets();
	public bool IsItem => Data.IsItem;

	public bool IsActive {get; set;}
	public ITargetable Target {
		get {
			return target;
		}
		set {
			target = value;
		}
	}
	private ITargetable target;
	private int turnsSinceUse = 999;

	public CharacterAction(CharacterActionData characterActionData, AbstractCharacter owner) {
		Data = characterActionData;
		currentPointsCost = Data.pointsCost;
		Owner = owner;
		AddPassiveTriggers();
	}

	public CharacterAction(CharacterActionData characterActionData) {
		Data = characterActionData;
		currentPointsCost = Data.pointsCost;
	}


	public CharacterAction(SerializableAction action, AbstractCharacter owner) {
		turnsSinceUse = action.turnsSinceUse;
		Data = ResourceLoader.GetAction(action.actionData);
		currentPointsCost = Data.pointsCost;
		Owner = owner;
		AddPassiveTriggers();
	}
	public CharacterAction(SerializableAction action) {
		turnsSinceUse = action.turnsSinceUse;
		Data = ResourceLoader.GetAction(action.actionData);
		currentPointsCost = Data.pointsCost;
	}

	public SerializableAction Serialize() {
		return new SerializableAction(Data.uniqueID, turnsSinceUse);
	}

	public int PointsCost => currentPointsCost;

	private int currentPointsCost = 0;

	public void SetPending() {
		Owner.SetPendingAction(PointsCost);
	}

	public void ClearPending() {
		Owner.ClearPendingAction();
	}

	public async UniTask OnBeforeRemoval() {
		if (Data.passiveTrigger == Trigger.None) {
			return;
		}
		Owner.RemoveTrigger(Data.uniqueID);
		await Data.Waive(this);
	}

	public async UniTask<Sprite> AwaitLoadSprite() {
		var spriteReference = Data.sprite;
		if(!spriteReference.RuntimeKeyIsValid()) {
			return null;
		}
		if (spriteReference.Asset == null) {
			var sprite = await spriteReference.LoadAssetAsync();
			return sprite;
		}
		return (Sprite)spriteReference.Asset;
	}

	private void AddPassiveTriggers() {
		if(Data.passiveTrigger == Trigger.None) {
			return;
		}
		var trigger = new TriggeredEvent(Data.passiveTrigger, this.Invoke, Data.uniqueID);
		Owner.AddTrigger(trigger);
	}

	public bool IsAvailable => Data.IsAvailable(this);


	public async UniTask Invoke() {
		if (!IsAvailable) {
			return;
		}
		if(await Data.Invoke(this)) {
			turnsSinceUse = 0;
		}
		if (!IsAvailable) {
			LevelController.Get().SetActiveAction(null);
		}
	}

	public async void Preview() {
		currentPointsCost = await Data.PreviewAction(this);
		SetPending();
	}

	public System.Type GetTargetType() => Data.GetTargetType();

	public bool ValidTarget() => Data.ValidTarget(this);
	
	public bool ValidTarget(ITargetable target) {
		Target = target;
		return ValidTarget();
	}

	public bool OnCooldown() {
		return turnsSinceUse < Data.cooldown;
	}

	public void Cooldown() {
		if (OnCooldown()) {
			turnsSinceUse++;
		}
	}
}
