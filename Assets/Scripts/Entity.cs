using UnityEngine;

//This is the base script that is used for all things that can occupy tiles in the game
//This includes units, obstacles, interactable objects, and crops
[RequireComponent(typeof(SpriteRenderer))]
public class Entity : MonoBehaviour
{
    protected SpriteRenderer sprite;
    //Stores the location of where this entity actually is
    private Vector3Int gridPos;
    protected bool isActive = true;
    //Determines where or not something can pathfind through the tile this entity is on
    [SerializeField] private bool isObstacle;
    //Determines if this entity can be clicked on or affected in any way
    [SerializeField] private bool isInteractable;
    private Vector3 offset = new Vector3(0.5f, 0.5f, 0);
    //Helper function for all derived entities to use to determine whether or not something is occupying the tile

    //Used for deactivating the entity
    public SpriteRenderer shadeSprite;
    public Vector3Int GetGridPos()
    {
        return gridPos;
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

    public void UpdateTransform(Vector3Int pos)
    {
        //Update the Transform to refelct the gameobject visually
        this.gameObject.transform.position = pos + (new Vector3(0.5f, 0.5f, 0f));
    }

    public virtual void DestroyEntity()
    {
        //Remove this entity from the field by updating the tile data it belongs to
        TileManager tM = FindFirstObjectByType<TileManager>();
        tM.GetTileDataAt(GetGridPos()).ClearOccupant();
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
}
