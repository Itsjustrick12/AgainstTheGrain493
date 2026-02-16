using System;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

//Dual Grid Tilemap Implementation is based on the logic from this video: https://www.youtube.com/watch?v=jEWFSv3ivTg
public class TileManager : MonoBehaviour
{
    //Tilemap the stores the underlying "what kind of tile is here?" information
    //(this should be covered by the display map upon rendering)
    public Tilemap placeholderMap;

    //The display based on the dual grid sprite alignment based on 4 placeholders
    //These two maps are two different layers of the display
    //Displays the underlying tile (grass, dirt, or water)
    public Tilemap baseDisplayMap;
    //Overlay tiles like paths and water are overlayed on top of primary 
    public Tilemap overlayDisplayMap;

    // Holds information about what entities (objects, units, crops) are going to be present at a given tile
    public Tilemap entitiesMap;
    //Will be used for drawing the boarder around where tiles are placed so theres no screen edges
    public Tilemap borderMap;

    //Used for getting nearby references of tiles
    private static Vector3Int[] NEIGHBORS = new Vector3Int[]
    {
        //Offsets for referencing specific tiles
        new Vector3Int(0,0,0), //Top Right
        new Vector3Int(1,0,0), //Top Left
        new Vector3Int(0,1,0), //Bottom Right
        new Vector3Int(1,1,0) //Bottom Left
    };
    //Used for working backwards from display to placeholder
    private static Vector3Int[] DIRECTIONS = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

    //need references to the placeholder tiles to convert to display tiles
    public TileBase grassTile;
    public TileBase waterTile;
    public TileBase dirtTile;
    public TileBase wateredDirtTile;
    public TileBase pathTile;

    //Dicionary for getting quick references to the data at a given tile (occupants, terrain etc)
    private Dictionary<Vector3Int, TileData> tilePosToData;

    //Stores grass dirt interaction tiles and greyed out for watered soil
    [SerializeField] TileBase[] tiles;

    public void Start()
    {
        tilePosToData = new Dictionary<Vector3Int, TileData>();
        int size = 16;
        for (int i = -size/2; i < size / 2; i++)
        {
            for (int j = -size / 2; j < size / 2; j++)
            {
                Vector3Int tilePos = new Vector3Int(i, j, 0);
                if (!placeholderMap.HasTile(tilePos))
                {
                    SetTile(tilePos, TileType.Grass);
                }
                SetBaseDisplayTile(tilePos);
                SetOverlayDisplayTile(tilePos);
            }
        }
    }

    //Needed for determining what kind of tile needs to be rendered based on the placeholder
    public TileType GetTileTypeAt(Vector3Int pos)
    {
        return TypeFromPlaceholder(placeholderMap.GetTile(pos));
    }

    //Return the proper placeholder tile based on the passsed type (must check against null!)
    private TileBase PlaceholderFromType(TileType type)
    {
        switch (type)
        {
            case TileType.Grass: return grassTile;
            case TileType.Dirt: return dirtTile;
            case TileType.WateredDirt: return wateredDirtTile;
            //case TileType.Water: return waterTile;
            //case TileType.Path: return pathTile;
            default: return null;
        }
    }
    //Return the proper Type tile based on the passsed placeholder tile
    private TileType TypeFromPlaceholder(TileBase tile)
    {
        if (tile == grassTile) return TileType.Grass;
        else if (tile == dirtTile) return TileType.Dirt;
        else if (tile == wateredDirtTile) return TileType.WateredDirt;
        //else if (tile == waterTile) return TileType.Water;
        //else if (tile == pathTile) return TileType.Path;
        
        //A proper placeholder tile wasn't passed
        return TileType.None;
    }

    //Get a reference to the data stored at any given position
    public TileData GetTileDataAt(Vector3Int pos)
    {
        tilePosToData.TryGetValue(pos, out TileData temp);
        //See if theres anything stored for the pos
        TileData tileData = temp;
        if (tileData != null)
        {
            return tileData;
        }
        //otherwise pass nothing back
        return null;
    }

    //Used when initiating the game scene board
    public void SetTile(Vector3Int pos, TileType type)
    {
        //Update the placeholder map
        placeholderMap.SetTile(pos, PlaceholderFromType(type));

        //Update the 4 affected display tiles
        //If water or path, draw to the overlay

        SetOverlayDisplayTile(pos);


        //Update four surrounding neighbors
        SetBaseDisplayTile(pos);

        //Update Tile Data object to reflect the new type
        if (tilePosToData.ContainsKey(pos))
        {
            //Update the tiledata that exists
            tilePosToData[pos].UpdateType(type);
        }
        else
        {
            tilePosToData[pos] = new TileData(type);
        }

    }

    private void SetBaseDisplayTile(Vector3Int pos)
    {
        //Update the four surrounding tiles (based on neighbors needed for calculations
        for (int i = 0; i < 4; i++)
        {
            Vector3Int tempPosition = pos + NEIGHBORS[i];
            baseDisplayMap.SetTile(tempPosition, CalculateBaseDisplayTile(tempPosition));
        }
    }

    private void SetOverlayDisplayTile(Vector3Int pos)
    {
        //Update the four surrounding tiles (based on neighbors needed for calculations
        for (int i = 0; i < 4; i++)
        {
            Vector3Int tempPosition = pos + NEIGHBORS[i];
            overlayDisplayMap.SetTile(tempPosition, CalculateOverlayDisplayTile(tempPosition));
        }
    }

    private TileBase CalculateBaseDisplayTile(Vector3Int pos)
    {
        TileType tR = GetTileTypeAt(pos - NEIGHBORS[0]);
        TileType tL = GetTileTypeAt(pos - NEIGHBORS[1]);
        TileType bR = GetTileTypeAt(pos - NEIGHBORS[2]);
        TileType bL = GetTileTypeAt(pos - NEIGHBORS[3]);

        int mask = 0;

        //Bit math for shifting to the correct tile iteration
        //Requires the sprites to be sorted in an odd way to match the bit logic
        //If marked as dirt of any kind, add the flag for that given corner
        if (tL == TileType.Dirt || tL == TileType.WateredDirt) mask |= 1;   // TL
        if (tR == TileType.Dirt || tR == TileType.WateredDirt) mask |= 2;   // TR
        if (bL == TileType.Dirt || bL == TileType.WateredDirt) mask |= 4;   // BL
        if (bR == TileType.Dirt || bR == TileType.WateredDirt) mask |= 8;   // BR

        return tiles[mask];
    }

    private TileBase CalculateOverlayDisplayTile(Vector3Int pos)
    {
        TileType tR = GetTileTypeAt(pos - NEIGHBORS[0]);
        TileType tL = GetTileTypeAt(pos - NEIGHBORS[1]);
        TileType bR = GetTileTypeAt(pos - NEIGHBORS[2]);
        TileType bL = GetTileTypeAt(pos - NEIGHBORS[3]);

        int mask = 0;

        //Bit math for shifting to the correct tile iteration
        //Requires the sprites to be sorted in an odd way to match the bit logic
        //If watered dirt, add the flag for that given corner
        if (tL == TileType.WateredDirt) mask |= 1;   // TL
        if (tR == TileType.WateredDirt) mask |= 2;   // TR
        if (bL == TileType.WateredDirt) mask |= 4;   // BL
        if (bR == TileType.WateredDirt) mask |= 8;   // BR

        //Return the mask shifted up to the opacity tiles
        return tiles[mask+16];
    }


}
