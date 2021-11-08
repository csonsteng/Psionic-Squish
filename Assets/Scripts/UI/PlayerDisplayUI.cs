using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplayUI : MonoBehaviour
{

    public Image image;
    
    public void SetPlayer(PlayableCharacter player) {
        image.color = player.colorScheme.PrimaryColor;

		if (!player.isActive) {
            var buttons = GetComponentsInChildren<Button>();
            if(buttons.Length > 1) {
                throw new System.Exception("More than one button found on inactive player");
			}
            buttons[0].onClick.RemoveAllListeners();
            buttons[0].onClick.AddListener(() => {
                LevelController.Get().SetActivePlayer(player);
            });
		}

        var pointsController = GetComponentInChildren<ActionPointsUI>();
        player.SetPointsController(pointsController);
    }
    
}
