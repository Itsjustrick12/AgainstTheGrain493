using System;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    //Needed for global references
    public static EconomyManager Instance { get; private set; }

    [Header("Currency")]
    [SerializeField] private int coins = 0;

    [Header("Harvested Crops")]
    [SerializeField] private Dictionary<int, int> harvestedCrops = new Dictionary<int, int>();

    //Used for returning coint amount to UI elements
    public static event Action<int> OnCoinsChanged;
    //Pass the ID with the crop that changed
    public static event Action<int> OnCropChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        AddHarvestedCrops(1);
        AddHarvestedCrops(3);
        AddHarvestedCrops(4);
    }

    public int GetCoins()
    {
        return coins;
    }

    public bool CanAfford(int amt)
    {
        if (amt <= coins)
        {
            return true;
        }
        return false;
    }

    public void SetCoins(int amt)
    {
        //Clamp so coins can never be negative
        coins = Mathf.Max(0, amt);
        OnCoinsChanged?.Invoke(coins);
    }

    public void AddCoins(int amt)
    {
        SetCoins(coins + amt);
    }

    public bool AttemptToBuy(int cost)
    {
        if (coins < cost)
            return false;

        SetCoins(coins - cost);
        return true;
    }

    public int GetHarvestedCrops(int id)
    {
        return harvestedCrops.TryGetValue(id, out int amt) ? amt : 0;
    }

    public Dictionary<int, int> GetAllHarvestedCrops()
    {
        return new Dictionary<int, int>(harvestedCrops);
    }

    public void AddHarvestedCrops(int id)
    {
        int current = GetHarvestedCrops(id);
        SetHarvestedCrops(id, current + 1);
    }

    public bool SellHarvestedCrops(int id)
    {
        if (!harvestedCrops.ContainsKey(id) || harvestedCrops[id] <= 0)
            return false;

        SetHarvestedCrops(id,harvestedCrops[id]-1);
        CropInfo info = CropDatabase.Instance.GetCropInfo(id);
        AddCoins(info.sellValue);
        return true;
    }

    public bool FeedHarvestedCrops(int id)
    {
        if (!harvestedCrops.ContainsKey(id) || harvestedCrops[id] <= 0)
            return false;

        SetHarvestedCrops(id, harvestedCrops[id] - 1);
        return true;
    }

    public void SetHarvestedCrops(int id, int amt)
    {
        harvestedCrops[id] = Mathf.Max(0, amt);
        OnCropChanged?.Invoke(id);
    }
    //Used to determine if you can pull up the feed menu to prevent errors
    public bool HasACrop()
    {
        foreach (var pair in harvestedCrops)
        {
            int cropID = pair.Key;
            int amount = pair.Value;
            //if any crop has a single entry, return that theres a crop
            if (amount > 0)
            {
                return true;
            }
        }
        return false;
    }
}