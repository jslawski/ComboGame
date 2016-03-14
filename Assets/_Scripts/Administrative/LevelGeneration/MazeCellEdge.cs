using UnityEngine;
using System.Collections;

public abstract class MazeCellEdge : MonoBehaviour {

	//The cell this edge belongs too, and the other cell that connects it
	public MazeCell cell, otherCell;

	//Direction of the cell edge
	public MazeDirection direction;

	//Initialize the edge to have the same position as the cell, as well as the proper
	//rotation, given its direction.  Populate all of the edge's member variables as well.
	public void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		this.cell = cell;
		this.otherCell = otherCell;
		this.direction = direction;
		cell.SetEdge(direction, this);
		transform.parent = cell.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = direction.ToRotation();
	}
}
