using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPath {
    private class Node
    {
        public int x;
        public int y;

        public float f_score;
        public float g_score;
        public Node camefrom;
    }

	public Vector2 GetBestMove(Vector3 start, Vector3 target, bool[][] tiles)
    {
        Vector2 dir = new Vector2();



        return dir;
    }

    private List<Vector2> Astar(Vector2 start, Vector2 end, bool[][] tiles)
    {
        List<Node> closedSet = new List<Node>();
        List<Node> openSet = new List<Node>();
        //openSet.Add(start);


        return null;
    }
}
