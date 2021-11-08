using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable 
{
	public MapSpace GetPosition();

    public delegate void Callback();
    public GameObject ClickableObject();
    public void HandleClick();
    public void HandleHover();
    public void HandleUnHover();
    public string GetName();
}
