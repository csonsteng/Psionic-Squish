using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UIController : MonoBehaviour
{
    public AbilityBarUI abilityBar;
    public PlayerDisplayController playerDisplay;
    public TextMeshProUGUI enemyTurnIndicator;
    public GameObject restartButton;
    public GameObject screenCover;
    public GameObject rewindButton;

    public Button clickCanceller;

    void Start()
    {
        Restart();
    }

    public void HideGame() {
        screenCover.SetActive(true);
	}

    public void ShowGame() {
        screenCover.SetActive(false);
	}

	private void Restart() {
        enemyTurnIndicator.gameObject.SetActive(false);
        clickCanceller.gameObject.SetActive(false);
    }

	public void PlayersTurn() {
        playerDisplay.ShowPlayers();
        abilityBar.ShowActions();
        enemyTurnIndicator.gameObject.SetActive(false) ;
    }

    public void EnemyTurn() {
        playerDisplay.HidePlayers();
        enemyTurnIndicator.gameObject.SetActive(true);
    }

    public void UpdateActionInteractability() {

        abilityBar.UpdateActionInteractability();
    }


    public void ShowRestartButton() {
        restartButton.SetActive(true);
	}

    public void ShowStartScreen() {
        LevelController.winText.gameObject.SetActive(false); 
        Restart();
        restartButton.SetActive(false);
	}

    public void DisableRewinds() {
        rewindButton.SetActive(false);
	}

    public void EnableRewinds() {
        rewindButton.SetActive(true);
	}

    public void EnableClickCanceller(UnityAction action) {
        clickCanceller.onClick?.Invoke();
        clickCanceller.gameObject.SetActive(true);
        clickCanceller.onClick.AddListener(() => {
            clickCanceller.gameObject.SetActive(false);
            action.Invoke();
            clickCanceller.onClick.RemoveAllListeners();
        });
	}

    private LevelController LevelController => LevelController.Get();

    public static UIController Get() {
        return GameObject.FindObjectOfType<UIController>();
    }
}
