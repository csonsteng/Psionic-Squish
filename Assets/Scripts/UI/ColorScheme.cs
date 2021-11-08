using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorScheme
{
    public Color PrimaryColor { get; }
    public Color Accent1 { get; }
    public Color Accent2 { get; }

    public ColorScheme(Color color) {
        PrimaryColor = color;
        Accent1 = ColorUtilities.FindMonochromatic(color);
        Accent2 = ColorUtilities.FindMonochromatic(color, false);
    }

}
