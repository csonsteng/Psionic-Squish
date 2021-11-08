using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    public Image characterDisplay;
    public Image darkAccent;
    public Image lightAccent;
    public Button colorTemplate;
    public ColorPalette palette;
    private PlayableCharacterData characterClass;
    private Color color;
    private ColorScheme scheme;
    private List<Button> buttons = new List<Button>();

    private void Start() {
        foreach(var color in palette.GetDisplayColors()) {
            var button = Instantiate(colorTemplate, colorTemplate.transform.parent);
            button.onClick.AddListener(() => {
                SetButton(color);
                button.interactable = false;
            });
            var image = button.GetComponentsInChildren<Image>()[1];
            image.color = color;
            button.gameObject.SetActive(true);
            buttons.Add(button);
		}

        var colorRoll = Random.Range(0, buttons.Count);
        var startingButton = buttons[colorRoll];
        startingButton.onClick.Invoke();

        var allClasses = ResourceLoader.References.characters.items;
        var classRoll = Random.Range(0, allClasses.Count);
        SetClass(allClasses[classRoll]);
	}
    public void SetClass(PlayableCharacterData selectedClass) {
        characterClass = selectedClass;
    }

    public PlayableCharacterData Class => characterClass;
    public ColorScheme Scheme => scheme;

    public PlayableCharacter GetCharacter() {
        if(Class == null || Scheme == null) {
            throw new System.Exception("No class and/or scheme");
		}
        return new PlayableCharacter(Class, Scheme);

	}

    public void SetButton(Color color) {
        this.color = color;
        foreach(var button in buttons) {
            button.interactable = true;
		}
        scheme = new ColorScheme(color);
        characterDisplay.color = scheme.PrimaryColor;
        var accent1 = scheme.Accent1;
        var accent2 = scheme.Accent2;
		if (ColorUtilities.CloserToBlack(accent1)) {
            darkAccent.color = accent1;
            lightAccent.color = accent2;
		}
		else {
            darkAccent.color = accent2;
            lightAccent.color = accent1;
        }

	}

}
