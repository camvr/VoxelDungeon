using System.Collections.Generic;
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
    public GameObject[] miscObjects;
    public GameObject wallTorch;
    public GameObject endTile;
    public bool fillVoid = true;
    public GameObject voidTile;

    private TileType[][] tiles;                             // An array of tiles representing the level
    private Room[] rooms;                                   // An array of rooms for this level that are generated
    private Corridor[] corridors;                           // An array of corridors for this level that are generated
    private GameObject dungeonTiles;                        // GameObject that acts as a container for the tiles in this level
    private int playerRoomIndex = -1;

    private TileType[] _legalFloorTiles = { TileType.Floor, TileType.Exit };
    private int level;

    private void Start()
    {
        if (GameManager.instance.isTutorial)
            level = 1;
        else
            level = LevelManager.instance.GetLevel() - 1;
        columns += 10 * level;
        rows += 10 * level;

        numRooms.m_Min += level * Random.Range(1, 4);
        numRooms.m_Max += level * Random.Range(1, 4);

        roomWidth.m_Min += level * Random.Range(1, 4);
        roomWidth.m_Max += level * Random.Range(1, 4);

        roomHeight.m_Min += level * Random.Range(1, 4);
        roomHeight.m_Max += level * Random.Range(1, 4);

        corridorLength.m_Min += level * Random.Range(1, 5);
        corridorLength.m_Max += level * Random.Range(1, 5);
    }

    public TileType[][] Generate () {
        dungeonTiles = new GameObject("DungeonTiles");

        SetupTilesArray();

        CreateRoomsAndCorridors();

        SetTilesValuesForRooms();
        SetTilesValuesForCorridors();
        PlaceDungeonFeatures();
        SetWallPositions();

        InstantiateTiles();

        return tiles;
    }

    public Vector3 GetPlayerStartPos()
    {
        int rightmost = 0;
        int minNum = columns + rows;

        // find furthest room from exit >:D
        for (int i = 1; i < rooms.Length; i++)
        {
            if (rooms[i].xPos + rooms[i].zPos < minNum)
            {
                minNum = rooms[i].xPos + rooms[i].zPos;
                rightmost = i;
            }
        }
        playerRoomIndex = rightmost;
        /*pos.x = rooms[rightmost].xPos + Random.Range(0, rooms[rightmost].roomWidth);
        pos.y = 0.5f;
        pos.z = rooms[rightmost].zPos + Random.Range(0, rooms[rightmost].roomHeight);*/
        Vector3 newPos = new Vector3(rooms[rightmost].xPos + Random.Range(0, rooms[0].roomWidth) + 1, 0.5f, rooms[rightmost].zPos + Random.Range(0, rooms[0].roomHeight) + 1);
        while (newPos.x < 0 || newPos.x >= tiles.Length || newPos.z < 0 || newPos.z >= tiles[0].Length || tiles[(int)newPos.x][(int)newPos.z] != TileType.Floor)
            newPos = new Vector3(rooms[rightmost].xPos + Random.Range(0, rooms[0].roomWidth) + 1, 0.5f, rooms[rightmost].zPos + Random.Range(0, rooms[0].roomHeight) + 1);

        return newPos;
    }

    public List<Vector3> GetEnemyDistributedPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < rooms.Length; i++)
        {
            if (playerRoomIndex != i && Random.Range(0f, 1f) >= 0.1)
            {
                Vector3 newPos = new Vector3(rooms[i].xPos + Random.Range(0, rooms[0].roomWidth) + 1, 0.5f, rooms[i].zPos + Random.Range(0, rooms[0].roomHeight) + 1);
                while (positions.Contains(newPos) || newPos.x < 0 || newPos.x >= tiles.Length || newPos.z < 0 || newPos.z >= tiles[0].Length ||tiles[(int)newPos.x][(int)newPos.z] != TileType.Floor)
                    newPos = new Vector3(rooms[i].xPos + Random.Range(0, rooms[0].roomWidth) + 1, 0.5f, rooms[i].zPos + Random.Range(0, rooms[0].roomHeight) + 1);
                positions.Add(newPos);
            }
        }

        return positions;
    }
	
    /**
     * Initializes the tiles array for the generation algorithm to store tile types in.
     */
	private void SetupTilesArray()
    {
        tiles = new TileType[columns + 2][];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileType[rows + 2];

            for (int j = 0; j < tiles[i].Length; j++)
                tiles[i][j] = TileType.Empty;
        }
    }

    private void CreateRoomsAndCorridors()
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

    private void SetTilesValuesForRooms()
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
                    tiles[xCoord][zCoord] = TileType.Floor;
                }
            }
        }
    }

    private void SetTilesValuesForCorridors()
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
                tiles[xCoord][zCoord] = TileType.Floor;
            }
        }
    }

    private void SetWallPositions()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                if (tiles[i][j] == TileType.Empty)
                {
                    if ((i > 0 && IsFloorTile(i - 1, j)) ||
                        (i < tiles.Length - 1 && IsFloorTile(i + 1, j)) ||
                        (j > 0 && IsFloorTile(i, j - 1)) ||
                        (j < tiles[i].Length - 1 && IsFloorTile(i, j + 1)))
                    {
                        if (Random.Range(0f, 1f) < 0.1f && NumSurroundingTiles(i, j, TileType.WallTorch) == 0)
                            tiles[i][j] = TileType.WallTorch;
                        else
                            tiles[i][j] = TileType.Wall;
                    }
                    else if ((i > 0 && j > 0 && IsFloorTile(i - 1, j - 1)) ||
                        (i < tiles.Length - 1 && j < tiles[i].Length - 1 && IsFloorTile(i + 1, j + 1)) ||
                        (i < tiles.Length - 1 && j > 0 && IsFloorTile(i + 1, j - 1)) ||
                        (i > 0 && j < tiles[i].Length - 1 && IsFloorTile(i - 1, j + 1)))
                    {
                        tiles[i][j] = TileType.Wall; // no torches on diagonals!
                    }
                }
            }
        }
    }

    private bool IsFloorTile(int x, int z)
    {
        foreach (TileType tile in _legalFloorTiles)
        {
            if (tiles[x][z] == tile)
                return true;
        }
        return false;
    }

    private void PlaceDungeonFeatures()
    {
        /* Place level exit */
        int leftmost = 0;
        int maxNum = 0;

        // find furthest room
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].xPos + rooms[i].zPos > maxNum)
            {
                maxNum = rooms[i].xPos + rooms[i].zPos;
                leftmost = i;
            }
        }

        // place exit in its approximate center
        int exitX = rooms[leftmost].xPos + Mathf.FloorToInt(rooms[leftmost].roomWidth / 2f);
        int exitZ = rooms[leftmost].zPos + Mathf.FloorToInt(rooms[leftmost].roomHeight / 2f);
        tiles[exitX][exitZ] = TileType.Exit;


        /* Random Dungeon features */
        for (int i = 0; i < rooms.Length; i++)
        {
            if (Random.Range(0f, 1f) < 0.4)
            {
                Vector2 pos = rooms[i].GetNonBlockingPosition();
                while (pos.x < columns && pos.y < rows && tiles[(int)pos.x][(int)pos.y] != TileType.Floor)
                {
                    pos = rooms[i].GetNonBlockingPosition();
                }

                tiles[(int)pos.x][(int)pos.y] = TileType.Misc;
            }
        }

        /* Old attempt for torches */
        /*for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                if (tiles[i][j] == TileType.Wall && NumSurroundingTiles(i, j, TileType.Floor) < 3) // TODO: change randomization to analytical tactic
                {
                    tiles[i][j] = TileType.WallTorch;
                }
            }
        }*/
    }

    private void InstantiateTiles()
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
                        InstantiateTile(floorTiles[(i + j) % 2], i, 0, j);
                        break;
                    case TileType.Wall:
                        InstantiateTile(wallTiles, i, 1.5f, j, 0, ((i + j) % 2)*90, 0);
                        break;
                    case TileType.WallTorch:
                        GameObject wallInstance = InstantiateTile(wallTiles, i, 1.5f, j);
                        GameObject torchInstance = InstantiateTile(wallTorch, 0, 0, 0);
                        torchInstance.transform.parent = wallInstance.transform;

                        if (i > 0 && tiles[i - 1][j] == TileType.Floor) // -x
                        {
                            torchInstance.transform.localPosition = new Vector3(-14, 6, 0);
                            torchInstance.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                        }
                        else if (i < columns && tiles[i + 1][j] == TileType.Floor) // +x
                        {
                            torchInstance.transform.localPosition = new Vector3(14, 6, 0);
                            torchInstance.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
                        }
                        else if (j > 0 && tiles[i][j - 1] == TileType.Floor) // -z
                        {
                            torchInstance.transform.localPosition = new Vector3(0, 6, -14);
                            torchInstance.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        }
                        else if (j < rows && tiles[i][j + 1] == TileType.Floor) // +z
                        {
                            torchInstance.transform.localPosition = new Vector3(0, 6, 14);
                            torchInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                        }
                        break;
                    case TileType.Exit:
                        InstantiateTile(endTile, i, 0, j, 0, Random.Range(0, 4) * 90f, 0);
                        break;
                    case TileType.Misc:
                        InstantiateTile(miscObjects, i, 0.5f, j, 0, Random.Range(0, 4) * 90f, 0);
                        break;
                    case TileType.Empty:
                        if (fillVoid) InstantiateTile(voidTile, i, 1.5f, j);
                        break;
                }
            }
        }
    }

    private GameObject InstantiateTile(GameObject[] prefabs, float xCoord, float yCoord, float zCoord, float xRot = 0, float yRot = 0, float zRot = 0)
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

        return tileInstance;
    }

    private GameObject InstantiateTile(GameObject prefab, float xCoord, float yCoord, float zCoord, float xRot = 0, float yRot = 0, float zRot = 0)
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

        return tileInstance;
    }

    private int NumSurroundingTiles(int x, int y, TileType tile)
    {
        int num = 0;

        if (x > 0 && tiles[x - 1][y] == tile) num++;
        if (x < columns && tiles[x + 1][y] == tile) num++;
        if (y > 0 && tiles[x][y - 1] == tile) num++;
        if (y < rows && tiles[x][y + 1] == tile) num++;

        return num;
    }
}
