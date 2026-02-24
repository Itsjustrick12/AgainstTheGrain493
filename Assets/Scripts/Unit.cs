using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : Entity
{
    [Header("General Settings")]
    public int ID;
    //added for targeting
    public List<EntityType> primary = new List<EntityType>();
    //no target uses a negative z value
    public Vector3Int target = new Vector3Int(0,0,-1);
    public bool isEnemy = false;

    [Header("Stats")]
    public int strength = 1;
    //Only increase for ranged attackers
    public int attackRange = 1;
    public int iq = 1;
    public int movementRange = 3;

    //Hidden logic for determining what a unit is able to do, define by the unit database
    private List<UnitAction> actions = new();

    public override void Awake()
    {
        base.Awake();
        InitializeActions();
    }

    public void InitializeActions(List<UnitAction> unitActions)
    {
        actions = unitActions;
    }

    public void InitializeActions()
    {
        //This references the action set defined in the unit database
        actions = UnitDatabase.Instance.GetActions(ID);
    }

    public List<UnitAction> GetAvailableActions()
    {
        foreach (var action in actions)
        {
            Debug.Log($"Action: {action.GetName()} | IsPossible: {action.IsPossible(this)}");
        }
        //Return all the actions that are currently possible given the Unit's information (and generally position)
        return actions.Where(action => action.IsPossible(this)).ToList();
    }

    public void GetHealth(int healthValue){ 
        currentHealth = healthValue;
    }

    public int GetAttackRange()
    {
        return attackRange;
    }

    public bool IsSameTeamAs(Unit diffUnit)
    {
        if (diffUnit == null)
        {
            return false;
        }

        //Check if same side, either both enemies or both not
        if ((isEnemy && diffUnit.isEnemy) || (!isEnemy && !diffUnit.isEnemy))
        {
            return true;
        }
        //Otherwise, different teams
        return false;
    }

    public bool IsSameTeamAs(Entity entity)
    {
        if (entity == null)
        {
            Debug.LogError("The passed entity is null, can't compare teams");
            return false;
        }

        //Cast entity to unit
        Unit unitCheck = entity as Unit;


        return IsSameTeamAs(unitCheck);
    }

    public int GetStrength()
    {
        return strength;
    }

    public void SetStrength(int strengthValue)
    {
        strength = strengthValue;
    }

    //gets vector3Int List for best target based on difficulty
    public void SetTarget()
    {
        
        //choose target based on iq
        if (iq == 1)
        {
            target = FindEasy();
        }
        else if (iq < 5)
        {
            target = FindMedium();
        }
        else
        {
            target = FindHard();
        }

        // Still nothing, clear target
        if (target.z == -1)
        {
            Debug.Log("No Pathable Primary/Any Target found");
            return;
        }
    }

    public Vector3Int GetTarget()
    {
        return target;
    }

    public Vector3Int SetAndReturnTarget()
    {
        SetTarget();
        return GetTarget();
    }

    List<Vector3Int> FindPositions(bool prime)
    {
        List<Vector3Int> temp = new List<Vector3Int>();
        List<Unit> tempUnits;

        //grabs the list depending on team
        if (isEnemy)
        {
            tempUnits = gameManager.GetAllFriendlyUnits();
        }
        else
        {
            tempUnits = gameManager.GetAllEnemyUnits();
        }

        //gets all of the entities to be targeted
        for(int i = 0; i < tempUnits.Count; i++)
        {
            //only grabs an entity if it's a primary target or secondary targeting
            if(primary.Contains(tempUnits[i].GetEntityType()) || !prime)
            {
                temp.Add(tempUnits[i].GetGridPos());
            }
        }

        //only grabs crops if they're a primary target or secondary targeting
        if(primary.Contains(EntityType.Crop) || !prime)
        {
            List<Crop> tempCrops = gameManager.GetAllCrops();
            for(int i = 0; i < tempCrops.Count; i++)
            {
                temp.Add(tempCrops[i].GetGridPos());
            }
        }

        return temp;
    }

    /*
        targeting for "Easy" enemies
        
        targets clostest primary target

        if there is no targetable primary target it chooses the closest unit
    */
    Vector3Int FindEasy()
    {
        //gets all primary targets
        List<Vector3Int> targets = FindPositions(true);

        Vector3Int best = new Vector3Int(0, 0, -1);
        int bestTTR = int.MaxValue;

        foreach (Vector3Int t in targets)
        {
            //get the path for the next potential target
            List<Vector3Int> path = tileHelper.TilePath(GetGridPos(), t);

            // Skip paths that return no path (only 1 item in list)
            if (path.Count == 1)
                continue;

            //uses turnstoreach to find the "best" target
            int ttr = TurnsToReach(path);
            if (ttr < bestTTR)
            {
                bestTTR = ttr;
                best = t;
            }
        }

        // if an accesible primary target was found return
        if(best.z != -1)
        {
            return best;
        }

        //now we look for any target
        targets = FindPositions(false);

        foreach (Vector3Int t in targets)
        {
            //get the path for the next potential target
            List<Vector3Int> path = tileHelper.TilePath(GetGridPos(), t);

            // Skip paths that return no path (only 1 item in list)
            if (path.Count == 1)
                continue;

            //uses turnstoreach to find the "best" target
            int ttr = TurnsToReach(path);
            if (ttr < bestTTR)
            {
                bestTTR = ttr;
                best = t;
            }
        }

        return best;
    }

    /*
        targeting for "Medium" enemies

        targets closest primary target
        or
        secondary target that takes -1 turn to kill
    */
    Vector3Int FindMedium()
    {
        //get list of all targets
        List<Vector3Int> targets = FindPositions(false);

        Vector3Int best = new Vector3Int(0, 0, -1);
        int bestTTK = int.MaxValue;

        foreach (Vector3Int t in targets)
        {
            //get the path for the next potential target
            List<Vector3Int> path = tileHelper.TilePath(GetGridPos(), t);

            // Skip paths that return no path (only 1 item in list)
            if (path.Count == 1)
                continue;

            //uses turnstokill to find the "best" target
            int ttk = TurnsToKill(path);

            //if the potential target is a secondary target add 1 to the ttk
            if(!primary.Contains(tileManager.GetTileDataAt(t).GetOccupyingEntity().GetEntityType()))
            {
                ttk++;
            }
            if (ttk < bestTTK)
            {
                bestTTK = ttk;
                best = t;
            }
        }

        return best;
    }

    //TODO
    Vector3Int FindHard()
    {
        return FindMedium();

    }

    //calculation for amount of turn to kill a unit, used for priority
    int TurnsToKill(List<Vector3Int> path)
    {
        int ttk = 0;

        //turns to get(next) to target
        ttk += TurnsToReach(path);

        //checks the last spot in the path to make sure there is actually a target
        if(tileManager.GetTileDataAt(path[path.Count - 1]).occupyingEntity != null)
        {
            Entity temptarget = tileManager.GetTileDataAt(path[path.Count - 1]).occupyingEntity;

            //so since temptarget can be attacked on the move turn you subtrace 1 turn from
            //the amount of turns required to kill target
            ttk += temptarget.GetHealth() / this.GetStrength() - 1;

            //if there is leftover it adds another turn
            if(temptarget.GetHealth() % this.GetStrength() > 0)
            {
                ttk++;
            }
        }

        return ttk;
        
    }

    //turns to get(next) to target
    int TurnsToReach(List<Vector3Int> path)
    {
        int ttr = 0;

        if(path.Count > 2)
        {
            for(int i = 1; i < path.Count - 1; i++)
            {
                ttr += tileManager.GetTileDataAt(path[i]).movementCost;
            }

            ttr = ttr / this.movementRange + 1;
        }

        return ttr;
    }

    bool Win1v1()
    {
        bool ret = false;
        /*if(tileGetTileDataAt(path[i]))
        {

        }*/
        return ret;
    }

    void Move(List<Vector3Int> path)
    {
        //make sure we dont pass the movement amount of the unit
        int tempMovement = movementRange;

        if(GetGridPos() == path[0])
        {
            path.RemoveAt(0);
        }
        while(tempMovement > 0 && path.Count > 1)
        {
            //if there is nothing in the next tile
            if(tileManager.GetTileDataAt(path[0]).occupyingEntity == null)
            {
                //if we have enough movement left
                if(tempMovement >= tileManager.GetTileDataAt(path[0]).movementCost)
                {
                    //Debug.Log("Move to " + path[0]);
                    //set our grid position to the next tile
                    tileManager.MoveEntity(GetGridPos(), path[0]);
                    //change amount of movement left
                    tempMovement -= tileManager.GetTileDataAt(path[0]).movementCost;
                    //remove the current tile from the path
                    path.RemoveAt(0);
                }
                else
                {
                    //Debug.Log("Not enough movement");
                    tempMovement = 0;
                }
            }
            else
            {
                //Debug.Log("Entity in next tile");
                tempMovement = 0;
            }
        }
    }

    void Attack()
    {
        if (target.z == -1)
        {
            Debug.Log("No target!");
            return;
        }


        Vector3Int pos = GetGridPos();
        bool isAdjacent = target == pos + Vector3Int.up ||
                          target == pos + Vector3Int.down ||
                          target == pos + Vector3Int.left ||
                          target == pos + Vector3Int.right;

        if (!isAdjacent)
        {
            Debug.Log("Target isn't adjacent!");
            //Set the target a second time
            return;
        }

        TileData targetTile = tileManager.GetTileDataAt(target);
        if (targetTile == null || targetTile.occupyingEntity == null)
        {
            Debug.Log("Nothing to attack!");
            return;
        }

        targetTile.occupyingEntity.TakeDamage(GetStrength());

        if (targetTile.occupyingEntity == null)
            target = new Vector3Int(0, 0, -1);
    }

    public void DoTurn()
    {
        //if a target hasn't been set we find the next target

        //Debug.Log("Finding Target");
        this.SetTarget();

        //See if our target is up to date (needed for concurrent enemy execution)
        TileData data = tileManager.GetTileDataAt(target);
        if (data != null && data.HasUnit())
        {
            Unit unitCheck = data.GetOccupyingEntity() as Unit;
            if (unitCheck && IsSameTeamAs(unitCheck))
            {
                SetTarget();
            }
        }
        else
        {
            //Get a new target if our old one is outdata
            SetTarget();
        }


        //if we found a target we move to it
        if (target.z != -1)
        {
            //Debug.Log("Target found at " + target);
            List<Vector3Int> path = tileHelper.TilePath(GetGridPos(), target);
            if (path.Count > 2)
            {
                Move(path);
            }
            else
            {
                Debug.Log("No Need to Move!");
            }
        }

        //then we attack the target
        Attack();
    }

}

