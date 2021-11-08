using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(Button))]
public class ContainerContentsSlot : MonoBehaviour
{
	public TextMeshProUGUI itemName;
	public Image itemPicture;

	private Button Button => GetComponent<Button>();

	public void SetButton(UnityAction action, string item, Sprite sprite = null) {
		Button.onClick.RemoveAllListeners();
		if (action != null) {
			Button.onClick.AddListener(action);
		}
		if(sprite == null) {
			itemName.text = item;
		}
	}
}
