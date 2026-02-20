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

    void GetStrength(int strengthValue)
    {
        strength = strengthValue;
    }

    //gets vector3Int List for best target based on difficulty
    public void SetTarget()
    {

        List<Vector3Int> tempList = FindPositions(true);
        //look for primary target on proper difficulty
        if(this.iq < 2)
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
        //look for any target on proper difficulty
    }

    List<Vector3Int> FindPositions(bool prime)
    {
        List<Vector3Int> temp = new List<Vector3Int>();
        List<Unit> tempUnits;
        if (isEnemy)
        {

            tempUnits = gameManager.GetAllFriendlyUnits();
        }
        else {
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

    //TODO
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

    public Vector3Int GetCurrentTarget()
    {
        return target;
    }

    public Vector3Int SetAndReturnTarget()
    {
        SetTarget();
        return GetCurrentTarget();
    }

    //TODO
    void FindMedium(List<Vector3Int> targets)
    {
        for(int i = 0; i < targets.Count; i++)
        {
            
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

    

}

