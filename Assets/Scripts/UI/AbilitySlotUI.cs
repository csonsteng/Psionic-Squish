using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public class AbilitySlotUI : MonoBehaviour
{
    public Image background;
    public Image border;
    public Image slot;

    public void SetScheme(ColorScheme scheme, bool reversed) {
        if (reversed) {
            background.color = scheme.Accent1;
            border.color = scheme.PrimaryColor;
            if (slot != null) {
                slot.color = scheme.PrimaryColor;
            }
        }
        else {
            background.color = scheme.PrimaryColor;
            border.color = scheme.Accent1;
            if (slot != null) {
                slot.color = scheme.Accent1;
            }
        }


        var slotButton = GetComponentInChildren<AbilitySlotButton>();
        if(slotButton != null) {
            slotButton.SetTooltipColor(scheme.Accent2);
		}
        foreach (var nestedSlot in GetComponentsInChildren<AbilitySlotUI>()) {
            if(nestedSlot == this) {
                continue;
			}
            nestedSlot.SetScheme(scheme, reversed);
		}
    }

    public void AddAction(CharacterAction action) {

        var slotButton = GetComponentInChildren<AbilitySlotButton>();
        if(slotButton == null) {
            throw new System.Exception($"No slot button found on {gameObject.name}");
		}

        slotButton.AddAction(action);

	}

    public void UpdateActionInteractability() {
        var slotButton = GetComponentInChildren<AbilitySlotButton>();
        if (slotButton == null) {
            return;
        }
        slotButton.UpdateInteractability();
    }

}
