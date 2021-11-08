using UnityEngine;
using System;

[System.Serializable]
public class Direction
{
	[SerializeField]
	public Compass compass;
	public enum Compass {
		N,
		NW,
		W,
		SW,
		S,
		SE,
		E,
		NE
	}

	public Direction() {

	}
	public Direction Random() {
		int randomDirection = UnityEngine.Random.Range(0, 8);
		compass = (Compass) randomDirection;
		return this;
	}
	public Direction(Vector2 direction) {

		if (direction.x == 0) {
			if (direction.y < 0) {
				compass = Compass.N;
			}
			else if (direction.y > 0) {
				compass = Compass.S;
			}
		}
		else if (direction.y == 0) {
			if (direction.x < 0) {
				compass = Compass.E;
			}
			else if (direction.x > 0) {
				compass = Compass.W;
			}
		}
		else if (direction.x < 0) {
			if (direction.y < 0) {
				compass = Compass.NE;
			}
			else if (direction.y > 0) {
				compass = Compass.SE;
			}
		}
		else if (direction.x > 0) {
			if (direction.y < 0) {
				compass = Compass.NW;
			}
			else if (direction.y > 0) {
				compass = Compass.SW;
			}
		}
		else {
			throw new Exception("Direction cannot be created from a 0,0 vector");
		}

	}

	public Direction(Compass orientation) {
		compass = orientation;
	}

	public Direction Opposite() {
		//because i can't figure out geometry -- side note - why the hell did I make this a compass rose and not just use angle??? lmao
		var direction = new Direction();
		switch (compass) {
			case Compass.N:
				direction.compass = Compass.S;
				break;
			case Compass.NW:
				direction.compass = Compass.SE;
				break;
			case Compass.W:
				direction.compass = Compass.E;
				break;
			case Compass.SW:
				direction.compass = Compass.NE;
				break;
			case Compass.S:
				direction.compass = Compass.N;
				break;
			case Compass.SE:
				direction.compass = Compass.NE;
				break;
			case Compass.E:
				direction.compass = Compass.W;
				break;
			case Compass.NE:
				direction.compass = Compass.SW;
				break;
		}
		return direction;
	}

	public Vector3 Rotation(bool isDegrees = true) {
		if (isDegrees) {
			return new Vector3(0f, Degrees(), 0f);
		}
		else {
			return new Vector3(0f, Radians(), 0f);
		}
	}

	public float Degrees() {
		return -45 * (int)compass;
	}

	public float Radians() {
		return Degrees() * Mathf.PI / 180f;
	}
}
