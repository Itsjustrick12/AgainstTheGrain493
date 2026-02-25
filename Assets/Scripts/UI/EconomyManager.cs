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

    public void SetCoins(int amount)
    {

        //Clamp so coins can never be negative
        coins = Mathf.Max(0, amount);
    }

    public void AddCoins(int amount)
    {
        SetCoins(coins + amount);
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
        return harvestedCrops.TryGetValue(id, out int amount) ? amount : 0;
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

        harvestedCrops[id]--;
        return true;
    }

    public void SetHarvestedCrops(int id, int amount)
    {
        harvestedCrops[id] = Mathf.Max(0, amount);
    }
}