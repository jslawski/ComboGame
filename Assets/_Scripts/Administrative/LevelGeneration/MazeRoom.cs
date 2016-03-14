using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeRoom : ScriptableObject {

	public int settingsIndex;								//Index of the room that corresponds to the settings indexed in Maze

	public MazeRoomSettings settings;						//Settings of the room (currently just floor color)

	private List<MazeCell> cells = new List<MazeCell>();	//Cells contained within this room

	//Add a cell to this room
	public void Add(MazeCell cell) {
		cell.room = this;
		cells.Add(cell);
	}

	//Assimilate cells from one room into another room
	public void Assimilate(MazeRoom room) {
		for (int i = 0; i < room.cells.Count; i++) {
			Add(room.cells[i]);
		}
	}
}
