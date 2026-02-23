using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum InteractionState
{
    Selection,
    Movement,
    ActionSelection,
    TargetSelection,
    Execution
}

//This is needed to call events and recieve callbacks from UI buttons
[System.Serializable]
public class UnitActionEvent : UnityEvent<UnitAction>
{
}

//This script is responsible for handling the interactions between units
//This includes movement, crop interactions, attacking, and more
public class UnitInteractionSystem : MonoBehaviour
{
    public TileManager tileManager;

    //Stores the location of the current tile selected
    public TileCursor cursor;
    [SerializeField]private ActionMenu actionMenu;

    public InteractionState state;

    //Get reference to the unit/location we are currently interacting with
    public Vector3Int selectedPosition;
    public Entity selectedEntity;

    public Transform hoverTransform;
    public SpriteRenderer hoverSprite;

    private Vector3 offset = new Vector3(0.5f, 0.75f, 0);

    //Change this later but for now default Unity UI interactions is good
    private DefaultInputActions input;

    public bool makingDecision = false;

    public void Awake()
    {
        cursor = FindFirstObjectByType<TileCursor>();
        tileManager = FindFirstObjectByType<TileManager>();
        //actionMenu = FindFirstObjectByType<ActionMenu>();
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

    //Transitions through the state machine to determine what action to take
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
                        ShowUnitOptions(toData.GetOccupyingEntity());
                        return;
                    }
                    //Only place if entity can go to new tile
                    else if (toData.CanPlaceEntity())
                    {
                        ClearHoverSprite();
                        tileManager.MoveEntity(selectedPosition, pos);

                        //For now just switch to selection
                        ShowUnitOptions(toData.GetOccupyingEntity());
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
            case InteractionState.ActionSelection:
                //do action logic for determining nearby tiles here
                //temp, just go to state
                //Attempt the action
                Unit unitCheck = selectedEntity as Unit;
                if (unitCheck != null)
                {
                    //do the action
                    List<UnitAction> actions = unitCheck.GetAvailableActions();
                    //Select the proper action
                    //todo add this logic
                    //ExecuteAction(actions[0], pos);
                }
                break;
            case InteractionState.TargetSelection:

            default:
                break;
        }
    }
    public void ExecuteAction(UnitAction action, Vector3Int pos)
    {
        if (selectedEntity is Unit unit)
        {
            if (action.IsPossible(unit))
            {
                //action.PerformAt(unit, pos);
                state = InteractionState.Selection;
            }
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

    private void ShowUnitOptions(Unit unit)
    {
        if (unit == null)
        {
            Debug.LogError("THERES NO UNIT TO SHOW OPTIONS FOR");
            return;
        }
        state = InteractionState.ActionSelection;
        actionMenu.ShowMenu(unit);
        makingDecision = true;
    }

    private void ShowUnitOptions(Entity entity)
    {
        Unit unit = entity as Unit;
        if (unit == null)
        {
            Debug.LogError("This isn't a unit, can't show options");
            return;
        }
        ShowUnitOptions(unit);
    }

    private void OnEnable()
    {
        input = new DefaultInputActions();
        input.Enable();
        input.UI.Submit.performed += OnSelect;
    }

    private void OnDisable()
    {
        input.UI.Submit.performed -= OnSelect;
        input.Disable();
    }
}
