using UnityEngine;
using System.Collections;

public enum MazeDirection { North, East, South, West, NumDirections };

public static class MazeDirections {

	//Vectors to correspond with MazeDirection enum
	private static IntVector2[] vectors = {
		new IntVector2(0, 1),
		new IntVector2(1, 0),
		new IntVector2(0, -1),
		new IntVector2(-1, 0)
	};

	//Array of MazeDirection, in the opposite order of the enum
	private static MazeDirection[] opposites = {
		MazeDirection.South,
		MazeDirection.West,
		MazeDirection.North,
		MazeDirection.East
	};

	//Array of possible quaternions for rotation in the 4 directions
	private static Quaternion[] rotations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 270f, 0f)
	};

	//Convert MazeDirection to a vector
	//The "this" keyword allows us to call this function like so:
	//	MazeDirection someDirection = RandomDirection;
	//	someDirection.ToIntVector2();
	public static IntVector2 ToIntVector2(this MazeDirection direction) {
		return vectors[(int)direction];
	}

	//Return the opposite direction of the given direction
	public static MazeDirection GetOpposite(this MazeDirection direction) {
		return opposites[(int)direction];
	}

	//Return the correct rotation of the given direction
	public static Quaternion ToRotation(this MazeDirection direction) {
		return rotations[(int)direction];
	}

	//Get a random direction
	public static MazeDirection RandomDirection {
		get {
			return (MazeDirection)Random.Range(0, (int)MazeDirection.NumDirections);
		}
	}
}
