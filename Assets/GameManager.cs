using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public TileManager tileManager;

    //Transforms for sorting
    public Transform FriendlyUnits;
    public Transform EnemyUnits;

    public Tilemap entityMap;
    
    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();

    }

    public void Start()
    {
        SpawnStartingUnits();
    }

    public void SpawnStartingUnits()
    {
        //loop over a placeholder tilemap for placing units 
        int size = 16;
        for (int i = -size / 2; i < size / 2; i++)
        {
            for (int j = -size / 2; j < size / 2; j++)
            {
                Vector3Int tilePos = new Vector3Int(i, j, 0);
                if (entityMap.HasTile(tilePos))
                {
                    TileBase temp = entityMap.GetTile(tilePos);
                    UnitInfo info = UnitDatabase.Instance.GetUnitInfoFromTile(temp);
                    if (info != null)
                    {
                        SpawnUnitOnTile(info, tilePos);
                    }
                    entityMap.SetTile(tilePos, null);
                }
            }
        }
    }

    public void SpawnUnitOnTile(UnitInfo unitInfo, Vector3Int pos)
    {
        //spawn the new object
        GameObject obj = Instantiate(unitInfo.prefab);
        if (unitInfo.isEnemy)
        {
            obj.transform.parent = EnemyUnits;
        }
        else
        {
            obj.transform.parent = FriendlyUnits;
        }
        //Update the position
        Unit unitRef  = obj.GetComponent<Unit>();
        if (unitRef != null) {

            tileManager.PlaceEntityOnTile(pos, unitRef);
            unitRef.UpdateTransform(pos);
        }

    }
    //Uses the transform containers to return all friendly units
    public List<Unit> GetAllFriendlyUnits()
    {
        return new List<Unit>(FriendlyUnits.GetComponentsInChildren<Unit>());
    }

    public List<Unit> GetAllEnemyUnits()
    {
        return new List<Unit>(EnemyUnits.GetComponentsInChildren<Unit>());
    }
}
