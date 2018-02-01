using UnityEngine;

public enum Direction
{
    North,
    East,
    South,
    West,
}

public class Corridor
{
    public int startXPos;           // The x coordinate of the start of the corridor
    public int startZPos;           // The z coordinate of the start of the corridor
    public int corridorLength;      // The length of the corridor
    public Direction direction;     // The direction which the corridor is heading for its room

    // Get the end position of the corridor based on its start position and which direction it's heading.
    public int EndPositionX
    {
        get
        {
            if (direction == Direction.North || direction == Direction.South)
                return startXPos;
            if (direction == Direction.East)
                return startXPos + corridorLength - 1;
            return startXPos - corridorLength + 1;
        }
    }


    public int EndPositionZ
    {
        get
        {
            if (direction == Direction.East || direction == Direction.West)
                return startZPos;
            if (direction == Direction.North)
                return startZPos + corridorLength - 1;
            return startZPos - corridorLength + 1;
        }
    }


    public void setupCorridor(Room room, RandInt length, RandInt roomWidth, RandInt roomHeight, int columns, int rows, bool firstCorridor)
    {
        // Set a random direction
        direction = (Direction)Random.Range(0, 4);

        // Calculate the opposite direction
        Direction oppositeDirection = (Direction)(((int)room.enteringCorridor + 2) % 4);

        if (!firstCorridor && direction == oppositeDirection)
        {
            int newDir = (int)direction;
            newDir = (newDir + 1) % 4;
            direction = (Direction)newDir;
        }

        // Generate random length
        corridorLength = length.Random;

        // Create a max for how long the length can be
        int maxLength = length.m_Max;

        switch (direction)
        {
            case Direction.North:
                startXPos = Random.Range(room.xPos, room.xPos + room.roomWidth - 1);
                startZPos = room.zPos + room.roomHeight;
                maxLength = rows - startZPos - roomHeight.m_Min;
                break;
            case Direction.East:
                startXPos = room.xPos + room.roomWidth;
                startZPos = Random.Range(room.zPos, room.zPos + room.roomHeight - 1);
                maxLength = columns - startXPos - roomWidth.m_Min;
                break;
            case Direction.South:
                startXPos = Random.Range(room.xPos, room.xPos + room.roomWidth);
                startZPos = room.zPos;
                maxLength = startZPos - roomHeight.m_Min;
                break;
            case Direction.West:
                startXPos = room.xPos;
                startZPos = Random.Range(room.zPos, room.zPos + room.roomHeight);
                maxLength = startXPos - roomWidth.m_Min;
                break;
        }

        // Clamp the length of the corridor to make sure it doesn't go out of bounds
        corridorLength = Mathf.Clamp(corridorLength, 1, maxLength);
    }
}
