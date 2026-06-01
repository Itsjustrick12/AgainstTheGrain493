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

    public override bool IsPossible(Entity entity)
    {
        List<Vector3Int> positions = GetValidTargets(entity);
        if (positions != null && positions.Count > 0)
        {
            return true;
        }
        return false;
    }

    public override bool Action(TileData tileData)
    {
        if (data != null && data.CanPlaceEntity())
        {
            return true;
        }

        return false;
    }

    //actually preforms the Action on the tile
    public override void PerformAt(TileData tileData)
    {
        if(tileData = null)
        {
            return;
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        Vector3Int pos = tileData.getGridPos();
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
