using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
//Debug Class used for testing tile system implementation and rendering
//Use the type in the inspector and the left click to paint to the map
public class TilePainter : TileCursor
{
    [SerializeField] TileManager tileManager;
    [SerializeField] TileHelper pathfinder;

    public Tilemap debugMap;
    public TileType type = TileType.Dirt;

    public bool paintingTile;
    //Specifically defined input settings for the project
    private AgainstTheGrainInput input;

    public Vector3Int startPoint;
    public Vector3Int endPoint;

    public TileBase pathTile;
    public TileBase startTile;
    public TileBase endTile;

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        pathfinder = FindFirstObjectByType<TileHelper>();
    }

    protected override void Update()
    {
        HandleCursor();
        Vector3Int tile = GetMouseTile();

        if (paintingTile)
        {
           PaintTile(tile);
        }
        
    }
    //Necessary Input functions for using custom input rules
    public void PaintTile(Vector3Int pos)
    {
        tileManager.SetTile(pos, type);
    }

    public void DropBrush(InputAction.CallbackContext a)
    {
        paintingTile = true;
        PaintTile(GetMouseTile());
    }

    public void ReadTile(InputAction.CallbackContext a)
    {
        TileData data = tileManager.GetTileDataAt(currentTile);
        if (data != null)
        {
            data.PrintTileData();
        }
    }

    public void LiftBrush(InputAction.CallbackContext a)
    {
        paintingTile = false;
    }

    public void PickStart(InputAction.CallbackContext a)
    {
        if (startPoint != null)
        {

            debugMap.SetTile(startPoint, null);
        }
        startPoint = currentTile;
        debugMap.SetTile(startPoint,startTile);
    }

    public void PickEnd(InputAction.CallbackContext a)
    {
        if (endPoint != null)
        {

            debugMap.SetTile(endPoint, null);
        }
        endPoint = currentTile;
        debugMap.SetTile(endPoint, endTile);
    }

    public void DrawPath(InputAction.CallbackContext a)
    {
        List<Vector3Int> positions = pathfinder.TilePath(startPoint, endPoint);

        //Update the map to show the new tiles
        ClearDebugMap();

        //Draw for each tile in the list
        foreach (Vector3Int pos in positions)
        {
            hoverMap.SetTile(pos, pathTile);
        }
    }

    public void ClearDebugMap()
    {
        debugMap.ClearAllTiles();
    }

    private void OnEnable()
    {
        input = new AgainstTheGrainInput();
        input.Enable();
        input.Gameplay.Paint.canceled += LiftBrush;
        input.Gameplay.Paint.started += DropBrush;
        input.Gameplay.DrawPath.performed += DrawPath;
        input.Gameplay.PickA.performed += PickStart;
        input.Gameplay.PickB.performed += PickEnd;
    }

    private void OnDisable()
    {
        input.Gameplay.Paint.canceled -= LiftBrush;
        input.Gameplay.Paint.started -= DropBrush;
        input.Gameplay.DrawPath.performed += DrawPath;
        input.Gameplay.PickA.performed += PickStart;
        input.Gameplay.PickB.performed += PickEnd;
        input.Disable();
    }

}
