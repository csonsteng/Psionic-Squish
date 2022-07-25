using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
	public static void SetLayerRecursive(this GameObject gameObject, string layer) => 
		gameObject.SetLayerRecursive(LayerMask.NameToLayer(layer));
	public static void SetLayerRecursive(this GameObject gameObject, int layer)
	{
		gameObject.layer = layer;
		foreach (Transform transform in gameObject.transform)
		{
			transform.gameObject.SetLayerRecursive(layer);
		}
	}

	public static void Destroy(this GameObject gameObject) => Object.Destroy(gameObject);
	
}
