using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class EnemyCharacter : AbstractCharacter {
    public EnemyStatus Status {
		get {
            return status;
		}
		set {
            status = value;
            if (statusShowing) {
                EnemyStateMachine.Get().ShowEnemyStatus(this).Forget();
            }
            if(status == EnemyStatus.Sleeping) {
                Sleep();
			}
        }
    }
    [SerializeField] private EnemyStatus status;
    [SerializeReference] public MapSpace target;
    public Direction targetDirection;

    [SerializeReference] public List<MapSpace> patrolPath = new List<MapSpace>();
    [SerializeReference] public List<MapSpace> tileIndicators = new List<MapSpace>();
    public bool statusShowing = false;

    private bool OnDefense => status == EnemyStatus.Defend || status == EnemyStatus.FallingBack;

    private EnemyStatusIndicator StatusIndicator => gameObject.GetComponentInChildren<EnemyStatusIndicator>();

    public EnemyCharacter(EnemyCharacterData data): base(data) {
        direction = new Direction().Random();
    }

    protected override AbstractCharacterData LoadReference() => ResourceLoader.GetEnemy(referenceID);
    public override bool CanBeAttacked() => true;
    public void ClearTileIndicators() {
        foreach(var space in tileIndicators) {
            space.ClearStepIndicator();
		}
        tileIndicators.Clear();
	}

    public void ShowStatus() {
        StatusIndicator.SetStatus(Status);
        statusShowing = true;
	}

    public void ClearStatus() {
        StatusIndicator.ClearStatus();
        ClearTileIndicators();
        statusShowing = false;
    }

    public void SetAlert() {
        StatusIndicator.SetAlert();
	}


    public List<MapSpace> GetPath() {
        if(target == null) {
            return new List<MapSpace>();
		}
        var path = Pathfinding.GetPath(position, target);
        return path;
    }

    public void SetChase(AbstractCharacter character) {
        SetChase(character.GetPosition());
        targetDirection = character.GetFacing().Opposite();
    }

    public void TrySetChase(MapSpace space) {
		if (!OnDefense && status != EnemyStatus.Chase) {
            SetChase(space);
		}
	}

    public void SetChase(MapSpace space) {
        Status = EnemyStatus.Chase;
        SetAlert();
        target = space;
        if (patrolPath.Count == 0) {
			patrolPath.Add(GetPosition());
			patrolPath.Add(target);
		}
    }

    public void SetFallingBack(MapSpace space) {
        Status = EnemyStatus.FallingBack;
        target = space;
    }

    public void SetDefend(MapSpace patrolPoint) {
        Status = EnemyStatus.Defend;
        patrolPath.Clear();
        patrolPath.Add(GetPosition());
        patrolPath.Add(patrolPoint);
        target = patrolPoint;
    }

    public override void StepActions() {

        var cameraPoint = GetGroundPosition();
        var myDistance = (cameraPoint - gameObject.transform.position).magnitude;
        var volume = 0.9f*(20f - myDistance) / 20f;
        volume = Mathf.Clamp(volume, 0f, 0.9f);
        AudioSource.volume = volume;
        LevelController.Get().ShowSound(gameObject);
    }

    private Vector3 GetGroundPosition() {
        var mainCamera = UnityEngine.GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Ray worldRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        groundPlane.Raycast(worldRay, out float distanceToGround);
        return worldRay.GetPoint(distanceToGround);
    }

    public override int GetLayer() => LayerMask.NameToLayer("Default");

    public override async UniTask NewTurn() {
        await base.NewTurn();
        actionPointsRemaining = maxActionPoints;
        ClearStatus();
    }

    public void EndTurn() {
        actionPointsRemaining = 0;
	}

    public override void AddAction(CharacterAction action) {
        inventory.TryAddAction(action);
    }

    public override async UniTask<bool> RemoveAction(CharacterAction action, bool canRemoveFixed = false) {
        if(!action.IsItem && !canRemoveFixed) {
            return false;
		}
        bool hadItem = inventory.RemoveAction(action.Data);
        await UniTask.CompletedTask;
        return hadItem;
    }
}
