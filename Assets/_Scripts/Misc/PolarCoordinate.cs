using UnityEngine;
using System.Collections;

namespace PolarCoordinates {
	public class PolarCoordinate {
		public float radius;
		public float angle;

		public PolarCoordinate(float newRadius, Vector3 cartesianPoint) {
			radius = newRadius;
			angle = Mathf.Atan2(cartesianPoint.z, cartesianPoint.x);
		}

		public PolarCoordinate(float newRadius, float newAngle) {
			radius = newRadius;
			angle = newAngle;
		}

		public Vector3 PolarToCartesian() {
			return new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
		}

		public static PolarCoordinate CartesianToPolar(Vector3 cart) {
			return new PolarCoordinate(Mathf.Sqrt(Mathf.Pow(cart.x, 2) + Mathf.Pow(cart.z, 2)), cart);
		}
	}
}
