using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//This is the base class for making actions in the game that are availble after moving a unit
public class EntityAction : ScriptableObject
{
    public string actionName = "EntityAction";
    public string actionTrigger = "none";

    [Tooltip("width add additional width(0 is 1 tile, 1 is 3 tiles, ect..)")]
    public int width = 1;
    [Tooltip("length must be greater than 0")]
    public int length = 1;

    public AudioClip actionSound = null;

    //Returns the name of the action for scripts to identify actions with
    public virtual string GetName()
    {
        return actionName;
    }

    //Determines whether or not a unit can perform a given action
    public virtual bool IsPossible(Entity entity)
    {
        if (GetValidTargets(entity).Count <= 0 || !entity.IsActive())
        {
            return false;
        }
        return true;
    }
    //Holds the logic that is excuted when the action is chosen

    //finds any valid targets for the action
    public virtual List<Vector3Int> GetValidTargets(Entity entity)
    {
        //creates the empty list for return
        List<Vector3Int> targets = new List<Vector3Int>();

        //makes sure entity exists
        if (entity == null)
        {
            Debug.LogError("Trying to get valid targets based on an invalid Unit in attack action");
            return targets;
        }

        //defines origin tile and tilemanager
        TileManager TM = FindFirstObjectByType<TileManager>();
        Vector3Int startPos = entity.GetGridPos();

        //checks all four directions for targets
        foreach (Vector3Int offset in TileManager.DIRECTIONS)
        {
            //checks valid targets in length
            for(int i = 1; i <= length; i++)
            {
                //checks valid targets width
                for(int j = 0; j < width; j++)
                {
                    //so first we find the tile i length away
                    Vector3Int currentTile = startPos + offset * i;
                    TileData data = TM.GetTileDataAt(currentTile);
                    if(data == null)
                    {
                        continue;
                    }

                    //if there's no width we just check the center tile
                    if(j == 0)
                    {
                        //if the width is actionable
                        if(Action(data))
                        {
                            targets.Add(currentTile);
                        }
                    }//if we have a width we go through all the widths
                    else
                    {
                        Vector3Int checkTile = currentTile + new Vector3Int(offset.y * j, offset.x * j, 0);
                        data = TM.GetTileDataAt(checkTile);
                        if(data != null)
                        {
                            if(Action(TM.GetTileDataAt(checkTile)))
                            {
                                targets.Add(checkTile);
                            }
                        }
                        checkTile = currentTile + new Vector3Int(offset.y * j * -1, offset.x * j * -1, 0);
                        data = TM.GetTileDataAt(checkTile);
                        if(data != null)
                        {
                            if(Action(TM.GetTileDataAt(checkTile)))
                            {
                                targets.Add(checkTile);
                            }
                        }
                    }

                }
            }
        }

        return targets;
    }
    

    public virtual bool IsAOE()
    {
        if(width > 1 || length > 1)
        {
            return true;
        }
        return false;
    }

    //The unit here is the unit performing the action
    public virtual void PerformAt(Entity entity, List<Vector3Int> positions)
    {
        if(!IsAOE())
        {
            PerformAt(entity, positions[0]);
            return;
        }
        foreach(Vector3Int current in positions)
        {
            PerformAt(entity, current);
        }
    }

    //The unit here is the unit performing the action
    public virtual void PerformAt(Entity entity, Vector3Int pos)
    {
        TileManager TM = FindFirstObjectByType<TileManager>();
        Unit unit = entity as Unit;

        //returns if the unit doesn't exist
        if(unit == null)
        {
            Debug.Log("unit does not exist");
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

        TileData data = TM.GetTileDataAt(pos);
        Vector3Int startPos = unit.GetGridPos();
        Vector3Int offset = pos - startPos;

        if(data != null)
        {
            for(int i = 1; i <= length; i++)
            {
                //checks valid targets width
                for(int j = 0; j < width; j++)
                {
                    //so first we find the tile i length away
                    Vector3Int currentTile = startPos + offset * i;
                    data = TM.GetTileDataAt(currentTile);
                    if(data == null)
                    {
                        continue;
                    }

                    //if there's no width we just check the center tile
                    if(j == 0)
                    {
                        //if the width is actionable
                        if(Action(data))
                        {
                            PerformAt(data);
                        }
                    }//if we have a width we go through all the widths
                    else
                    {
                        Vector3Int checkTile = currentTile + new Vector3Int(offset.y * j, offset.x * j, 0);
                        data = TM.GetTileDataAt(checkTile);
                        if(data != null)
                        {
                            if(Action(TM.GetTileDataAt(checkTile)))
                            {
                                PerformAt(data);
                            }
                        }
                        checkTile = currentTile + new Vector3Int(offset.y * j * -1, offset.x * j * -1, 0);
                        data = TM.GetTileDataAt(checkTile);
                        if(data != null)
                        {
                            if(Action(TM.GetTileDataAt(checkTile)))
                            {
                                PerformAt(data);
                            }
                        }
                    }
                }
            }
        }
    }

    //Used for AOE functions
    public virtual List<Vector3Int> GetExtensionTiles(Vector3Int target, Vector3Int casterPos)
    {
        if(!IsAOE())
        {
            return new List<Vector3Int>();
        }

        //TODO set up extension Tiles properly
        List<Vector3Int> tiles =  new List<Vector3Int>();

        /*
            Vector3Int dir = new Vector3Int(
            Math.Sign(target.x - casterPos.x),
            Math.Sign(target.y - casterPos.y), 0);
        
            Debug.Log(target);
            Debug.Log(target+dir);

            return new List<Vector3Int> {target + dir};
        */

        return tiles;
    }

    //actually checks to see if the action can be done at position tilePos
    public virtual bool Action(TileData tileData){return false;}
    //actually preforms the Action on the tile
    public virtual void PerformAt(TileData tileData){}
}