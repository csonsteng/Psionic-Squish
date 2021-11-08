using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorNamer : MonoBehaviour
{
    public GameObject colorNamer;
    public TMP_InputField input;

    private ColorPanel panel;
    public void Submit() {
        panel.NewColor(input.text);
        Close();
	}

    public void Close() {
        colorNamer.SetActive(false);
	}

    public void Open(ColorPanel panel) {
        this.panel = panel;
        colorNamer.SetActive(true);
    }

    public static ColorNamer Get() {
        return FindObjectOfType<ColorNamer>();
	}
}
