using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyStatusIndicator : MonoBehaviour
{
    public TextMeshProUGUI statusIndicator;
    // Start is called before the first frame update
    void Start()
    {
        ClearStatus();
    }

    public void SetStatus(EnemyStatus status) {
		switch (status) {
            case EnemyStatus.Chase:
                SetIndicator("Chasing");
                break;
            case EnemyStatus.Patrol:
                SetIndicator("Patrolling");
                break;
            case EnemyStatus.Hunt:
                SetIndicator("Hunting");
                break;
            case EnemyStatus.Stationary:
                SetIndicator("Stationary");
                break;
            case EnemyStatus.Sleeping:
                SetIndicator("Sleeping");
                break;
            case EnemyStatus.FallingBack:
                SetIndicator("Falling Back");
                break;
            case EnemyStatus.Defend:
                SetIndicator("Defending");
                break;
        }
    }

    public void SetAlert() {
        SetIndicator("!");
	}

    private void SetIndicator(string indicator) {
        statusIndicator.text = indicator;
        statusIndicator.gameObject.SetActive(true);
    }

    public void ClearStatus() {
        statusIndicator.text = "";
        statusIndicator.gameObject.SetActive(false);
    }
}
