using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewStructureDatabase", menuName = "AgainstTheGrain/Databases/StructureDatabase")]
public class StructureDatabase : ScriptableObject
{
    private static StructureDatabase instance;

    public static StructureDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<StructureDatabase>("StructureDatabase");
                if (instance == null)
                {
                    Debug.LogError("StructureDatabase not found in Resources!");
                }
                else
                {
                    instance.BuildLookup();
                }
            }
            return instance;
        }
    }

    public List<StructureInfo> structures = new List<StructureInfo>();

    private Dictionary<int, StructureInfo> lookup;

    private void BuildLookup()
    {
        lookup = new Dictionary<int, StructureInfo>();

        foreach (var structure in structures)
        {
            if (!lookup.ContainsKey(structure.ID))
            {
                lookup.Add(structure.ID, structure);
            }
        }
    }

    public StructureInfo GetStructureInfo(int ID)
    {
        if (lookup == null)
        {
            BuildLookup();
        }

        if (lookup.TryGetValue(ID, out StructureInfo structure))
        {
            return structure;
        }

        Debug.LogError("There isn't a structure with ID " + ID);
        return null;
    }

    public GameObject GetPrefab(int ID)
    {
        StructureInfo structure = GetStructureInfo(ID);
        return structure != null ? structure.prefab : null;
    }
    public List<EntityAction> GetActions(int ID)
    {
        if (lookup == null)
        {
            BuildLookup();
        }

        //Use the lookup to try and find unit info
        if (lookup.TryGetValue(ID, out StructureInfo unit))
        {
            return unit.actions;
        }

        Debug.LogError($"There is no Unit with ID: {ID} in the lookup!");
        return null;

    }

    public Sprite GetIcon(int ID)
    {
        StructureInfo structure = GetStructureInfo(ID);
        return structure != null ? structure.sprite : null;
    }

    public int GetIDFromTile(TileBase tile)
    {
        foreach (var structure in structures)
        {
            if (structure.tile == tile)
                return structure.ID;
        }

        return -1;
    }

    public StructureInfo GetStructureInfoFromTile(TileBase tile)
    {
        foreach (var structure in structures)
        {
            if (structure.tile == tile)
                return structure;
        }

        return null;
    }

    public int GetNumStructures()
    {
        return structures.Count;
    }
}