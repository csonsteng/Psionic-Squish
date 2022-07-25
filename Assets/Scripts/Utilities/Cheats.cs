using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cheats : MonoBehaviour
{
	private LevelController controller;

	private void Start()
	{
		controller = LevelController.Get();
	}
	private void Update()
	{
		var keyboard = Keyboard.current;
		if(keyboard.leftCtrlKey.isPressed && keyboard.vKey.isPressed)
		{
			var map = controller.GetMap();
			foreach(var space in map.spaces)
			{
				space.ShowTileAndOccupants();
			}

		}
	}
}
