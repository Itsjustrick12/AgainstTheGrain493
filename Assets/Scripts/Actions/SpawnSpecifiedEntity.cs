using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(menuName = "Actions/SpawnEntity")]
public class SpawnSpecifiedEntity : EntityAction
{
    public GameObject entityObj;
    public override string GetName()
    {
        return ("Spawn " + entityObj.GetComponent<Entity>().name);
    }

    public override List<Vector3Int> GetValidTargets(Entity entity)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        //get references 
        if (entity == null)
        {
            Debug.LogError("Trying to get valid targets based on an invalid Unit in attack action");
            return targets;
        }

        TileManager TM = FindFirstObjectByType<TileManager>();

        Vector3Int startPos = entity.GetGridPos();

        //get a reference to all tiles nearby and check if there are opposing units there
        foreach (Vector3Int offset in TileManager.DIRECTIONS)
        {
            Vector3Int currentTile = startPos + offset;
            TileData data = TM.GetTileDataAt(currentTile);

            if (data != null && data.CanPlaceEntity())
            {
                targets.Add(currentTile);
            }
        }

        Debug.Log("barn sees " + targets.Count + "valid targets for spawning");
        return targets;
    }

    public override bool IsAOE()
    {
        return false;
    }

    public override bool IsPossible(Entity entity)
    {
        List<Vector3Int> positions = GetValidTargets(entity);
        if (positions != null && positions.Count > 0)
        {
            return true;
        }
        return false;
    }

    public override void PerformAt(Entity entity, List<Vector3Int> positions)
    {
        if (IsAOE())
        {
            //do multiple
        }
        else
        {
            PerformAt(entity, positions[0]);
        }
    }

    public override void PerformAt(Entity entity, Vector3Int pos)
    {
        TileManager manager = FindFirstObjectByType<TileManager>();
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        //Execute a simple attack on the unit at the location specified
        TileData data = manager.GetTileDataAt(pos);

        if (data == null)
        {
            return;
        }
        GameObject tempEntity = Instantiate(entityObj);
        Entity toSpawn = tempEntity.GetComponent<Entity>();
        toSpawn.UpdateTransform(pos);
        manager.PlaceEntityOnTile(pos, toSpawn);
        

        //Spawn the entity with the tilemanager
        //Deactivate that unit
        if (toSpawn is Fence)
        {
            toSpawn.Activate();
        }
        else
        {
            toSpawn.Deactivate();
        }
        entity.Deactivate();
    }
}
