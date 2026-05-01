using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewCropDatabase", menuName = "AgainstTheGrain/Databases/CropDatabase")]
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
            if (!lookup.ContainsKey(crop.ID))
            {
                lookup.Add(crop.ID, crop);
            }
            else
            {
                Debug.LogError($"Duplicate Crop ID detected: {crop.ID}");
            }
        }
    }

    public CropInfo GetCropInfo(int ID)
    {
        if (lookup == null)
            BuildLookup();

        if (lookup.TryGetValue(ID, out CropInfo crop))
            return crop;

        Debug.LogError("No crop exists with ID: " + ID);
        return null;
    }

    public TileBase GetSeedTile(int ID)
    {
        CropInfo crop = GetCropInfo(ID);
        return crop != null ? crop.tile : null;
    }

    public Sprite GetIcon(int ID)
    {
        CropInfo crop = GetCropInfo(ID);
        return crop != null ? crop.sprite : null;
    }

    public int GetNumStages(int ID)
    {
        CropInfo crop = GetCropInfo(ID);
        return crop != null ? crop.numStages : 0;
    }

    public int GetSellValue(int ID)
    {
        CropInfo crop = GetCropInfo(ID);
        return crop != null ? crop.sellValue : 0;
    }

    public int GetIDFromTile(TileBase tile)
    {
        foreach (var crop in crops)
        {
            if (crop.tile == tile)
                return crop.ID;
        }

        return -1;
    }

    public CropInfo GetCropInfoFromTile(TileBase tile)
    {
        foreach (var crop in crops)
        {
            if (crop.tile == tile)
                return crop;
        }

        return null;
    }

    public int GetNumCrops()
    {
        return crops.Count;
    }
}
