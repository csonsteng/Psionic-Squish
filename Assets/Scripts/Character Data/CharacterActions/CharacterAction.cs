using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEditor.AnimatedValues;

[System.Serializable]
public class CharacterAction: ReferenceInstance<CharacterActionData> {

	protected override CharacterActionData LoadReference() => ResourceLoader.GetAction(referenceID);
	public AbstractCharacter Owner
	{
		get
		{
			return owner;
		}
		private set
		{
			owner = value;
		}
	}
	[SerializeReference] private AbstractCharacter owner;
	[field: SerializeField] public int PointsCost {get; private set;}
	[SerializeField] private int turnsSinceUse = 999;

	public string DisplayName => Data.displayName;
	public string Description => Data.description;

	public bool IsDefault => Data.defaultAction;

	public bool RequiresTargets => Data.RequiresTargets();
	public bool IsItem => Data.IsItem;

	public bool IsActive { get; set; }
	public ITargetable Target { get; set; }

	public CharacterAction(CharacterActionData characterActionData, AbstractCharacter owner): base(characterActionData) 
	{
		PointsCost = Data.pointsCost;
		Owner = owner;
		AddPassiveTriggers();
	}

	public CharacterAction(CharacterActionData characterActionData) : base(characterActionData)
	{
		PointsCost = Data.pointsCost;
	}

	public override void OnAfterDeserialize()
	{
		base.OnAfterDeserialize();
		AddPassiveTriggers();
	}

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
		PointsCost = await Data.PreviewAction(this);
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
