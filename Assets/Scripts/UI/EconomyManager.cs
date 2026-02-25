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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
    }

    public void AddCoins(int amt)
    {
        SetCoins(coins + amt);
    }

    public bool AttemptToBuy(int cost)
    {
        if (coins < cost)
            return false;

        coins -= cost;
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
        harvestedCrops[id] = GetHarvestedCrops(id) + 1;
    }

    public bool SellHarvestedCrops(int id)
    {
        if (!harvestedCrops.ContainsKey(id) || harvestedCrops[id] <= 0)
            return false;

        harvestedCrops[id] = harvestedCrops[id]-1;
        CropInfo info = CropDatabase.Instance.GetCropInfo(id);
        AddCoins(info.sellValue);
        return true;
    }

    public void SetHarvestedCrops(int id, int amt)
    {
        harvestedCrops[id] = Mathf.Max(0, amt);
    }
}