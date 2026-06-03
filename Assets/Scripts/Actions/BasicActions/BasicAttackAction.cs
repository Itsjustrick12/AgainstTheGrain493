using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
[CreateAssetMenu(menuName = "Actions/Attack")]
public class BasicAttackAction : EntityAction
{

    //Need to validate size when returned
    public override List<Vector3Int> GetValidTargets(Entity entity)
    {
        List<Vector3Int> targets = new List<Vector3Int>();

        Unit unit = entity as Unit;
        if (unit == null)
        {
            Debug.LogError("No Unit, just an entity");
        }

        Vector3Int startPos = unit.GetGridPos();
        int atkRange = unit.GetAttackRange();

        for (int i = -atkRange; i <= atkRange; i++)
        {
            for (int j = -atkRange; j <= atkRange; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) > atkRange) continue;

                //Don't include self as target
                if (i == 0 && j == 0) continue;

                Vector3Int currentTile = startPos + new Vector3Int(i, j, 0);
                if(AttackAction(unit, currentTile))
                {
                    targets.Add(currentTile);
                }
            }
        }

        return targets;

    }

    //actually checks to see if the action can be done at position tilePos
    public virtual bool AttackAction(Unit unit, Vector3Int centerTile)
    {
        TileManager manager = FindFirstObjectByType<TileManager>();
        //checks valid targets in length
        for(int i = 1; i <= length; i++)
        {
            //checks valid targets width
            for(int j = 0; j < width; j++)
            {
                Vector3Int currentTile = centerTile + new Vector3Int(i, j, 0);
                TileData data = manager.GetTileDataAt(currentTile);
                if (data != null && data.HasOccupant())
                {
                    Unit unitCheck = data.occupyingEntity as Unit;
                    if (unitCheck != null && !unit.IsSameTeamAs(unitCheck))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //The unit here is the unit performing the action
    public override void PerformAt(Entity entity, Vector3Int pos)
    {
        Unit unit = entity as Unit;

        //returns if the unit doesn't exist
        if(unit == null)
        {
            return;
        }//sets the animator if it exists
        else if(unit.HasAnimator())
        {
            if(pos.x - unit.GetGridPos().x != 0)
            {
                unit.animator.SetFloat("facing", pos.x - unit.GetGridPos().x);
            }
            unit.SetAnimationTrigger(actionTrigger);
        }

        if(actionSound != null)
        {
            SoundManager.Instance.PlaySound(actionSound);
        }

        PerformAttackAt(unit, pos);
    }

    //actually preforms the Action on the tile
    public virtual void PerformAttackAt(Unit unit, Vector3Int centerTile)
    {
        //make sure unit exists
        if (unit == null)
        {
            return;
        }

        TileManager manager = FindFirstObjectByType<TileManager>();

        //checks valid targets in length
        for(int i = 1; i <= length; i++)
        {
            //checks valid targets width
            for(int j = 0; j < width; j++)
            {
                Vector3Int currentTile = centerTile + new Vector3Int(i, j, 0);
                TileData data = manager.GetTileDataAt(currentTile);

                //makes sure the tile exists
                if(data == null)
                {
                    continue;
                }

                Entity targetEntity = FindFirstObjectByType<TileManager>().GetEntityOnTile(currentTile);
                if (targetEntity == null)
                {
                    continue;
                }

                Unit targetUnit = targetEntity as Unit;
                if(targetUnit == null)
                {
                    continue;
                }

                unit.ShowNumber(unit.GetStrength(), targetUnit.GetGridPos(), unit.GetGridPos().x - unit.GetGridPos().x);
                targetUnit.TakeDamage(unit.GetStrength(), unit.GetGridPos());
            }
        }
    }
}
