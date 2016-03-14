using UnityEngine;
using System.Collections;

[System.Serializable]
public struct IntVector2 {
	public int x, z;

	public IntVector2 (int newX, int newZ) {
		x = newX;
		z = newZ;
	}

	public static IntVector2 operator + (IntVector2 a, IntVector2 b) {
		a.x += b.x;
		a.z += b.z;
		return a;
	}
}
