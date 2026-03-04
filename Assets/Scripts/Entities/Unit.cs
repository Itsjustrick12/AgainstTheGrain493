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
    public bool canFly = false;

    [Header("Stats")]
    public int strength = 1;
    //Only increase for ranged attackers
    public int attackRange = 1;
    public int iq = 1;
    public int movementSpeed = 3;

    public override void Awake()
    {
        base.Awake();
        InitializeActions();
    }

    public void InitializeActions()
    {
        //This references the action set defined in the unit database
        actions = UnitDatabase.Instance.GetActions(ID);
    }

    //attack range can't be less than 1
    public void SetAttackRange(int tempAttackRange)
    {
        if(tempAttackRange < 1)
        {
            attackRange = 1;
        }
        else
        {
            attackRange = tempAttackRange;
        }
    }

    public int GetAttackRange()
    {
        return attackRange;
    }

    //movement speed can't be less than 0(can't move)
    public void SetMovementSpeed(int tempMovementSpeed)
    {
        if(tempMovementSpeed < 0)
        {
            movementSpeed = 0;
        }
        else
        {
            movementSpeed = tempMovementSpeed;
        }
    }

    public int GetMovementSpeed()
    {
        return movementSpeed;
    }

    public void SetCanFly(bool temp)
    {
        canFly = temp;
    }

    public bool GetCanFly()
    {
        return canFly;
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
        Debug.Log("Test 1");
        target = aiManager.FindTarget(this);
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

    public List<Vector3Int> GetMovementRange()
    {
        return tileHelper.GetMovementRange(this);
    }

    /*
        given a list of Vector3Int, the Move will move as far as possible down the list
        consraints:
        can't end on a occupied tile
        can't move past unit's move range

    */
    void Move(List<Vector3Int> path)
    {
        //first we shorten the path until the last tile is the max movement speed for the unit
        int tempMovement = movementSpeed;
        /*
        int maxPosition = 0;
        while(maxPosition == path.Count - 1)
        {
            
        }

        while(tileManager.GetTileDataAt(path[path.Count]).GetOccupyingEntity() == null)
        {
            path.RemoveAt(path.Count - 1);
        }
        */
        //make sure we dont pass the movement amount of the unit
        tempMovement = movementSpeed;

        if(GetGridPos() == path[0])
        {
            path.RemoveAt(0);
        }

        while(tempMovement > 0 && path.Count > 0)
        {
            //if there is nothing in the next tile
            if(tileManager.GetTileDataAt(path[0]).GetOccupyingEntity() == null)
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

        if (!CanAttack())
        {
            //Debug.Log("Target isn't adjacent!");
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

    //returns true if the unit can target the enemy
    public bool CanAttack()
    {
        if(Mathf.Abs(target.x - GetGridPos().x) + Mathf.Abs(target.y - GetGridPos().y) <= attackRange)
        {
            return true;
        }
        
        return false;
    }

    public void DoTurn()
    {
        //if a target hasn't been set we find the next target

        //Debug.Log("Finding Target");
        SetTarget();

        //if we found a target we move to it
        if (target.z != -1)
        {
            //Debug.Log("Target found at " + target);
            List<Vector3Int> path = tileHelper.TilePath(GetGridPos(), target, this);
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

