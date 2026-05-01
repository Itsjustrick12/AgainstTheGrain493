using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class EntityInfo : ScriptableObject
{
    [Header("Basic Information")]
    public int ID;
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

}