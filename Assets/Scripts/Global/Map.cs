using UnityEngine;

/*
 * TODO: Turn this into the data type used to store and generate the map (not just a purely static instance)
 */
public static class Map
{
    public enum TileType
    {
        Wall,
        Floor,
        Empty,
    }

    private static TileType[][] board;

    public static void setBoard(TileType[][] b)
    {
        board = b;
    }

    public static Vector3 GetLegalPosition()
    {
        int x = Random.Range(0, board.Length);
        int z = Random.Range(0, board[x].Length);

        while (board[x][z] != Map.TileType.Floor)
        {
            x = Random.Range(0, board.Length);
            z = Random.Range(0, board[x].Length);
        }

        return new Vector3(x, 0.5f, z);
    }


    public static bool IsLegalPos(Vector3 pos)
    {
        return board[(int)pos.x][(int)pos.z] == Map.TileType.Floor;
    }
}
