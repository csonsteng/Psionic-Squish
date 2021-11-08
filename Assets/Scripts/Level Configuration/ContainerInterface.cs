using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ContainerInterface : MonoBehaviour
{

    public Button interactButton;
	public ContainerContentsDisplay contentsDisplay;

	private void Start() {
		HideInteract();
		GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("WorldUICamera").GetComponent<Camera>();
	}
	public void ShowInteract(UnityAction action) {
		interactButton.onClick.RemoveAllListeners();
		interactButton.onClick.AddListener(action);
        interactButton.gameObject.SetActive(true);
		contentsDisplay.gameObject.SetActive(false);
	}

    public void HideInteract() {
		interactButton.onClick.RemoveAllListeners();
		contentsDisplay.gameObject.SetActive(false);
		interactButton.gameObject.SetActive(false);
	}

	public void ShowContents(LevelInteractableContainer container) {
		interactButton.gameObject.SetActive(false);
		contentsDisplay.gameObject.SetActive(true);
		contentsDisplay.ShowContents(container);
	}
}
