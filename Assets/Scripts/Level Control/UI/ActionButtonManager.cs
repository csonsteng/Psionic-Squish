using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ActionButtonManager : MonoBehaviour
{

    public GameObject buttonTemplate;

    private List<GameObject> buttons = new List<GameObject>();

    public void ClearButtons() {
        foreach (var button in buttons) {
            Destroy(button);
        }
        buttons.Clear();
	}

    private void Start() {
        buttonTemplate.SetActive(false);
	}

	private void AddButton(string label, bool available, int pointsCost, string description, UnityAction action) {
        var newButton = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
        var actionButton = newButton.GetComponent<ActionButton>();
        actionButton.ConfigureButton(label, pointsCost, description, available, action);
  //      var buttonComponent = newButton.GetComponent<Button>();
  //      if (available) {
  //          buttonComponent.interactable = true;
  //          buttonComponent.onClick.AddListener(() => {
  //              action?.Invoke();
  //          });
  //      }
		//else {
  //          buttonComponent.interactable = false;
  //      }
  //      newButton.GetComponentInChildren<TextMeshProUGUI>().text = label;
        newButton.SetActive(true);
        buttons.Add(newButton);

    }

    public void AddButtons() {
        var character = LevelController.ActivePlayer;
		if (character == null) {
            return;
		}
        //var actions = character.actions;
        ClearButtons();
        bool interactable = LevelController.ActiveAction != null && character.ActionPoints() > 1;
        AddButton("Move", interactable, 1, "Move one square", () => {
            LevelController.SetActiveAction(null);
            AddButtons();
        });
        foreach (var action in character.inventory.GetAllActions()) {
            interactable = action.IsAvailable && action != LevelController.ActiveAction;
            AddButton(action.DisplayName, interactable, action.PointsCost, action.Description, () => {
                LevelController.SetActiveAction(action);
                AddButtons();
            });
        }
        AddButton("End Turn", true, 0, "End Turn",() => {
            LevelController.EndTurn();
        });
    }

    private LevelController LevelController => LevelController.Get();

}
