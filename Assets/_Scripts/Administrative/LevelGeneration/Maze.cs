using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	public IntVector2 size;									//Size of the maze
	public MazeCell cellPrefab;								//Maze cell
	
	public MazePassage passagePrefab;						//Maze Passage (aka open space)
	public MazeDoor doorPrefab;								//Maze door.  Currently identical visually to passagePrefab
	public MazeWall wallPrefab;								//Maze Wall
	public MazeRoomSettings[] roomSettings;					//Array of settings a room can have (indexed in the inspector)

	[Range(0f, 1f)]
	public float doorProbability;							//Probability of a door appearing instead of a passage

	private MazeCell[,] cells;								//All cells within the maze
	private List<MazeRoom> rooms = new List<MazeRoom>();	//List of rooms within the maze

	//Return a random coordinates inside the maze
	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
		}
	}

	//Check to see if coordinates is within the maze boundaries
	public bool ContainsCoordinates(IntVector2 coordinates) {
		return (coordinates.x >= 0) && (coordinates.x < size.x) && (coordinates.z >= 0) && (coordinates.z < size.z);
	}

	//Get a specific cell from the maze's array of cells
	public MazeCell GetCell(IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}


	//Generate the entire maze
	public IEnumerator Generate() {
		cells = new MazeCell[size.x, size.z];

		//Keep a list of all active cells currently in the maze
		List<MazeCell> activeCells = new List<MazeCell>();

		//Create the initial cell and initialize it's room
		MazeCell newCell = CreateCell(RandomCoordinates);
		newCell.Initialize(CreateRoom(-1));
		activeCells.Add(newCell);

		//Iterate while the coordinates is within the maze AND hasn't been visited before
		while (activeCells.Count > 0) {
			yield return new WaitForSeconds(0.001f);
			GenerateNextCell(activeCells);
		}

	}

	//Generate the next cell in the maze
	private void GenerateNextCell(List<MazeCell> activeCells) {
		//Choose a random cell from the list of active cells
		int curIndex = activeCells.Count - 1;//Random.Range(0, activeCells.Count);
		MazeCell curCell = activeCells[curIndex];

		//If all of the edges of the current cell have been checked,
		//remove it from the list of active cells
		if (curCell.IsFullyInitialized) {
			activeCells.RemoveAt(curIndex);
			return;
		}

		//Choose a random direction to check next
		MazeDirection direction = curCell.RandomUninitializedDirection;
		IntVector2 coordinates = curCell.coordinates + direction.ToIntVector2();

		//Only generate the next cell if it is within the maze boundaries
		if (ContainsCoordinates(coordinates)) {
			MazeCell neighbor = GetCell(coordinates);
			//Generate a passage if the next cell hasn't been visited before
			if (neighbor == null) {
				neighbor = CreateCell(coordinates);
				CreatePassage(curCell, neighbor, direction);
				activeCells.Add(neighbor);
			}
			//If the neighbor cell already exists, and is in the same room,
			//the edge connecting the two is automatically a passage
			else if (curCell.room.settingsIndex == neighbor.room.settingsIndex) {
				CreatePassageInSameRoom(curCell, neighbor, direction);
			}
			//If the next cell has already been placed before, generate a wall
			else {
				CreateWall(curCell, neighbor, direction);
			}
		}
		//Otherwise, remove the cell from the list of active cells in the maze
		//and put a wall there
		else {
			CreateWall(curCell, null, direction);
		}
	}

	//Create a passage edge if the cells are in the same room
	private void CreatePassageInSameRoom(MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		//Cell 1
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		//Cell 2
		passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());

		//Assimilate cells from one room into another room if the settings
		//of the room are the same, but the actual rooms are separate instances
		if (cell.room != otherCell.room) {
			MazeRoom roomToAssimilate = otherCell.room;
			cell.room.Assimilate(roomToAssimilate);
			rooms.Remove(roomToAssimilate);
			Destroy(roomToAssimilate);
		}
	}

	//Instantiate and initialize the passage edge for both cells that are touching
	private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		//Determine whether or not the passage is a door to another room, or a simple passage
		MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;

		//Instantiate passage for first cell
		MazePassage passage = Instantiate(prefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);

		//If the passage leads to another room, create a new room
		//for the otherCell
		if (passage is MazeDoor) {
			otherCell.Initialize(CreateRoom(cell.room.settingsIndex));
		}
		else {
			otherCell.Initialize(cell.room);
		}

		//Instantiate passage for second cell
		passage = Instantiate(prefab) as MazePassage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}

	//Instantiate and initialize a wall edge for both cells that are touching (or the one cell at the edge of the maze)
	private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		//Instantiate for first cell
		MazeWall wall = Instantiate(wallPrefab) as MazeWall;
		wall.Initialize(cell, otherCell, direction);

		//Create a second wall for the other cell if it exists
		//ie: If the first wall wasn't generated at the edge of the maze
		if (otherCell != null) {
			wall = Instantiate(wallPrefab) as MazeWall;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
	}

	//Create a cell in the game at the specified coordinates
	private MazeCell CreateCell(IntVector2 coordinates) {
		//Instantiate new cell
		MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
		
		//Add new cell to array of maze cells
		cells[coordinates.x, coordinates.z] = newCell;

		//Set the cell's coordinates
		newCell.coordinates = coordinates;

		//Set the cell's size
		newCell.size = new Vector2(newCell.transform.GetChild(0).transform.localScale.x, newCell.transform.GetChild(0).transform.localScale.y);

		print(newCell.size);

		//Name the cell something useful to view in the heirarchy
		newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;

		//Set the cell's parent to the Maze object
		newCell.transform.parent = transform;

		//Set the cell's localPosition to its proper location in the maze
		newCell.transform.localPosition = new Vector3((coordinates.x * newCell.size.x) - (size.x * 0.5f) + (0.5f * newCell.size.x), 0f, 
													  (coordinates.z * newCell.size.y) - (size.z * 0.5f) + (0.5f * newCell.size.y));

		return newCell;
	}

	//Create a new room in the maze
	private MazeRoom CreateRoom(int indexToExclude) {
		MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();

		//Pick a random room type
		newRoom.settingsIndex = Random.Range(0, roomSettings.Length);

		//Ensures the same room type doesn't get generated adjacent to each other
		//If the new room's settingsIndex is the same as the indexToExclude, just use
		//that index + 1 for a different room.
		if (newRoom.settingsIndex == indexToExclude) {
			newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;
		}

		//Initialize the new room and add it to the maze's list of rooms
		newRoom.settings = roomSettings[newRoom.settingsIndex];
		rooms.Add(newRoom);

		return newRoom;
	}
}
