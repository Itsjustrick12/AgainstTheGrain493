using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileCursor : MonoBehaviour
{
    public Tilemap hoverMap;
    public Tilemap targetMap;
    public TileBase selectTile;

    private bool showHighlight = true;

    //The currently hovered tile
    protected Vector3Int currentTile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Update()
    {
        HandleCursor();
    }

    protected virtual void HandleCursor()
    {
        Vector3Int tile = GetMouseTile();

        if (showHighlight && tile != currentTile && targetMap.HasTile(tile))
        {
            DeselectLast();
            currentTile = tile;
            SelectCurrent(tile);
        }
    }

    public void SelectCurrent(Vector3Int pos)
    {
        hoverMap.SetTile(pos, selectTile);
    }

    //May be changed
    protected Vector3Int GetMouseTile()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        return targetMap.WorldToCell(mouseWorld);
    }
    public void DeselectLast()
    {
        hoverMap.SetTile(currentTile, null);
    }
    public void HideHighlight()
    {
        DeselectLast();
        showHighlight = false;
    }
    public void ShowHighlight()
    {
        showHighlight = true;
    }
}
