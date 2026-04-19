using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

//This is the base script that is used for all things that can occupy tiles in the game
//This includes units, obstacles, interactable objects, and crops

public enum EntityType
{
    Animal,
    Crop,
    Enemy,
    Farmer,
    Structure,
    None
}

[RequireComponent(typeof(SpriteRenderer))]
public class Entity : MonoBehaviour, IBuffable
{
    protected SpriteRenderer sprite;
    //Stores the location of where this entity actually is
    private Vector3Int gridPos;
    protected bool isActive = true;
    protected TileManager tileManager;
    protected GameManager gameManager;
    protected AIManager aiManager;
    protected TileHelper tileHelper;

    private bool isInitialized = false;

    //near constant color used for dimming entities when they are deactivated
    public static readonly Color DimColor = new Color(0.4f, 0.4f, 0.4f);

    [Header("Stats")]
    //stores the entity's max hitpoints
    [SerializeField] protected int maxHealth = 10;
    //stores the entity's type
    [SerializeField] protected EntityType type = EntityType.None;
    //stores the entity's hitpoints
    [SerializeField] protected int currentHealth = 10;
    //Determines where or not something can pathfind through the tile this entity is on
    [SerializeField] private bool isObstacle;
    //Determines if this entity can be clicked on or affected in any way
    [SerializeField] private bool isInteractable;
    private Vector3 offset = new Vector3(0.5f, 0.5f, 0);
    //holds the animator
    [SerializeField] protected Animator animator;

    //Hidden logic for determining what a unit is able to do, define by the unit database
    protected List<EntityAction> actions = new();

    public static event Action<Entity> OnEntityDestroyed;

    //For managing buffs
    protected List<Buff> activeBuffs = new List<Buff>();

    public virtual void Start()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        tileHelper = FindFirstObjectByType<TileHelper>();
    }

    public void InitializeActions(List<EntityAction> newActions)
    {
        actions = newActions;
    }

    public List<EntityAction> GetAvailableActions()
    {
        //Return all the actions that are currently possible given the Unit's information (and generally position)
        return actions.Where(action => action.IsPossible(this)).ToList();
    }

    public void SetGridPos(Vector3Int pos)
    {
        gridPos = pos;
        transform.position = pos+offset;
    }

    public Vector3Int GetGridPos()
    {
        return gridPos;
    }

    public int GetHealth()
    {
        return currentHealth;
    }
    
    public void SetCurrentHealth(int healthValue)
    { 
        if(healthValue > maxHealth)
        {
            healthValue = maxHealth;
        }
        currentHealth = healthValue;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetMaxHealth(int healthValue)
    { 
        if(healthValue > 0)
        {
            healthValue = maxHealth;
        }
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public void SetHealth(int healthValue)
    { 
        if(healthValue > maxHealth)
        {
            healthValue = maxHealth;
        }
        currentHealth = healthValue;
    }

    public bool IsInteractable()
    {
        return isInteractable;
    }

    public void SetIsInteractable(bool temp)
    {
        isInteractable = temp;
    }

    public bool IsObstacle()
    {
        return isObstacle;
    }
    public void SetIsObstacle(bool obstacle)
    {
        isObstacle = obstacle;
    }
    public void HideSprite()
    {
        sprite.enabled = false;
    }
    public void ShowSprite()
    {
        sprite.enabled = true;
    }

    public Sprite GetSprite()
    {
        return sprite.sprite;
    }

    public void SetSprite(Sprite temp)
    {
        sprite.sprite = temp;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual EntityType GetEntityType()
    {
        return type;
    }

    public void SetType(EntityType temp)
    {
        type = temp;
    }

    public virtual void Die()
    {
        //Remove entity from tile
        TileData tile = tileManager.GetTileDataAt(GetGridPos());
        tile.ClearOccupant();

        //Remove from hierarchy (needed for the check of how many units there are
        transform.SetParent(null);

        //Now game state is accurate
        OnEntityDestroyed(this);

        //Destroy entity after the check to allow it to happen
        Destroy(gameObject);

    }

    public void UpdateTransform(Vector3Int pos)
    {
        //Update the Transform to refelct the gameobject visually
        this.gameObject.transform.position = pos + (new Vector3(0.5f, 0.5f, 0f));
    }

    public virtual void DestroyEntity()
    {
        //Remove this entity from the field by updating the tile data it belongs to
        TileManager tM = FindFirstObjectByType<TileManager>();
        tM.GetTileDataAt(GetGridPos()).occupyingEntity = null;
        Destroy(this.gameObject);
    
    }

    public virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        aiManager = FindFirstObjectByType<AIManager>();
        Initialize();
    }

    //Used to update stats based on database
    public virtual void Initialize()
    {
        isInitialized = true;
    }

    public bool GetIsInitialized()
    {
        return isInitialized;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void Deactivate()
    {
        sprite.color = DimColor;
        isActive = false;
    }

    public virtual void Activate()
    {
        sprite.color = Color.white;
        isActive = true;
    }

    public void AddBuff(Buff buff)
    {
        activeBuffs.Add(buff);
        buff.Apply(this);
    }
    //Is called by the buff class itself who manages the duration of itself
    public void RemoveBuff(Buff buff)
    {
        if (!activeBuffs.Contains(buff))
        {
            //if the buff isn't here, dont do anything
            return;
        }
        activeBuffs.Remove(buff);
    }

    public void ClearBuffs()
    {
        activeBuffs.Clear();
    }
}
