using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InteractionState
{
    Selection,
    Movement,
    Action
}

//This script is responsible for handling the interactions between units
//This includes movement, crop interactions, attacking, and more
public class UnitInteractionSystem : MonoBehaviour
{
    public TileManager tileManager;

    //Stores the location of the current tile selected
    public TileCursor cursor;

    public InteractionState state;

    //Get reference to the unit/location we are currently interacting with
    public Vector3Int selectedPosition;
    public Entity selectedEntity;

    public Transform hoverTransform;
    public SpriteRenderer hoverSprite;

    private Vector3 offset = new Vector3(0.5f, 0.75f, 0);

    private AgainstTheGrainInput input;

    public void Awake()
    {
        cursor = FindFirstObjectByType<TileCursor>();
        tileManager = FindFirstObjectByType<TileManager>();
        state = InteractionState.Selection;
    }

    public void Update()
    {
        //Move the hoverSprite to the currently selected location
        hoverTransform.position = cursor.GetCurrentTile() + offset;
    }

    public bool AttemptSelection(Vector3Int pos)
    {
         
        TileData data = tileManager.GetTileDataAt(pos);

        if (data == null || !data.HasOccupant())
        {
            return false;
        }

        selectedPosition = pos;
        selectedEntity = data.GetOccupyingEntity();
        return true;
    }

    public void MoveEntity(Vector3Int newPos)
    {
        //Swap the location of the currently selected Entity to the tile passed

        //Do a temp like swap for the position
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        Vector3Int pos = cursor.GetCurrentTile();
        switch (state)
        {
            case InteractionState.Selection:
                //Attempt to set the selected Unit on the tile
                if (AttemptSelection(pos))
                {
                    //If above is true, get reference to entity and switch to movement phase
                    selectedPosition = pos;
                    selectedEntity = tileManager.GetTileDataAt(pos).GetOccupyingEntity();
                    LiftHoverSprite();
                    state = InteractionState.Movement;
                    return;
                }
                else
                {
                    selectedEntity = null;
                }
                break;
            case InteractionState.Movement:
                TileData fromData = tileManager.GetTileDataAt(selectedPosition);
                TileData toData = tileManager.GetTileDataAt(pos);
                //if selected tile, try to place the unit at the current location to see if it works
                if (selectedEntity!=null && toData != null)
                {

                    //check they if they're the same
                    if (fromData == toData)
                    {
                        //Just place at same place and move on
                        ClearHoverSprite();
                        //switch to interaction selection phase
                        state = InteractionState.Selection;
                        return;
                    }
                    //Only place if entity can go to new tile
                    else if (toData.CanPlaceEntity())
                    {
                        ClearHoverSprite();
                        tileManager.MoveEntity(selectedPosition, pos);
                        //For now just switch to selection
                        state = InteractionState.Selection;
                        return;
                    }
                }
                else
                {
                    //Place the unit back or just dont do anything
                    ClearHoverSprite();
                    state = InteractionState.Selection;

                }
                break;
            case InteractionState.Action:
                //do action logic for determining nearby tiles here
                //temp, just go to state
                state = InteractionState.Selection;
                break;
            default:
                break;
        }
    }
    //Call upon selected a unit in movement
    public void LiftHoverSprite()
    {
        if (selectedEntity!= null)
        {
            selectedEntity.HideSprite();
            hoverSprite.sprite = selectedEntity.GetSprite();
        }

    }
    //Clear before placement
    public void ClearHoverSprite()
    {
        if (selectedEntity != null)
        {
            selectedEntity.ShowSprite();
        }
        hoverSprite.sprite = null;
    }

    private void OnEnable()
    {
        input = new AgainstTheGrainInput();
        input.Enable();
        input.Gameplay.ReadTile.performed += OnSelect;
    }

    private void OnDisable()
    {
        input.Gameplay.ReadTile.performed -= OnSelect;
        input.Disable();
    }
}
