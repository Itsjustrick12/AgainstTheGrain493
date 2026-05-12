using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/AdvancedPlant")]
public class AdvancedPlantAction : PlantAction
{
    //
    public override List<Vector3Int> GetValidTargets(Entity unit)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        //get references 
        if (unit == null)
        {
            Debug.LogError("Trying to get valid targets based on an invalid Unit in attack action");
            return targets;
        }

        TileManager TM = FindFirstObjectByType<TileManager>();

        Vector3Int startPos = unit.GetGridPos();

        //get a reference to all tiles nearby and check if there are opposing units there
        foreach (Vector3Int offset in TileManager.DIRECTIONS)
        {
            for(int i = 0; i < 3; i++)
            {
                Vector3Int currentTile = startPos + (offset * i);
                TileData data = TM.GetTileDataAt(currentTile);

                if(data != null && data.IsPlantable())
                {
                    targets.Add(startPos + offset);
                    i = 3;
                }
            }

            
        }
        //Debug.Log("Found " + targets.Count + " different crops that can be harvested");
        return targets;

    }

    public override bool IsPossible(Entity unit)
    {
        //Attack isn't possible if there are no nearby enemy units or the unit already moved
        if (GetValidTargets(unit).Count <= 0 || !unit.IsActive())
        {
            return false;
        }
        return true;
    }

    public override bool IsAOE()
    {
        return true;
    }

    public override void PerformAt(Entity unit, Vector3Int pos)
    {
        GameManager GM = FindFirstObjectByType<GameManager>();

        //attempt to trigger the animation
        Unit unitCheck = unit as Unit;
        if(unitCheck != null && unitCheck.HasAnimator())
        {
            if(pos.x - unitCheck.GetGridPos().x != 0)
            {
                unitCheck.animator.SetFloat("facing", pos.x - unitCheck.GetGridPos().x);
            }
            unitCheck.SetAnimationTrigger("plant");
        }

        //attempt to plant crop
        GM.SpawnCropOnTile(CropDatabase.Instance.GetCropInfo(cropID), pos);

        //plant the crops away from the unit
        
        if(pos.x != unit.GetGridPos().x)
        {
            GM.SpawnCropOnTile(CropDatabase.Instance.GetCropInfo(cropID),new Vector3Int(pos.x, pos.y + 1, pos.z));
            GM.SpawnCropOnTile(CropDatabase.Instance.GetCropInfo(cropID),new Vector3Int(pos.x, pos.y - 1, pos.z));
        }
        else if(pos.y != unit.GetGridPos().y)
        {
            GM.SpawnCropOnTile(CropDatabase.Instance.GetCropInfo(cropID),new Vector3Int(pos.x + 1, pos.y, pos.z));
            GM.SpawnCropOnTile(CropDatabase.Instance.GetCropInfo(cropID),new Vector3Int(pos.x - 1, pos.y, pos.z));
        }

        SoundManager.Instance.PlaySound(plantSound);
        onPlant?.Invoke();
    }

    public override List<Vector3Int> GetExtensionTiles(Vector3Int target, Vector3Int casterPos)
    {
        Vector3Int dir = new Vector3Int(
            Math.Sign(target.x - casterPos.x),
            Math.Sign(target.y - casterPos.y), 0);
        Vector3Int perp = new Vector3Int(dir.y, dir.x, 0);

        return new List<Vector3Int> { target - perp, target + perp };
    }
}
