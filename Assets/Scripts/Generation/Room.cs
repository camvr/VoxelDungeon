using UnityEngine;

public class Room
{
    public int xPos;                    // The x coordinate of the lower left tile of the room
    public int zPos;                    // The z coordinate of the lower left tile of the room
    public int roomWidth;               // The width of the room
    public int roomHeight;              // The height of the room
    public Direction enteringCorridor;  // The direction of the corridor entering the room

	public void setupRoom(RandInt widthRange, RandInt heightRange, int columns, int rows)
    {
        // Generate room dimensions
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;

        // Set the x and z coordinates to place the room roughly in the center of the board
        xPos = Mathf.RoundToInt(columns / 2f - roomWidth / 2f) + 1;
        zPos = Mathf.RoundToInt(rows / 2f - roomHeight / 2f) + 1;
    }

    public void setupRoom(RandInt widthRange, RandInt heightRange, int columns, int rows, Corridor corridor)
    {
        enteringCorridor = corridor.direction;

        // Generate room dimensions
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;

        switch (corridor.direction)
        {
            case Direction.North:
                roomHeight = Mathf.Clamp(roomHeight, 1, rows - corridor.EndPositionZ);
                zPos = corridor.EndPositionZ;
                
                xPos = Random.Range(corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);
                xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.East:
                roomWidth = Mathf.Clamp(roomWidth, 1, columns - corridor.EndPositionX);
                xPos = corridor.EndPositionX;

                zPos = Random.Range(corridor.EndPositionZ - roomHeight + 1, corridor.EndPositionZ);
                zPos = Mathf.Clamp(zPos, 0, rows - roomHeight);
                break;
            case Direction.South:
                roomHeight = Mathf.Clamp(roomHeight, 1, corridor.EndPositionZ);
                zPos = corridor.EndPositionZ - roomHeight + 1;

                xPos = Random.Range(corridor.EndPositionX - roomWidth + 1, corridor.EndPositionX);
                xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.West:
                roomWidth = Mathf.Clamp(roomWidth, 1, corridor.EndPositionX);
                xPos = corridor.EndPositionX - roomWidth + 1;

                zPos = Random.Range(corridor.EndPositionZ - roomHeight + 1, corridor.EndPositionZ);
                zPos = Mathf.Clamp(zPos, 0, rows - roomHeight);
                break;
        }

        xPos += 1;
        zPos += 1;
    }

    public Vector2 GetNonBlockingPosition()
    {
        return new Vector2(xPos + Random.Range(1, roomWidth - 1), zPos + Random.Range(1, roomHeight - 1));
    }
}
