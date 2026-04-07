using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : Entity
{
    [Header("General Settings")]
    public int ID;
    [SerializeField] protected int strength = 1;
    [SerializeField] protected int attackRange = 1;
    [SerializeField] protected int movementRange = 3;
    public bool isFed = false;
    public bool isEnemy = false;
    public bool canFly = false;

    [Header("For Targeting")]
    //added for targeting
    public List<EntityType> primary = new List<EntityType>();
    //no target uses a negative z value
    public Vector3Int target = new Vector3Int(0,0,-1);
    public int iq = 1;

    //Necessary for animating
    public bool isMoving = false;

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
        if (activeBuffs.Count <= 0)
        {

            return movementRange;
        }
        int baseIncrease = 0;
        float multiplier = 1;
        //otherwise, total movement buffs and return
        //loop through all buffs to check for strengh buffs
        foreach (Buff buff in activeBuffs)
        {
            //check for strength buffs
            MovementBuff mBuff = buff as MovementBuff;
            if (mBuff != null)
            {
                baseIncrease += mBuff.baseIncrease;
                multiplier *= mBuff.multiplier;
            }
        }

        //return the calculated stat after base increases and multiplier
        return (int)((movementRange + baseIncrease) * multiplier);
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
        if (activeBuffs.Count == 0)
            return strength;
        
        int baseIncrease = 0;
        float multiplier = 1;

        
        //loop through all buffs to check for strengh buffs
        foreach (Buff buff in activeBuffs)
        {
            //check for strength buffs
            StrengthBuff sBuff = buff as StrengthBuff;
            if (sBuff != null)
            {
                baseIncrease += sBuff.baseIncrease;
                multiplier *= sBuff.multiplier;
            }
        }

        //return the calculated stat after base increases and multiplier
        return (int)((strength + baseIncrease) * multiplier);
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

    public IEnumerator Move(List<Vector3Int> path)
    {
        if (path.Count == 0) yield break;
        isMoving = true;

        Vector3Int startLogicalPos = GetGridPos();

        // Strip starting tile if path includes current position
        if (startLogicalPos == path[0])
            path.RemoveAt(0);

        if (path.Count == 0) yield break;

        Vector3Int destination = path[path.Count - 1];


        // --- VISUAL MOVE: lerp through each waypoint ---
        Vector3 cellOffset = new Vector3(
            tileManager.entitiesMap.cellSize.x,
            tileManager.entitiesMap.cellSize.y, 0) * 0.5f;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 startWorld = transform.position;
            Vector3 endWorld = tileManager.entitiesMap.CellToWorld(path[i]) + cellOffset;

            // Derive direction from previous step in path (not from tile data)
            Vector3Int prevPos = (i == 0) ? startLogicalPos : path[i - 1];
            Vector3Int dir = path[i] - prevPos;

            if (animator != null)
            {
                animator.SetBool("moving", true);
                animator.SetBool("attacking", false);
                animator.SetFloat("x position", Mathf.Clamp(dir.x, -1, 1));
                animator.SetFloat("y position", Mathf.Clamp(dir.y, -1, 1));
            }

            float elapsed = 0f;
            while (elapsed < tileManager.stepDuration)
            {
                transform.position = Vector3.Lerp(startWorld, endWorld, elapsed / tileManager.stepDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = endWorld; // snap to exact position
        }
        // --- LOGICAL MOVE: once, start to destination only ---
        tileManager.MoveEntity(startLogicalPos, destination);

        // Reset animator to idle
        if (animator != null)
        {
            animator.SetBool("moving", false);
            animator.SetBool("attacking", false);
            animator.SetFloat("x position", 0);
            animator.SetFloat("y position", 0);
        }

        isMoving = false;
    }

    bool Attack()
    {
        if (target.z == -1)
        {
            Debug.Log("UNIT.No target!");
            return false;
        }


        Vector3Int pos = GetGridPos();
        bool isAdjacent = Mathf.Abs(target.x - pos.x) + Mathf.Abs(target.y - pos.y) <= GetAttackRange();

        if (!isAdjacent)
        {
            Debug.Log("Target isn't adjacent!");
            //Set the target a second time
            return false;
        }

        TileData targetTile = tileManager.GetTileDataAt(target);
        if (targetTile == null || targetTile.occupyingEntity == null)
        {
            Debug.Log("UNIT.Nothing to attack!");
            return false;
        }

        targetTile.occupyingEntity.TakeDamage(GetStrength());

        if (targetTile.occupyingEntity == null)
            target = new Vector3Int(0, 0, -1);

        return true;
    }

    public void DoTurn()
    {
        //Debug.Log("Finding Target");
        if (target.z == -1)
        {
            target = aiManager.FindTarget(this);
        }

        //See if our target is up to date (needed for concurrent enemy execution)
        TileData data = tileManager.GetTileDataAt(target);
        if (data != null && !data.HasUnit())
        {
           target = aiManager.FindTarget(this);
        }

        //Debug.Log("UNIT.Found Target: " + target.x + " " + target.y + " " + target.z);
        //if we found a target we move to it
        if (target.z != -1)
        {
            //Check if the target is in the attack range
            if (!tileHelper.IsWithinRange(GetGridPos(), target, GetAttackRange()))
            {

                //Debug.Log("UNIT.Target found at " + target);
                List<Vector3Int> path = tileHelper.TilePath(GetGridPos(), target, this);
                //Debug.Log("UNIT.Distance = " + path.Count);
                if (path.Count > 0)
                {
                    StartCoroutine(Move(DeterminePath(path)));
                }
                else
                {
                    Debug.Log("UNIT.No Need to Move!");
                }
            }

        }

        //then we attack the target
        if (Attack())
        {
            //do nothing the attack worked
        }
        else
        {
            //if the attack failed on the target, we weren't in range
            // try to attack again with temp adjacent target
            Vector3Int temp = target;
            target = aiManager.FindTargetInRange(this);
            Attack();
            target = temp;
        }
    }

    public List<Vector3Int> DeterminePath(List<Vector3Int> orig)
    {
        List<Vector3Int> path = orig;
       //Reduce path to be only the segements that are moveable
       int budget = GetMoveRange();
        int cost = 0;
        int steps = 0;
        Vector3Int prev = path[0]; // first item is start, skip it
        foreach (Vector3Int step in path.Skip(1))
        {
            bool isDiagonal = (step.x != prev.x) && (step.y != prev.y);
            int tileCost = tileManager.GetTileDataAt(step).movementCost + (isDiagonal ? 1 : 0);
            if (cost + tileCost > budget) break;
            cost += tileCost;
            steps++;
            prev = step;
        }
        path = path.Skip(1).Take(steps).ToList();
        return path;
    }

    public override void Die()
    {
        SoundManager.Instance.PlayEntitySound(this, SoundType.DEATH);
        base.Die();
    }

    public override void TakeDamage(int damage)
    {
        SoundManager.Instance.PlayEntitySound(this, SoundType.HURT);
        if (activeBuffs.Count <= 0)
        {
            base.TakeDamage(damage);
            return;
        }
        else
        {
            //calculate buff defense if any
            int baseIncrease = 0;
            float multiplier = 1;
            foreach (Buff buff in activeBuffs)
            {
                //check for strength buffs
                DefenseBuff dBuff = buff as DefenseBuff;
                if (dBuff != null)
                {
                    baseIncrease += dBuff.baseIncrease;
                    multiplier *= dBuff.multiplier;
                }
            }

            //calculate reduction
            int newDamage = Mathf.Max(0, (int)(damage - (baseIncrease * multiplier)));
            base.TakeDamage(newDamage);
        }
    }
}

