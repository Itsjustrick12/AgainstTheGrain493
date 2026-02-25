using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

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
public class EntityActionEvent : UnityEvent<EntityAction>
{
}

//This script is responsible for handling the interactions between units
//This includes movement, crop interactions, attacking, and more
public class UnitInteractionSystem : MonoBehaviour
{
    public TileManager tileManager;

    public Tilemap optionsMap;
    public TileBase optionTile;
    //Stores the location of the current tile selected
    public TileCursor cursor;
    [SerializeField]private ActionMenu actionMenu;

    public InteractionState state;

    //Get reference to the unit/location we are currently interacting with
    public Vector3Int selectedPosition;

    //Needed for reseting movement with cancel action
    public Vector3Int prevLocation;
    public Vector3Int afterLocation;

    public Entity selectedEntity;

    public Transform hoverTransform;
    public SpriteRenderer hoverSprite;

    private Vector3 offset = new Vector3(0.5f, 0.75f, 0);

    //Change this later but for now default Unity UI interactions is good
    private DefaultInputActions input;

    [SerializeField] List<Vector3Int> validLocations;
    private EntityAction currAction;

    public void Awake()
    {
        cursor = FindFirstObjectByType<TileCursor>();
        tileManager = FindFirstObjectByType<TileManager>();
        //actionMenu = FindFirstObjectByType<ActionMenu>();
        actionMenu.OnActionSelected.AddListener(SelectAction);
        state = InteractionState.Selection;
    }

    public void Update()
    {
        Vector3Int currPos = cursor.GetCurrentTile();
        //Move the hoverSprite to the currently selected location
        hoverTransform.position = currPos + offset;
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
        if (selectedEntity.IsActive())
        {

            return true;
        }
        return false;
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
                    if (selectedEntity is Unit)
                    {
                        LiftHoverSprite();
                        state = InteractionState.Movement;
                        return;
                    }
                    else if (selectedEntity is Structure)
                    {
                        //do structure interaction
                        Structure structureCheck = selectedEntity as Structure;
                        structureCheck.Interact();
                        state = InteractionState.ActionSelection;
                    }
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
                if (selectedEntity != null && toData != null)
                {

                    //check they if they're the same
                    if (fromData == toData)
                    {
                        //Just place at same place and move on
                        ClearHoverSprite();
                        //switch to interaction selection phase
                        ShowUnitOptions(toData.GetOccupyingEntity());

                        //Needed for the cancel action to work
                        prevLocation = selectedPosition;
                        afterLocation = pos;
                        return;
                    }
                    //Only place if entity can go to new tile
                    else if (toData.CanPlaceEntity())
                    {
                        ClearHoverSprite();
                        tileManager.MoveEntity(selectedPosition, pos);

                        //For now just switch to selection
                        ShowUnitOptions(toData.GetOccupyingEntity());
                        //Needed for the cancel action to work
                        prevLocation = selectedPosition;
                        afterLocation = pos;
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
            case InteractionState.TargetSelection:
                if (!currAction.IsAOE() && AttemptTarget(pos))
                {
                    state = InteractionState.Selection;
                }
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

    public bool AttemptTarget(Vector3Int pos)
    {
        //Check that the position you clicked is a valid target
        if (validLocations.Count > 0 && validLocations.Contains(pos))
        {
            //Execute the action
            currAction.PerformAt(selectedEntity as Unit, pos);
            selectedEntity.Deactivate();
            optionsMap.ClearAllTiles();
            return true;
        }
        return false; 
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
    }


    public void SelectAction(EntityAction action)
    {
        //Check for unique UI ish actions for not doing direct things

        //once this is called, shift to tile selection based on target tiles
        if (action.GetName() == "Wait")
        {
            //Don't do anything, consider the unit moved and don't do anything else
            state = InteractionState.Selection;
            selectedEntity.Deactivate();
            return;
        }
        else if (action.GetName() == "Cancel")
        {
            //Undo the movement from the previous action and return
            tileManager.MoveEntity(afterLocation, prevLocation);
            state = InteractionState.Selection;
            return;
        }
        else
        {
            currAction = action;

            //Switch to target selection phase
            //Update the valid locations
            validLocations = action.GetValidTargets(selectedEntity as Unit);
            //Highlight the selectable locations
            foreach (Vector3Int pos in validLocations)
            {
                optionsMap.SetTile(pos, optionTile);
            }
            //Otherwise, perform the action on the selected tile
            state = InteractionState.TargetSelection;
        }
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
