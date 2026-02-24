using System.Collections.Generic;
using UnityEngine;
//test class used for ai pathfinding

public class TileHelper : MonoBehaviour
{
    public static int X_SIZE = 16;
    public static int Y_SIZE = 16;
    TileManager tileManager;

    public void Start()
    {   
        tileManager = FindFirstObjectByType<TileManager>();
    }

    //changes a 0,0 Vector3Int to the grid layout
    public static Vector3Int ZerotoGrid(Vector3Int input)
    {

        input[0] = input[0] - (X_SIZE / 2) + 1;
        input[1] = input[1] - (Y_SIZE / 2) + 1;

        return input;
    }

    //changes a grid Vector3Int to the 0,0 layout
    public static Vector3Int GridtoZero(Vector3Int input)
    {

        input[0] = input[0] + (X_SIZE / 2) - 1;
        input[1] = input[1] + (Y_SIZE / 2) - 1;

        return input;
    }

    //returns if the tile is in bounds
    //TODO only for even sizes atm
    public static bool InRange(Vector3Int vect)
    {

        if(vect[0] > (X_SIZE / 2) || vect[0] < (X_SIZE / 2 - X_SIZE + 1))
        {
            return false;
        }
        if(vect[1] > (Y_SIZE / 2) || vect[1] < (Y_SIZE / 2 - Y_SIZE + 1))
        {
            return false;
        }
        return true;
    }

    //returns start to end vector of vectors
    public List<Vector3Int> TilePath(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> ret = new List<Vector3Int>();

        //Debug.Log("Start Pos: " + start + " | End Pos: " + end);
        //check to make sure it's in range
        if(!InRange(start) || !InRange(end))
        {
            ret.Add(Vector3Int.zero);
            Debug.Log("Not in range");
            return ret;
        }

        //check to make sure they aren't equal
        if(start == end)
        {
            ret.Add(start);
            Debug.Log("Path is equal");
            return ret;
        }

        //make the open and closed lists
        List<Node> openList = new List<Node>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        //add the start node to the open list
        Node startNode = new Node(start);
        startNode.SetH(end);
        startNode.SetF();
        openList.Add(startNode);

        //keep looping until the list is empty
        while(openList.Count > 0)
        {
            //sort openList by f
            openList.Sort((a, b) => a.f.CompareTo(b.f));
            Node currentNode = openList[0];

            //check if the current node is the end
            if(currentNode.location == end)
            {
                //if it is get the return ready
                Node node = currentNode;
                while (node != null)
                {
                    ret.Insert(0, node.location);
                    node = node.parent;
                }
                //Debug.Log("Path Of Length: " + ret.Count);
                //for(int i = 0; i < ret.Count; i++)
                //{
                //    Debug.Log(ret[i]);
                //}
                return ret;
            }

            //if it isnt send it to the closed list
            openList.RemoveAt(0);
            closedSet.Add(currentNode.location);

            //check the 4 directions TODO diagonal?
            Vector3Int[] neighbors = new Vector3Int[] 
            {
                currentNode.location + Vector3Int.up,
                currentNode.location + Vector3Int.down,
                currentNode.location + Vector3Int.left,
                currentNode.location + Vector3Int.right
            };

            foreach(var neighborPos in neighbors)
            {
                if(!InRange(neighborPos) || closedSet.Contains(neighborPos))
                {
                    continue;
                }

                //if tile is not walkable, skip
                var tileData = tileManager.GetTileDataAt(neighborPos);
                if(tileData == null || (!tileData.CanEnter() && neighborPos != end))
                {
                    continue;
                }
                int gCost = currentNode.g + tileData.movementCost;

                Node existingNode = openList.Find(n => n.location == neighborPos);

                if(existingNode == null)
                {
                    Node neighborNode = new Node(neighborPos, currentNode);
                    neighborNode.SetG(gCost);
                    neighborNode.SetH(end);
                    openList.Add(neighborNode);
                }
                else if(gCost < existingNode.g)
                {
                    //better path found to neighbor
                    existingNode.parent = currentNode;
                    existingNode.SetG(gCost);
                }
            }
        }

        // No path
        ret.Add(Vector3Int.zero);
        Debug.Log("No path");
        return ret;
    }
};

public class Node
{
    public Vector3Int location;
    public int f;
    public int g;
    public int h;
    public Node parent;

    public Node(Vector3Int loc, Node parent = null)
    {
        this.location = loc;
        this.parent = parent;
        g = 0;
        h = 0;
        f = 0;
    }

    public void SetF()
    {
        f = g + h;
    }

    public void SetG(int temp)
    {
        g = temp;
        SetF();
    }

    public void SetH(Vector3Int end)
    {
        h = CalcH(end);
        SetF();
    }

    public int CalcH(Vector3Int end)
    {
        Vector3Int start = TileHelper.GridtoZero(location);
        end = TileHelper.GridtoZero(end);
        return Mathf.Abs(start[0] - end[0]) + Mathf.Abs(location[1] - end[1]);
    }
};