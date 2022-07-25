using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Anything that the player can target (characters and mapspaces)
/// </summary>
public interface ITargetable 
{
	public MapSpace GetPosition();
    public GameObject ClickableObject();
    public void HandleClick();
    public void HandleHover();
    public void HandleUnHover();
    public string GetName();
}
