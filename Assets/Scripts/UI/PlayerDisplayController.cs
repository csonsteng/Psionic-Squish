using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplayController : MonoBehaviour
{

	public GameObject activePlayer;
	public List<GameObject> inactivePlayers = new List<GameObject>();
	public void ShowPlayers() {
		var members = LevelController.Get().GetParty().members;

		int inactiveIndex = 0;
		foreach(var member in members){
			if (member.isActive) {
				ShowPlayer(member, activePlayer);
				continue;
			}

			if(inactiveIndex >= inactivePlayers.Count) {
				throw new System.Exception("Number of players exceeds number of available player displays");
			}
			ShowPlayer(member, inactivePlayers[inactiveIndex]);
			inactiveIndex++;
		}
	}

	public void HidePlayers() {
		activePlayer.SetActive(false);
		foreach(var player in inactivePlayers) {
			player.SetActive(false);
		}
	}

	private void ShowPlayer(PlayableCharacter player, GameObject position) {
		var playerDisplay = position.GetComponent<PlayerDisplayUI>();
		playerDisplay.SetPlayer(player);
		position.SetActive(true);
	}
}
