using UnityEngine;
using System.Collections;

public class MazeCell : MonoBehaviour {

	public IntVector2 coordinates;				//Coordiantes of the cell
	public MazeRoom room;                       //Room that the cell belongs to
	public Vector2 size;						//Size of the cell

	private int initializedEdgeCount;			//Number of edges that have been checked so far

	//Array of all possible edges on a cell
	private MazeCellEdge[] edges = new MazeCellEdge[(int)MazeDirection.NumDirections];

	//Initialize cell to be in the specified room
	public void Initialize(MazeRoom room) {
		room.Add(this);
		transform.GetChild(0).GetComponent<Renderer>().material = room.settings.floorMaterial;
	}

	public MazeCellEdge GetEdge(MazeDirection direction) {
		return edges[(int)direction];
	}

	public void SetEdge(MazeDirection direction, MazeCellEdge edge) {
		edges[(int)direction] = edge;
		initializedEdgeCount += 1;
	}

	//Returns true if all of the edges have been checked for this cell
	public bool IsFullyInitialized {
		get {
			return initializedEdgeCount == (int)MazeDirection.NumDirections;
		}
	}

	//Get a random uninitialized direction
	public MazeDirection RandomUninitializedDirection {
		get {
			int skips = Random.Range(0, (int)MazeDirection.NumDirections - initializedEdgeCount);
			for(int i = 0; i < (int)MazeDirection.NumDirections; i++) {
				if (edges[i] == null) {
					if(skips == 0) {
						return (MazeDirection)i;
					}
					skips -= 1;
				}
			}
			throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
		}
	}
}
