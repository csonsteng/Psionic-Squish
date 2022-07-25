using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public abstract class CharacterActionData: ReferenceData {
    public string displayName;
    [TextArea]
    public string description;
    public Type type;
    public int rarity;
    public AssetReferenceSprite sprite;
    public Trigger passiveTrigger;
    public int pointsCost=1;
    public int cooldown = 1;
    public bool defaultAction = false;
    public ActionCriteria actionCriteria;

    public enum Type {
        Skill,
        Spell, 
        Item
	}

    public bool IsItem => type == Type.Item;

    public virtual bool RequiresTargets() {
        if(actionCriteria.targets == ActionCriteria.Targets.Self) {
            return false;
		}
        return true;
	}

    public virtual bool IsAvailable(CharacterAction action) {
        if(action.Owner == null)
		{
            Debug.LogError($"No owner on {uniqueID}");
            return false;
		}
        if (action.Owner.ActionPoints() >= pointsCost && !action.OnCooldown()) {
            return true;
        }
        return false;
    }

    public async UniTask<bool> Invoke(CharacterAction action) {
        if (actionCriteria.targets == ActionCriteria.Targets.Self) {
            action.Target = action.Owner;
        }
        if (RequiresTargets() && !ValidTarget(action)) {
            return false;
        }

        if(await TakeAction(action)) {
            action.Owner.SpendActionPoints(pointsCost);
            UIController.Get().UpdateActionInteractability();
            return true;
        }
        return false;
	}

    public async UniTask<bool> Waive(CharacterAction action) {
        return await UndoAction(action);
	}

    protected virtual async UniTask<bool> UndoAction(CharacterAction action) {
        await UniTask.CompletedTask;
        return false;
    }

    public virtual async UniTask<int> PreviewAction(CharacterAction action) {
        action.Target.HandleHover();
        await UniTask.CompletedTask;
        return pointsCost;
	}

    protected abstract UniTask<bool> TakeAction(CharacterAction action);

    public virtual bool ValidTarget(CharacterAction action) {
        if (RequiresTargets()) {
            return actionCriteria.Assess(action);
        }
        return false;
	}

    public virtual System.Type GetTargetType() => actionCriteria.GetTargetType();

    protected LevelController Controller => LevelController.Get();

}