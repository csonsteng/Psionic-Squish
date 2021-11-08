using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{

    public CharacterSelector character1;
    public CharacterSelector character2;

    public void StartGame() {
        var operation = SceneManager.LoadSceneAsync(1,LoadSceneMode.Single);
        operation.completed += PlayGame;


	}

    private async void PlayGame(AsyncOperation operation) {
        var levelController = LevelController.Get();
        if (levelController == null) {
            throw new System.Exception("Scene not yet loaded when trying to get level controller");
        }

        var party = new Party();

        party.members.Add(character1.GetCharacter());
        party.members.Add(character2.GetCharacter());

        levelController.SetParty(party);
        await levelController.Play();
    }
}
