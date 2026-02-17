using UnityEngine;
public enum TileType
{
    None,
    Grass,
    Dirt,
    WateredDirt,
    //Path,
    //Water
}
public class TileData
{
    public Vector3Int gridPos;
    public TileType type;
    //This is the entity stored at the current tile, could be a crop, unit, structure, or object etc
    public Entity occupyingEntity;
    //This is based on the TILE not the entity blocking the tile
    public bool canWalk;
    //Some tiles may require additional movement costs down the line
    //For simple tiles, just set the movement cost to be 1
    public int movementCost;
    //Depending on the type of tile, units may recieve additional protection during combat
    public int defenseBonus;

    //Makes an unoccupied grass tile
    public TileData()
    {
        type = TileType.Grass;
        occupyingEntity = null;
        canWalk = true;
        movementCost = 1;
        defenseBonus = 0;
    }
    public TileData(TileType type, Entity occupyingEntity = null, bool canWalk = true, int movementCost = 1, int defenseBonus = 0)
    {
        this.type = type;
        this.occupyingEntity = occupyingEntity;
        this.canWalk = canWalk;
        this.movementCost = movementCost;
        this.defenseBonus = defenseBonus;
    }

    //Used for pathfinding algorithms
    public bool IsWalkable()
    {
        return canWalk && (occupyingEntity == null || !occupyingEntity.IsObstacle());
    }
    public bool HasOccupant()
    {
        return occupyingEntity != null;
    }

    public void UpdateType(TileType type)
    {
        this.type = type;
        //maybe disable walking if a certain type later
    }

    public void PlaceEntity(Entity entity)
    {
        if (entity != null)
        {
            Debug.Log("Entity was overwritten!");
        }
        occupyingEntity = entity;
    }

    public bool CanPlaceEntity()
    {
        if (occupyingEntity != null)
        {
            return false;
        }
        return true;
    }
}
