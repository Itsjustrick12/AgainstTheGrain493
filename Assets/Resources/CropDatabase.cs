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
        return crop != null ? crop.tile : null;
    }

    public Sprite GetIcon(int id)
    {
        CropInfo crop = GetCropInfo(id);
        return crop != null ? crop.sprite : null;
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
            if (crop.tile == tile)
                return crop.id;
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
