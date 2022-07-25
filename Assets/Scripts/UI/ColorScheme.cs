using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorScheme
{
    public Color PrimaryColor => primary;
    public Color Accent1 => accent1;
    public Color Accent2 => accent2;

    [SerializeField] private Color primary;
    [SerializeField] private Color accent1;
    [SerializeField] private Color accent2;

    public ColorScheme(Color color) {
        primary = color;
        accent1 = ColorUtilities.FindMonochromatic(color);
        accent2 = ColorUtilities.FindMonochromatic(color, false);
    }

}
