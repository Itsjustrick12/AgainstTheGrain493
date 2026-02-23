using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Entity
{
    [Header("General Settings")]
    public int ID;
    public bool isActive = false;
    //added for targeting
    public List<EntityType> primary = new List<EntityType>();
    //no target uses a negative z value
    public Vector3Int target = new Vector3Int(0,0,-1);
    public bool isEnemy = false;

    [Header("Stats")]
    public int strength = 1;
    public int movementRange = 3;
    public int iq = 1;

    int GetStrength()
    {
        return strength;
    }

    void SetStrength(int strengthValue)
    {
        strength = strengthValue;
    }

    //gets vector3Int List for best target based on difficulty
    public void SetTarget()
    {

        List<Vector3Int> tempList = FindPositions(true);

        //if no primary targets look at all targets
        if(tempList.Count < 1)
        {
            tempList = FindPositions(false);
        }

        if(tempList.Count > 0)
        {
            //find target target based on difficulty
            if(true)
            {
                FindEasy(tempList);
            }
            else if(this.iq < 5)
            {
                FindMedium(tempList);
            }
            else
            {
                FindHard(tempList);
            }
        }
        else
        {
            this.target = new Vector3Int(0,0,-1);
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

        if (isEnemy)
        {
            tempUnits = gameManager.GetAllFriendlyUnits();
        }
        else
        {
            tempUnits = gameManager.GetAllEnemyUnits();
        }

        for(int i = 0; i < tempUnits.Count; i++)
        {
            if(primary.Contains(tempUnits[i].GetEntityType()) || !prime)
            {
                temp.Add(tempUnits[i].GetGridPos());
            }
        }

        if(primary.Contains(EntityType.Crop) || !prime)
        {
            List<Crop> tempCrops = gameManager.GetAllCrops();
            for(int i = 0; i < tempUnits.Count; i++)
            {
                temp.Add(tempCrops[i].GetGridPos());
            }
        }

        return temp;
    }

    void FindEasy(List<Vector3Int> targets)
    {
        int ttr = 999;
        for(int i = 0; i < targets.Count; i++)
        {
            if(TurnsToReach(tileHelper.TilePath(this.GetGridPos(), targets[i])) < ttr)
            {
                ttr = TurnsToReach(tileHelper.TilePath(this.GetGridPos(), targets[i]));
                target = targets[i];
            }
        }
    }

    //TODO
    void FindMedium(List<Vector3Int> targets)
    {
        int ttk = 999;
        for(int i = 0; i < targets.Count; i++)
        {
            if(TurnsToKill(tileHelper.TilePath(this.GetGridPos(), targets[i])) < ttk)
            {
                ttk = TurnsToKill(tileHelper.TilePath(this.GetGridPos(), targets[i]));
                target = targets[i];
            }
        }

    }

    //TODO
    void FindHard(List<Vector3Int> targets)
    {
        for(int i = 0; i < targets.Count; i++)
        {
            
        }

    }

    //calculation for amount of turn to kill a unit, used for priority
    int TurnsToKill(List<Vector3Int> path)
    {
        int ttk = 0;

        //turns to get(next) to target
        ttk += TurnsToReach(path);

        //turn to kill target
        if(tileManager.GetTileDataAt(path[path.Count]).occupyingEntity != null)
        {
            Entity temptarget = tileManager.GetTileDataAt(path[path.Count]).occupyingEntity;
            ttk += temptarget.GetHealth() / this.GetStrength();
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
        while(tempMovement > 0 && path.Count > 0)
        {
            //if there is nothing in the next tile
            if(tileManager.GetTileDataAt(path[0]).occupyingEntity == null)
            {
                //if we have enough movement left
                if(tempMovement >= tileManager.GetTileDataAt(path[0]).movementCost)
                {
                    //set our grid position to the next tile
                    this.SetGridPos(path[0]);
                    //change amount of movement left
                    tempMovement -= tileManager.GetTileDataAt(path[0]).movementCost;
                    //remove the current tile from the path
                    path.RemoveAt(0);
                }
                else
                {
                    tempMovement = 0;
                }
            }
            else
            {
                tempMovement = 0;
            }
        }
    }

    void Attack()
    {
        //check to see if the target is next to us
        if(target == this.GetGridPos() + Vector3Int.up || target == this.GetGridPos() + Vector3Int.down || target == this.GetGridPos() + Vector3Int.left || target == this.GetGridPos() + Vector3Int.right && target.z != -1)
        {
            //we make sure the target tile has an entity
            if(tileManager.GetTileDataAt(target).occupyingEntity != null)
            {
                //we then attack the target
                tileManager.GetTileDataAt(target).occupyingEntity.TakeDamage(this.GetStrength());

                //if we kill the target
                if(tileManager.GetTileDataAt(target).occupyingEntity == null)
                {
                    //we then set target
                    this.SetTarget();
                }
            }
        }
    }

    public void DoTurn()
    {
        //if a target hasn't been set we find the next target
        if(this.GetGridPos().z != -1)
        {
            this.SetTarget();
        }

        //if we found a target we move to it
        if(this.GetGridPos().z != -1)
        {
            Move(tileHelper.TilePath(this.GetGridPos(), target));
        }

        //then we attack the target
        Attack();
    }

}

