﻿using System.Collections;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int columns = 100;                               // The total number of columns in the level space
    public int rows = 100;                                  // The total number of rows in the level space
    public RandInt numRooms = new RandInt(15, 20);          // The range of how many rooms there can be
    public RandInt roomWidth = new RandInt(3, 10);          // The range of the allowed widths of each room
    public RandInt roomHeight = new RandInt(3, 10);         // The range of the allowed heights of each room
    public RandInt corridorLength = new RandInt(2, 10);     // The range of allowed lengths of corridors that connect rooms
    public GameObject[] floorTiles;                         // An array of floor tile prefabs
    public GameObject[] wallTiles;                          // An array of wall tile prefabs

    private TileType[][] tiles;                             // An array of tiles representing the level
    private Room[] rooms;                                   // An array of rooms for this level that are generated
    private Corridor[] corridors;                           // An array of corridors for this level that are generated
    private GameObject dungeonTiles;                        // GameObject that acts as a container for the tiles in this level


	public TileType[][] Generate () {
        dungeonTiles = new GameObject("DungeonTiles");

        SetupTilesArray();

        CreateRoomsAndCorridors();

        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();
        SetWallPositions();

        InstantiateTiles();

        return tiles;
    }
	
    /**
     * Initializes the tiles array for the generation algorithm to store tile types in.
     */
	void SetupTilesArray()
    {
        tiles = new TileType[columns + 2][];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileType[rows + 2];

            for (int j = 0; j < tiles[i].Length; j++)
                tiles[i][j] = TileType.Empty;
        }
    }

    void CreateRoomsAndCorridors()
    {
        // Initialize the number of rooms and corresponding corridors
        rooms = new Room[numRooms.Random];
        corridors = new Corridor[rooms.Length - 1];

        // Instantiate first room and corridor
        rooms[0] = new Room();
        corridors[0] = new Corridor();

        // Setup the first room and its corridor
        rooms[0].setupRoom(roomWidth, roomHeight, columns, rows);
        corridors[0].setupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

        for (int i = 1; i < rooms.Length; i++)
        {
            // Create and setup a new room
            rooms[i] = new Room();
            rooms[i].setupRoom(roomWidth, roomHeight, columns, rows, corridors[i - 1]);

            if (i < corridors.Length)
            {
                // Create and setup a new corridor
                corridors[i] = new Corridor();
                corridors[i].setupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
            }
        }
    }

    void SetTilesValuesForRooms()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            Room currRoom = rooms[i];

            // Loop through dimensions of room
            for (int j = 0; j < currRoom.roomWidth; j++)
            {
                int xCoord = currRoom.xPos + j;

                for (int k = 0; k < currRoom.roomHeight; k++)
                {
                    int zCoord = currRoom.zPos + k;

                    // Set tile at this position to a floor tile
                    tiles[xCoord + 1][zCoord + 1] = TileType.Floor;
                }
            }
        }
    }

    void SetTilesValuesForCorridors()
    {
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currCorridor = corridors[i];

            // Loop through its length
            for (int j = 0; j < currCorridor.corridorLength; j++)
            {
                int xCoord = currCorridor.startXPos;
                int zCoord = currCorridor.startZPos;

                switch (currCorridor.direction)
                {
                    case Direction.North:
                        zCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        zCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }

                // Set tile at this position to a floor tile
                tiles[xCoord + 1][zCoord + 1] = TileType.Floor;
            }
        }
    }

    void SetWallPositions()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                if (tiles[i][j] == TileType.Empty)
                {
                    if ((i > 0 && tiles[i - 1][j] == TileType.Floor) ||
                        (i < tiles.Length - 1 && tiles[i + 1][j] == TileType.Floor) ||
                        (j > 0 && tiles[i][j - 1] == TileType.Floor) ||
                        (j < tiles[i].Length - 1 && tiles[i][j + 1] == TileType.Floor))
                    {
                        tiles[i][j] = TileType.Wall;
                    }
                }
            }
        }
    }

    void InstantiateTiles()
    {
        // Loop through all tiles in the tiles array
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                // Instantiate a tile at this position
                switch (tiles[i][j])
                {
                    case TileType.Floor:
                        InstantiateFromArray(floorTiles[(i + j) % 2], i, 0, j);
                        break;
                    case TileType.Wall:
                        InstantiateFromArray(wallTiles, i, 1, j, 0, ((i + j) % 2)*90, 0);
                        break;
                }
            }
        }
    }

    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord, float zCoord, float xRot = 0, float yRot = 0, float zRot = 0)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, zCoord);

        Quaternion rot;
        if (xRot != 0 || yRot != 0 || zRot != 0)
            rot = Quaternion.Euler(xRot, yRot, zRot);
        else
            rot = Quaternion.identity;

        // Select a random prefab
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, rot) as GameObject;

        // Set prefab's parent to the level holder
        tileInstance.transform.parent = dungeonTiles.transform;
    }

    void InstantiateFromArray(GameObject prefab, float xCoord, float yCoord, float zCoord, float xRot = 0, float yRot = 0, float zRot = 0)
    {
        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, zCoord);

        Quaternion rot;
        if (xRot != 0 || yRot != 0 || zRot != 0)
            rot = Quaternion.Euler(xRot, yRot, zRot);
        else
            rot = Quaternion.identity;

        // Select a random prefab
        GameObject tileInstance = Instantiate(prefab, position, rot) as GameObject;

        // Set prefab's parent to the level holder
        tileInstance.transform.parent = dungeonTiles.transform;
    }
}
