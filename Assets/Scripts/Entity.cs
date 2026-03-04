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
public class Entity : MonoBehaviour
{
    protected SpriteRenderer sprite;
    //Stores the location of where this entity actually is
    private Vector3Int gridPos;
    protected bool isActive = true;
    protected TileManager tileManager;
    protected GameManager gameManager;
    [SerializeField]protected TileHelper tileHelper;

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
    //Helper function for all derived entities to use to determine whether or not something is occupying the tile

    //Hidden logic for determining what a unit is able to do, define by the unit database
    protected List<EntityAction> actions = new();

    //Used for deactivating the entity
    public SpriteRenderer shadeSprite;

    public static event Action<Entity> OnEntityDestroyed;
    public Vector3Int GetGridPos()
    {
        return gridPos;
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
    public virtual bool IsObstacle()
    {
        return isObstacle;
    }
    public void SetIsObstacle(bool obstacle)
    {
        isObstacle = obstacle;
    }
    public bool IsInteractable()
    {
        return isInteractable;
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

    public int GetHealth()
    {
        return currentHealth;
    }

    public void SetHealth(int healthValue)
    { 
        if(healthValue > maxHealth)
        {
            healthValue = maxHealth;
        }
        currentHealth = healthValue;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        EntityInfo data = UnitDatabase.Instance.GetUnitInfo(1);
        SoundManager.Instance.PlaySound(data.hurtSound);
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
        EntityInfo data = UnitDatabase.Instance.GetUnitInfo(1);
        SoundManager.Instance.PlaySound(data.deathSound);
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
        shadeSprite.sprite = sprite.sprite;
    }

   public bool IsActive()
    {
        return isActive;
    }

    public void Deactivate()
    {
        shadeSprite.enabled = true;
        isActive = false;
    }

    public void Activate()
    {
        shadeSprite.enabled = false;
        isActive = true;
    }

    public virtual void Start()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        tileHelper = FindFirstObjectByType<TileHelper>();
    }
}
