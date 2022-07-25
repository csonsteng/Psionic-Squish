using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;

[Serializable]
public abstract class AbstractCharacter : ReferenceInstance<AbstractCharacterData>, ITargetable {

	#region Serialized Fields
	public Inventory inventory;

	public bool isActive = false;
	public bool isDead = false;
	public ColorScheme colorScheme;

	[SerializeReference] protected MapSpace position;
	[SerializeField] protected Direction direction;

	[SerializeField] protected int actionPointsRemaining;
	[SerializeField] protected int maxActionPoints;

	#endregion

	public UnityAction onClick;
	protected GameObject gameObject;
	protected LineOfSight lineOfSight;
	protected VisionProfile visionProfile;
	protected Material originalMaterial;
	protected AudioSource AudioSource => gameObject.GetComponent<AudioSource>();
	public Animator Animator => GetAnimator();
	protected List<TriggeredEvent> triggers = new List<TriggeredEvent>();
	protected ActionPointsUI pointsController;

	public AbstractCharacter(AbstractCharacterData data, ColorScheme scheme = null): base(data)
	{
		maxActionPoints = Data.actionPoints;
		inventory = new Inventory();
		if (scheme == null) {
			colorScheme = new ColorScheme(ColorUtilities.RandomColor());
		}
		else {
			colorScheme = scheme;
		}
		foreach(var action in Data.characterClass.startingActions) {
			AddAction(new CharacterAction(action, this));
		}
		RefreshActionPoints();
	}

	private Animator GetAnimator() {
		if(gameObject == null) {
			return null;
		}
		return gameObject.GetComponentInChildren<Animator>();
	}

	public CharacterAction DefaultAction() {
		if(inventory == null) {
			return null;
		}
		return inventory.GetDefaultAction();
	}

	public abstract void AddAction(CharacterAction action);
	public abstract UniTask<bool> RemoveAction(CharacterAction action, bool canRemoveFixed = false);

	public async UniTask TryFireTriggers(Trigger trigger) {
		foreach(var triggerEvent in triggers) {
			await triggerEvent.TryFireTrigger(trigger);
		}
	}

	public virtual void SetPendingAction(int pointsCost) {
		if(pointsController == null) {
			return;
		}
		pointsController.ShowPointsCost(pointsCost);
	}

	public virtual void ClearPendingAction() {
		pointsController.ClearPendingPoints();

	}

	public virtual void SetPointsController(ActionPointsUI pointsController) {
		this.pointsController = pointsController;
		pointsController.ShowActionPoints(this);
	}

	public virtual void ClearPointsController() {
		pointsController = null;
	}

	public void Die() {
		if (Animator == null) {
			return;
		}
		isDead = true;
		position.RelenquishPosition(gameObject);
		Animator.SetBool("Dead", true);
	}

	public void Idle() {
		if (Animator == null) {
			return;
		}
		Animator.updateMode = AnimatorUpdateMode.Normal;
		Animator.SetBool("Asleep", false);
		Animator.SetBool("Moving", false);
		Animator.SetBool("Dead", isDead);
	}

	public void Resurect() {
		isDead = false;
		Idle();
	}

	public void Sleep() {
		if(Animator == null) {
			return;
		}
		Animator.SetBool("Asleep", true);
	}

	public float SetMoving() {
		if (Animator == null) {
			return 1f;
		}
		Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
		Animator.SetBool("Moving", true);
		return Animator.GetCurrentAnimatorStateInfo(0).length;
	}

	public void AddTrigger(TriggeredEvent trigger) {
		triggers.Add(trigger);
	}

	public bool RemoveTrigger(string triggerID) {
		for(var i=triggers.Count -1; i>=0; i--) {
			if(triggers[i].id == triggerID) {
				triggers.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public Direction GetFacing() {
		return direction;
	}
	public void SetFacing(Direction direction) {
		this.direction = direction;
		gameObject.transform.SetPositionAndRotation(gameObject.transform.position, Quaternion.Euler(direction.Rotation()));
		UpdateVisionProfile();
	}
	public void SetPosition(MapSpace position) {
		if (this.position != null) {
			this.position.RelenquishPosition(GameObject);
		}
		position.ClaimPositionImpassable(GameObject, GetLayer());
		this.position = position;
	}
	public int ActionPoints() => actionPointsRemaining;
	public void ModifyAP(int deltaAP) {
		maxActionPoints += deltaAP;
		actionPointsRemaining += deltaAP;
	}

	public void SpendActionPoints(int points) {
		if (pointsController != null) {
			pointsController.SpendPendingPoints();
		}
		actionPointsRemaining -= points;
	}
	public void RefreshActionPoints() {
		actionPointsRemaining = maxActionPoints;
	}
	public GameObject GameObject => gameObject;
	
	public void SetGameObject(GameObject gameObject) {
		this.gameObject = gameObject;
		lineOfSight = gameObject.GetComponent<LineOfSight>();
		if (lineOfSight == null) {
			lineOfSight = gameObject.AddComponent<LineOfSight>();
		}
		lineOfSight.InitializeFOV(Data.fieldOfView, Data.viewDistance);
		var createMaterial = new Material(Renderer().material) {
			color = colorScheme.PrimaryColor
		};
		Renderer().material = createMaterial;
	}
	public abstract int GetLayer();
	public VisionProfile GetVisionProfile() {
		return visionProfile;
	}
	public abstract bool CanBeAttacked();
	public void UpdateVisionProfile(bool obscured = true) {
		if (lineOfSight == null) {
			return;
		}
		visionProfile = lineOfSight.UpdateVision(obscured);
	}
	public virtual async UniTask NewTurn() {
		await TryFireTriggers(Trigger.TurnStart);
	}
	public abstract void StepActions();
	public void TakeStep() {
		if (actionPointsRemaining <= 0) {
			return;
		}
		actionPointsRemaining--;
		StepActions();

		UpdateVisionProfile();
		Data.step.Play(AudioSource);

	}

	public MapSpace GetPosition() {
		return position;
	}

	public GameObject ClickableObject() {
		return gameObject;
	}

	public void HandleClick() {
		onClick?.Invoke();
	}

	public void HandleHover() {
		if (originalMaterial == null) {
			originalMaterial = Renderer().material;
		}
		Renderer().material = Data.hoverMaterial;

		Renderer().receiveShadows = false;
	}

	public void HandleUnHover() {
		if (originalMaterial != null) {
			Renderer().material = originalMaterial;
			Renderer().receiveShadows = true;
		}
	}

	public string GetName() {
		return Data.characterName;
	}

	private SkinnedMeshRenderer Renderer() {
		var child = gameObject.transform.GetChild(0);
		return child.GetComponentInChildren<SkinnedMeshRenderer>();
	}
}