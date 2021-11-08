using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionProfile 
{
	public Dictionary<GameObject, int> tileHits = new Dictionary<GameObject, int>();

	int maxHidden = 2;
	int maxPartial = 10;



	public Threshold GetThreshold(GameObject tileObject) {
		tileHits.TryGetValue(tileObject, out int hits);
		return CalculateThreshold(hits);	//will return hidden if tileObject is not in dictionary
	}

	public IEnumerable<KeyValuePair<GameObject, Threshold>> GetThresholds() {
		foreach(var tileHit in tileHits) {
			yield return new KeyValuePair<GameObject, Threshold>(tileHit.Key, CalculateThreshold(tileHit.Value));
		}
	}

	public Threshold GetThreshold(MapSpace mapSpace) {
		return GetThreshold(mapSpace.GetTileObject());
	}

	private Threshold CalculateThreshold(int hits) {
		if (hits > maxPartial) {
			return Threshold.Complete;
		}
		else if (hits > maxHidden) {
			return Threshold.Partial;
		}
		return Threshold.Hidden;
	}
}

public enum Threshold {
	Hidden,
	Partial,
	Complete
}
