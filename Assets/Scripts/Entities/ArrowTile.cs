using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Tilemaps;
public class Arrow : MonoBehaviour
{
    [SerializeField]
    public Sprite[] arrowSprites;
    private SpriteRenderer sprite;
    public TileManager tileManager;
    public Tilemap arrowMap;
    public Vector3Int gridPos;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        Debug.Log("Tried to interact with Arrow");
    }

    private Dictionary<Tuple<bool, bool, bool, bool>, int> convertToSprite;
    private Dictionary<Vector3Int, TileData> tilePosToData;

    public void Initialize()
    {
        SetUp();
    }

    public bool IsArrowAt(Vector3Int pos)
    {
        return arrowMap != null && arrowMap.HasTile(pos);
    }

    public Vector3Int GetGridPos()
    {
        return gridPos;
    }

    public TileData GetTileDataAt(Vector3Int pos)
    {
        tilePosToData.TryGetValue(pos, out TileData tileData);
        return tileData;
    }

    public void SetUp()
    {
        convertToSprite = new()
        {
            {new (false, true, false, true), 0 },
            {new (false, false, true, true), 1 },
            {new (false, true, true, false), 2 },
            {new (false, false, false, false), 3 },
            {new (true, false, false, true), 4 },
            {new (true, false, true, false), 5 },
            {new (false, true, false, false), 6 },
            {new (true, true, false, false), 7 },
            {new (false, false, false, true), 8 },
            {new (false, false, true, false), 9 },
            {new (true, false, false, false), 10 },
            {new (true, true, true, true), 11 }

        };
        UpdateSprite();
        UpdateNeighbors();
    }

    public void UpdateSprite()
    {
        Vector3Int pos = gridPos;

        bool up = IsArrowAt(pos + Vector3Int.up);
        bool down = IsArrowAt(pos + Vector3Int.down);
        bool left = IsArrowAt(pos + Vector3Int.left);
        bool right = IsArrowAt(pos + Vector3Int.right);

        var key = Tuple.Create(up, down, left, right);

        if(convertToSprite.TryGetValue(key, out int index) && arrowSprites != null && index < arrowSprites.Length && sprite != null)
        {
            sprite.sprite = arrowSprites[index];
        }
        else
        {
            Debug.LogWarning($"No sprite mapped for arrow combination: up={up} down={down} left={left} right={right}");
            if(arrowSprites != null && arrowSprites.Length > 3 && sprite != null)
            {
                sprite.sprite = arrowSprites[3];
            }
        }
    }

    public void UpdateNeighbors()
    {
        Vector3Int pos = gridPos;
        Vector3Int[] neighbors = {
            pos + Vector3Int.up,
            pos + Vector3Int.down,
            pos + Vector3Int.left,
            pos + Vector3Int.right
        };

        foreach (var neighborPos in neighbors)
        {
            // Find any Arrow GameObject at this neighbor cell and update its sprite
            // (Optional: keep a Dictionary<Vector3Int, Arrow> for efficiency)
            foreach(var arrow in FindObjectsByType<Arrow>(FindObjectsSortMode.None))
            {
                if (arrow != this && arrow.gridPos == neighborPos)
                    arrow.UpdateSprite();
            }
        }
    }

    public void Die()
    {
        UpdateNeighbors();

        // Remove the tile from the tilemap
        if (arrowMap != null)
            arrowMap.SetTile(gridPos, null);

        // Remove from scene hierarchy
        transform.SetParent(null);

        Destroy(gameObject);
    }
}
