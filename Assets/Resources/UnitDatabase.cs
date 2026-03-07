using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityInfo
{
    //Basic Indentifiers
    public int id;
    public string entityName;
    public int baseHealth = 0;

    [Header("Prefab Spawned in Game")]
    public GameObject prefab;

    [Header("Level Editor / Map Preview")]
    public TileBase tile;
    public Sprite sprite;

    [Header("Actions")]
    public List<EntityAction> actions;

    [Header("Economy Values")]
    public int purchasePrice;
    public int sellValue;

    [Header("Sounds")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip attackSound;
    public AudioClip pickupSound;
    public AudioClip placeSound;

}

//Entries for Database system used by Level Editor and other UI
[System.Serializable]
public class UnitInfo : EntityInfo
{
    [Header("Unit Specific")]
    public bool isEnemy;
    public int attackRange = 1;
    public int strength = 5;
    public int moveRange = 3;
}
//
[CreateAssetMenu(fileName = "AgainstTheGrain/Unit Database", menuName = "UnitDatabase")]
public class UnitDatabase : ScriptableObject
{
    private static UnitDatabase instance;

    public static UnitDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<UnitDatabase>("UnitDatabase");
                if (instance == null)
                {
                    Debug.LogError("UnitDatabase not found in Resources!");
                }
                else
                {
                    instance.BuildLookup();
                }
            }
            return instance;
        }
    }

    public List<UnitInfo> units = new List<UnitInfo>();

    // Used for quickly finding information with only an id int
    private Dictionary<int, UnitInfo> lookup;

    private void BuildLookup()
    {
        lookup = new Dictionary<int, UnitInfo>();
        //Loop over all units and add an entry in the lookup
        foreach (var unit in units)
        {
            if (!lookup.ContainsKey(unit.id))
            {
                lookup.Add(unit.id, unit);
            }
            else
            {
                //If exists already, read error out
                Debug.LogError($"There are two Units trying to use the same ID: {unit.id}");
            }
        }
    }

    public UnitInfo GetUnitInfo(int id)
    {
        if (lookup == null)
        {
            BuildLookup();
        }

        //Use the lookup to try and find unit info
        if (lookup.TryGetValue(id, out UnitInfo unit))
        {
            return unit;
        }

        Debug.LogError($"There is no Unit with ID: {id} in the lookup!");
        return null;

    }

    public List<EntityAction> GetActions(int id)
    {
        if (lookup == null)
        {
            BuildLookup();
        }

        //Use the lookup to try and find unit info
        if (lookup.TryGetValue(id, out UnitInfo unit))
        {
            return unit.actions;
        }

        Debug.LogError($"There is no Unit with ID: {id} in the lookup!");
        return null;

    }

    //get the reference to the specified unit's gameobject
    public GameObject GetPrefab(int id)
    {
        UnitInfo unit = GetUnitInfo(id);
        if (unit == null)
        {
            return null;
        }

        return unit.prefab;
    }

    public int GetPurcahsePrice(int id)
    {
        UnitInfo unit = GetUnitInfo(id);
        if (unit == null)
        {
            return -1;
        }

        return unit.purchasePrice;
    }

    //Get reference to the specfied unit's placeholder tile
    public TileBase GetTile(int id)
    {
        UnitInfo unit = GetUnitInfo(id);
        if (unit == null)
        {
            return null;
        }

        return unit.tile;
    }
    //Get reference to the specfied unit's sprite
    public Sprite GetSprite(int id)
    {
        UnitInfo unit = GetUnitInfo(id);
        if (unit == null)
        {
            return null;
        }

        return unit.sprite;
    }

    public int GetIDFromTile(TileBase tile)
    {
        //Start from ID 1
        for (int i = 1; i < units.Count + 1; i++)
        {
            //Get the unitTile for each index
            TileBase unitTile = GetTile(i);
            if (unitTile == tile)
            {
                return i;
            }
        }
        //Return -1 if no matching tile
        return -1;
    }

    public bool GetIsEnemy(int id)
    {
        UnitInfo unit = GetUnitInfo(id);
        if (unit == null)
        {
            Debug.LogError($"There is no Unit with ID: {id} in the lookup!");
        }

        return unit.isEnemy;
    }

    public int GetNumUnits()
    {
        return units.Count;
    }


    public UnitInfo GetUnitInfoFromTile(TileBase tile)
    {
        //Start from ID 1
        for (int i = 1; i < units.Count + 1; i++)
        {
            //Get the objectTile for each index
            TileBase objectTile = GetTile(i);
            if (objectTile == tile)
            {
                return units.ElementAt(i - 1);
            }
        }
        //Return -1 if no matching tile
        return null;
    }
}