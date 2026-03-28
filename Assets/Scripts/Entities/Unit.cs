using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : Entity
{
    [Header("General Settings")]
    public int ID;
    public int strength = 1;
    public int attackRange = 1;
    public int movementRange = 3;
    public bool isFed = false;
    public bool isEnemy = false;
    public bool canFly = false;

    [Header("For Targeting")]
    //added for targeting
    public List<EntityType> primary = new List<EntityType>();
    //no target uses a negative z value
    public Vector3Int target = new Vector3Int(0,0,-1);
    public int iq = 1;

    public override void Awake()
    {
        base.Awake();
        
    }

    public override void Initialize()
    {
        base.Initialize();
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(ID);
        attackRange = info.attackRange;
        strength = info.strength;
        maxHealth = info.baseHealth;
        currentHealth = info.baseHealth;
        actions = info.actions;
        movementRange = info.moveRange;
    }

    public void SetIsFed(bool value)
    {
        isFed = value;
    }

    public bool GetIsFed()
    {
        return isFed;
    }

    public int GetMoveRange()
    {
        return movementRange;
    }

    //This is dumb change this later
    public bool CanAttack()
    {
        foreach (var action in actions)
        {
            if (action.GetName() == "Attack")
            {
                return true;
            }
        }
        return false;
    }

    public void SetAttackRange(int temp)
    {
        if(temp > 0)
        {
            attackRange = temp;
        }
    }

    public void Heal(int amount)
    {
        int after = currentHealth + amount;
        currentHealth = (after) > maxHealth ? maxHealth : after;
    }

    public int GetAttackRange()
    {
        return attackRange;
    }

    public void SetCanFly(bool temp)
    { 
        canFly = temp;
    }

    public bool GetCanFly()
    { 
        return canFly;
    }

    public void SetIsEnemy(bool tempIsEnemy)
    {
        isEnemy = tempIsEnemy;
    }

    public bool GetIsEnemy()
    {
        return isEnemy;
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
            Debug.LogError("UNIT.The passed entity is null, can't compare teams");
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

    public Vector3Int GetTarget()
    {
        return target;
    }

    public Vector3Int SetAndReturnTarget()
    {
        aiManager.FindTarget(this);
        return GetTarget();
    }

    public List<Vector3Int> GetMovementRange()
    {
        return tileHelper.GetMovementRange(this);
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

    void Move(List<Vector3Int> path)
    {
        //make sure we dont pass the movement amount of the unit
        int tempMovement = movementRange;

        if(GetGridPos() == path[0])
        {
            path.RemoveAt(0);
        }
        while(tempMovement > 0 && path.Count > 0)
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
                    Debug.Log("Not enough movement");
                    tempMovement = 0;
                }
            }
            else
            {
                Debug.Log("Entity in next tile");
                tempMovement = 0;
            }
        }

    }

    void Attack()
    {
        if (target.z == -1)
        {
            Debug.Log("UNIT.No target!");
            return;
        }


        Vector3Int pos = GetGridPos();
        bool isAdjacent = Mathf.Abs(target.x - pos.x) + Mathf.Abs(target.y - pos.y) <= GetAttackRange();

        if (!isAdjacent)
        {
            Debug.Log("Target isn't adjacent!");
            //Set the target a second time
            return;
        }

        TileData targetTile = tileManager.GetTileDataAt(target);
        if (targetTile == null || targetTile.occupyingEntity == null)
        {
            Debug.Log("UNIT.Nothing to attack!");
            return;
        }

        targetTile.occupyingEntity.TakeDamage(GetStrength());

        if (targetTile.occupyingEntity == null)
            target = new Vector3Int(0, 0, -1);
    }

    public void DoTurn()
    {
        //Debug.Log("Finding Target");
        target = aiManager.FindTarget(this);

        //See if our target is up to date (needed for concurrent enemy execution)
        TileData data = tileManager.GetTileDataAt(target);
        if (data != null && data.HasUnit())
        {
            Unit unitCheck = data.GetOccupyingEntity() as Unit;
            if (unitCheck && IsSameTeamAs(unitCheck))
            {
                target = aiManager.FindTarget(this);
            }
        }
        else
        {
            //Get a new target if our old one is outdata
            target = aiManager.FindTarget(this);
        }

        //Debug.Log("UNIT.Found Target: " + target.x + " " + target.y + " " + target.z);
        //if we found a target we move to it
        if (target.z != -1)
        {
            //Debug.Log("UNIT.Target found at " + target);
            List<Vector3Int> path = tileHelper.TilePath(GetGridPos(), target, this);
            //Debug.Log("UNIT.Distance = " + path.Count);
            if (path.Count > 0)
            {
                Move(path);
            }
            else
            {
                Debug.Log("UNIT.No Need to Move!");
            }
        }

        //then we attack the target
        Attack();
    }

    public override void Die()
    {
        SoundManager.Instance.PlayEntitySound(this, SoundType.DEATH);
        base.Die();
    }

    public override void TakeDamage(int damage)
    {
        SoundManager.Instance.PlayEntitySound(this, SoundType.HURT);
        base.TakeDamage(damage);
    }
}

