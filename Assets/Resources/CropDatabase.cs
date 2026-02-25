using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class CropInfo
{
    public int id;
    public string cropName;

    [Header("Growth Settings")]
    //Seed stage counts as a stage, if you want a simple "grow for one turn to harvest" this number would be two
    public int numStages; 

    [Header("Economic Settings")]
    public int sellValue;

    [Header("Tilemap / Visuals")]
    //May be used by the tilemap if we want to plant pre-assigned tiles like this
    public TileBase seedTile;
    //Used to progress to full harvest, these are the sprites rendered on the tilemap
    //There should be a sprite for each sprite
    public Sprite[] growthStageSprites;
    //Used to display for UI and post harvest use
    public Sprite cropIcon;
}

[CreateAssetMenu(fileName = "AgainstTheGrain/Crop Database", menuName = "CropDatabase")]
public class CropDatabase : ScriptableObject
{
    private static CropDatabase instance;
    [Header("Basic Crop Prefab")]
    public GameObject cropPrefab;
    public static CropDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<CropDatabase>("CropDatabase");
                if (instance == null)
                {
                    Debug.LogError("CropDatabase not found in Resources!");
                }
                else
                {
                    instance.BuildLookup();
                }
            }
            return instance;
        }
    }

    public List<CropInfo> crops = new List<CropInfo>();

    private Dictionary<int, CropInfo> lookup;

    private void BuildLookup()
    {
        lookup = new Dictionary<int, CropInfo>();

        foreach (var crop in crops)
        {
            if (!lookup.ContainsKey(crop.id))
            {
                lookup.Add(crop.id, crop);
            }
            else
            {
                Debug.LogError($"Duplicate Crop ID detected: {crop.id}");
            }
        }
    }

    public CropInfo GetCropInfo(int id)
    {
        if (lookup == null)
            BuildLookup();

        if (lookup.TryGetValue(id, out CropInfo crop))
            return crop;

        Debug.LogError("No crop exists with id: " + id);
        return null;
    }

    public TileBase GetSeedTile(int id)
    {
        CropInfo crop = GetCropInfo(id);
        return crop != null ? crop.seedTile : null;
    }

    public Sprite GetIcon(int id)
    {
        CropInfo crop = GetCropInfo(id);
        return crop != null ? crop.cropIcon : null;
    }

    public int GetNumStages(int id)
    {
        CropInfo crop = GetCropInfo(id);
        return crop != null ? crop.numStages : 0;
    }

    public int GetSellValue(int id)
    {
        CropInfo crop = GetCropInfo(id);
        return crop != null ? crop.sellValue : 0;
    }

    public int GetIDFromTile(TileBase tile)
    {
        foreach (var crop in crops)
        {
            if (crop.seedTile == tile)
                return crop.id;
        }

        return -1;
    }

    public CropInfo GetCropInfoFromTile(TileBase tile)
    {
        foreach (var crop in crops)
        {
            if (crop.seedTile == tile)
                return crop;
        }

        return null;
    }

    public int GetNumCrops()
    {
        return crops.Count;
    }
}
