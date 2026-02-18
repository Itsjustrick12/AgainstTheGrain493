using UnityEngine;
using UnityEngine.InputSystem;
//Debug Class used for testing tile system implementation and rendering
//Use the type in the inspector and the left click to paint to the map
public class TilePainter : TileCursor
{
    [SerializeField] TileManager tileManager;
    public TileType type = TileType.Dirt;

    public bool paintingTile;
    //Specifically defined input settings for the project
    private AgainstTheGrainInput input;

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
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

    private void OnEnable()
    {
        input = new AgainstTheGrainInput();
        input.Enable();
        input.Gameplay.Paint.canceled += LiftBrush;
        input.Gameplay.Paint.started += DropBrush;
        input.Gameplay.ReadInfo.performed += ReadTile;
    }

    private void OnDisable()
    {
        input.Gameplay.Paint.canceled -= LiftBrush;
        input.Gameplay.Paint.started -= DropBrush;
        input.Gameplay.ReadInfo.performed -= ReadTile;
        input.Disable();
    }

}
