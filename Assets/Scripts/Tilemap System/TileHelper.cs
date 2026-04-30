using NUnit.Framework.Internal.Commands;
using System.Collections.Generic;
using UnityEngine;
//test class used for ai pathfinding

public class TileHelper : MonoBehaviour
{
    public static TileHelper Instance { get; private set; }
    public int X_SIZE = 16;
    public int Y_SIZE = 16;
    TileManager tileManager;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Start()
    {   
        tileManager = FindFirstObjectByType<TileManager>();
        X_SIZE = GameConstants.MapSizeToInt(GameManager.Instance.mapSize);
        Y_SIZE = X_SIZE;
    }

    //changes a 0,0 Vector3Int to the grid layout


    //changes a grid Vector3Int to the 0,0 layout
    public Vector3Int GridtoZero(Vector3Int input)
    {

        input[0] = input[0] + (X_SIZE / 2) - 1;
        input[1] = input[1] + (Y_SIZE / 2) - 1;

        return input;
    }

    //returns if the tile is in bounds
    //TODO only for even sizes atm
    public bool InRange(Vector3Int vect)
    {

        if(vect[0] > (X_SIZE / 2 - 1) || vect[0] < (X_SIZE / 2 - X_SIZE ))
        {
            return false;
        }
        if(vect[1] > (Y_SIZE / 2 - 1) || vect[1] < (Y_SIZE / 2 - Y_SIZE ))
        {
            return false;
        }
        return true;
    }

    public bool IsWithinRange(Vector3Int a, Vector3Int b, int range)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) <= range;
    }

    //returns start to end vector of vectors
    public List<Vector3Int> TilePath(Vector3Int start, Vector3Int end, Unit unit)
    {
        int isEnemy = -1;
        bool canFly = false;
        if(unit != null)
        {
            if(unit.GetIsEnemy() == false)
            {
                isEnemy = 0;
            }
            else
            {
                isEnemy = 1;
            }
            canFly = unit.GetCanFly();
        }
        return TilePath(start, end, isEnemy, canFly);
    }

    public List<Vector3Int> TilePath(Vector3Int start, Vector3Int end, int isEnemy, bool canFly)
    {
        List<Vector3Int> ret = new List<Vector3Int>();

        //Debug.Log("Start Pos: " + start + " | End Pos: " + end);
        //check to make sure it's in range
        if(!InRange(start) || !InRange(end))
        {
            Debug.Log("Not in range");
            return ret;
        }

        //check to make sure they aren't equal
        if(start == end)
        {
            ret.Add(start);
            Debug.Log("Start = End");
            return ret;
        }

        //if start is directly next to end, allow pathing
        if(Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y) == 1)
        {
            ret.Add(start);
            ret.Add(end);
            return ret;
        }

        /*
        //can't end on unit
        if(IsOccupied(end))
        {
            ret.Add(Vector3Int.zero);
            Debug.Log("End tile is occupied");
            return ret;
        }
        */

        List<Vector3Int> candidateEnds = new List<Vector3Int>();
        candidateEnds.Add(end);
        candidateEnds.Add(end + Vector3Int.left);
        candidateEnds.Add(end + Vector3Int.right);
        candidateEnds.Add(end + Vector3Int.up);
        candidateEnds.Add(end + Vector3Int.down);

        List<Vector3Int> bestFallbackPath = new List<Vector3Int>();

        for(int c = 0; c < candidateEnds.Count; c++)
        {
            Vector3Int candidateEnd = candidateEnds[c];

            if(!InRange(candidateEnd))
            {
                continue;
            }

            TileData candidateTile = tileManager.GetTileDataAt(candidateEnd);
            if(candidateTile == null || !candidateTile.CanEnter())
            {
                continue;
            }

            // can't end on any occupied tile
            if(candidateTile.GetOccupyingEntity() != null)
            {
                continue;
            }

            List<Node> openList = new List<Node>();
            HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

            Node startNode = new Node(start);
            startNode.SetG(0);
            startNode.SetH(candidateEnd);
            startNode.SetF();
            openList.Add(startNode);

            while(openList.Count > 0)
            {
                openList.Sort((a, b) => a.f.CompareTo(b.f));
                Node currentNode = openList[0];

                if(currentNode.location == candidateEnd)
                {
                    List<Vector3Int> path = new List<Vector3Int>();
                    Node node = currentNode;
                    while(node != null)
                    {
                        path.Insert(0, node.location);
                        node = node.parent;
                    }

                    //exact end found, return immediately
                    if(candidateEnd == end)
                    {
                        return path;
                    }

                    //otherwise keep shortest adjacent fallback
                    if(bestFallbackPath.Count == 0 || path.Count < bestFallbackPath.Count)
                    {
                        bestFallbackPath = path;
                    }

                    break;
                }

                openList.RemoveAt(0);
                closedSet.Add(currentNode.location);

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

                    var tileData = tileManager.GetTileDataAt(neighborPos);
                    if(tileData == null)
                    {
                        continue;
                    }

                    var occupyingEntity = tileData.GetOccupyingEntity();
                    if(!tileData.CanEnter() && occupyingEntity == null)
                    {
                        continue;
                    }

                    bool isEndTile = (neighborPos == candidateEnd);

                    if(!CanTraverseTile(tileData, neighborPos, isEndTile, isEnemy, canFly))
                    {
                        continue;
                    }

                    int gCost = currentNode.g + tileData.movementCost;

                    Node existingNode = openList.Find(n => n.location == neighborPos);

                    if(existingNode == null)
                    {
                        Node neighborNode = new Node(neighborPos, currentNode);
                        neighborNode.SetG(gCost);
                        neighborNode.SetH(candidateEnd);
                        neighborNode.SetF();
                        openList.Add(neighborNode);
                    }
                    else if(gCost < existingNode.g)
                    {
                        existingNode.parent = currentNode;
                        existingNode.SetG(gCost);
                        existingNode.SetH(candidateEnd);
                        existingNode.SetF();
                    }
                }
            }
        }

        return bestFallbackPath;
    }

    private bool CanTraverseTile(TileData tileData, Vector3Int pos, bool isEnd, int isEnemy, bool canFly)
    {
        Entity occupyingEntity = tileData.GetOccupyingEntity();

        //if there is no entity
        if(occupyingEntity == null)
        {
            return true;
        }

        //if we can fly
        if(canFly)
        {
            return true;
        }
        else
        {
            //if the Entity is a unit
            if(occupyingEntity is Unit)
            {
                Unit occupyingUnit = occupyingEntity as Unit;

                int occupyingFriendly = occupyingUnit.GetIsEnemy() ? 1 : 0;

                // ground units can only move through same-side units
                return occupyingFriendly == isEnemy;
            }
            //if it's an entity only and we can't fly
            return false;
        }

    }


    private bool IsOccupied(Vector3Int pos)
    {
        return tileManager.GetTileDataAt(pos).GetOccupyingEntity() is Unit;
    }

    //----
    //Make a function, public void draw movement range
    //Tkae in Vecotr 3 positon and unit, then see units range of movement and determine all possible movements
    //Output the list of positions, drawn somewhere else

    public List<Vector3Int> GetMovementRange(Unit currentUnit)
    {
        Vector3Int currentPos = currentUnit.GetGridPos();
        int moveAmt = currentUnit.GetMoveRange();
        var validPositions = new List<Vector3Int>();

        for (int i = -moveAmt; i <= moveAmt; i++)
        {
            for (int j = -moveAmt; j <= moveAmt; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) > moveAmt)
                    continue;

                Vector3Int candidateTile = new Vector3Int(currentPos.x + i, currentPos.y + j, 0);

                if (candidateTile == currentPos)
                {
                    validPositions.Add(candidateTile);
                    continue;
                }

                if (!InRange(candidateTile))
                    continue;

                TileData data = tileManager.GetTileDataAt(candidateTile);
                if (data == null || data.HasOccupant())
                    continue;

                var validPath = TilePath(currentPos, candidateTile, currentUnit);

                // Reject null, empty, or paths that didn't actually reach candidateTile
                if (validPath == null || validPath.Count <= 1)
                    continue;
                if (validPath[validPath.Count - 1] != candidateTile)
                    continue;

                // Sum actual movement cost instead of tile count
                int totalCost = 0;
                for (int k = 1; k < validPath.Count; k++)
                {
                    TileData stepData = tileManager.GetTileDataAt(validPath[k]);
                    if (stepData != null)
                        totalCost += stepData.movementCost;
                }

                if (totalCost <= moveAmt)
                    validPositions.Add(candidateTile);
            }
        }

        return validPositions;
    }

    /*
        GetQuickActionRange finds the list of all actions avalible to the unit
        these actions are sent in the list as different z coordinates
            0 = movement action
            1 = move + attack action
            2 = move + water action
            3 = move + harvest action

        
    */
    public List<Vector3Int> GetQuickActionRange(Unit currentUnit)
    {
        Vector3Int currentPos = currentUnit.GetGridPos();
        int moveAmt = currentUnit.GetMoveRange();
        int atkAmt = currentUnit.GetAttackRange();
        var validPositions = new List<Vector3Int>();

        //check all tiles within movement + attack range
        for (int i = -moveAmt - atkAmt; i <= moveAmt + atkAmt; i++)
        {
            for (int j = -moveAmt - atkAmt; j <= moveAmt + atkAmt; j++)
            {
                //set up the candidate tile use in other functions
                Vector3Int candidateTile = new Vector3Int(currentPos.x + i, currentPos.y + j, 0);
                //set up the return tile for actually returning, with -1 being an invalid tile
                Vector3Int returnTile = new Vector3Int(currentPos.x + i, currentPos.y + j, -1);
                //daa at candidatetile location
                TileData data = tileManager.GetTileDataAt(candidateTile);

                //make sure the tile is on the board
                if (!InRange(candidateTile))
                {

                }//check to see if the position is the current position
                else if(i == 0 && j == 0)
                {
                    //if so, return the current position as a movement action
                    returnTile.z = 0;

                }//check to see if the tile is in movement range AND the tile can be occupied
                else if(Mathf.Abs(i) + Mathf.Abs(j) <= moveAmt && !data.HasOccupant())
                {
                    //get the path to the current position
                    var validPath = TilePath(currentPos, candidateTile, currentUnit);
                    if(validPath == null || validPath.Count <= 1 || validPath[validPath.Count - 1] != candidateTile)
                    {
                        continue;
                    }
                    //get the cost of moving to the tile  
                    int totalCost = 0;
                    for (int k = 1; k < validPath.Count; k++)
                    {
                        TileData stepData = tileManager.GetTileDataAt(validPath[k]);
                        if (stepData != null)
                            totalCost += stepData.movementCost;
                    }

                    //if the cost isn't to great, we return the tile as a valid movement tile
                    if (totalCost <= moveAmt)
                    {
                        returnTile.z = 0;
                    }

                }//check to see if the tile is in action range AND has an occupant ir is an enemy(for enemy attack range)
                else if(Mathf.Abs(i) + Mathf.Abs(j) <= moveAmt + atkAmt && (data.HasOccupant() || currentUnit.GetIsEnemy()))
                {
                    //get the path to the current position
                    var validPath = TilePath(currentPos, candidateTile, currentUnit);

                    //remove the occupant tile
                    if(validPath.Count > 0)
                    {
                        validPath.RemoveAt(validPath.Count -1);
                    }
                    
                    //get the cost of moving to the tile
                    int totalCost = 0;
                    for (int k = 1; k < validPath.Count; k++)
                    {
                        TileData stepData = tileManager.GetTileDataAt(validPath[k]);
                        if (stepData != null)
                            totalCost += stepData.movementCost;
                    }

                    //if the cost isn't to great, we look at the occupant
                    if (totalCost <= moveAmt)
                    {
                        //if the currentUnit is an enemy
                        if(currentUnit.GetIsEnemy())
                        {
                            returnTile.z = 1;
                        }//if the currentUnit is a friendly
                        else if(data.GetOccupyingEntity() as Unit != null)
                        {
                            if((data.GetOccupyingEntity() as Unit).GetIsEnemy())
                            {
                                returnTile.z = 1;
                            }
                        }//if the occupying tile is a crop
                        else if(data.GetOccupyingEntity() as Crop != null)
                        {
                            //if the crop can be harvested
                            if((data.GetOccupyingEntity() as Crop).CanBeHarvested())
                            {
                                returnTile.z = 3;
                            }//if the crop can be watered
                            else if(!(data.GetOccupyingEntity() as Crop).IsWatered())
                            {
                                returnTile.z = 2;
                            }
                        }
                    }
                    
                }

                if (returnTile.z != -1)
                        validPositions.Add(returnTile);
                
                
                
                
                
                

                /*if (candidateTile == currentPos)
                {
                    validPositions.Add(candidateTile);
                    continue;
                }

                if (data == null || data.HasOccupant())
                    continue;

                var validPath = TilePath(currentPos, candidateTile, currentUnit);

                // Reject null, empty, or paths that didn't actually reach candidateTile
                if (validPath == null || validPath.Count <= 1)
                    continue;
                if (validPath[validPath.Count - 1] != candidateTile)
                    continue;

                // Sum actual movement cosvt instead of tile count
                int totalCost = 0;
                for (int k = 1; k < validPath.Count; k++)
                {
                    TileData stepData = tileManager.GetTileDataAt(validPath[k]);
                    if (stepData != null)
                        totalCost += stepData.movementCost;
                }

                if (totalCost <= moveAmt)
                    validPositions.Add(candidateTile);*/
            }
        }

        return validPositions;
    }


    //Draw interaction range
    public List<Vector3Int> GetInteractionRange(Unit currentUnit)
    {
        Vector3Int currentPos = currentUnit.GetGridPos();
        //Interaction range is anything that can be reached from the farthest tile you can move to
        int moveAmt = currentUnit.GetMoveRange() + 1;

        var validPositions = new List<Vector3Int>();

        // Loop around the unit within possible movement distance +-moveamt
        for (int i = -moveAmt; i <= moveAmt; i++)
        {
            for (int j = -moveAmt; j <= moveAmt; j++)
            {
                //Skip tiles that are unreachable via the distance
                if (Mathf.Abs(i) + Mathf.Abs(j) > moveAmt)
                    continue;

                //Get a reference to the next possible tile to pathfind to
                Vector3Int candidateTile = new Vector3Int(currentPos.x + i, currentPos.y + j, 0);
                //The starting tile is not a part of the interaction range
                if (candidateTile == currentPos)
                {
                    continue;
                }

                if (!InRange(candidateTile))
                    continue;

            }
        }

        return validPositions;
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
        Vector3Int start = ZerotoGrid(location);
        end = ZerotoGrid(end);
        return Mathf.Abs(start[0] - end[0]) + Mathf.Abs(start[1] - end[1]);
    }

    public Vector3Int ZerotoGrid(Vector3Int input)
    {

        input[0] = input[0] - (TileHelper.Instance.X_SIZE / 2) + 1;
        input[1] = input[1] - (TileHelper.Instance.Y_SIZE / 2) + 1;

        return input;
    }
};