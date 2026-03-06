using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

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
public class UnitInteractionSystem : TileCursor
{
    public TileManager tileManager;

    public Tilemap optionsMap;
    public TileBase optionTile;
    //Stores the location of the current tile selected
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
    private AgainstTheGrainInput input;

    [SerializeField] List<Vector3Int> validLocations;
    private EntityAction currAction;

    private int nextUnitID = -1;

    public bool isInputOn = true;

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        //actionMenu = FindFirstObjectByType<ActionMenu>();
        actionMenu.OnActionSelected.AddListener(SelectAction);
        state = InteractionState.Selection;
        BarnUIMenu.OnUnitPurchased.AddListener(SetNextUnit);
        BarnUIMenu.OnPurchaseComplete.AddListener(SelectAction);
        BarnUIMenu.CancelAction.AddListener(StopAction);

        validLocations = new List<Vector3Int>();
    }
    //Restrict to only display updated tiles
    protected override void HandleCursor()
    {
        if (!isInputOn) return;

        Vector3Int tile = GetMouseTile();

        if (showHighlight && tile != currentTile && targetMap.HasTile(tile))
        {
            if ((validLocations.Count==0) || (validLocations.Count > 0 && IsInRange(tile)))
            {
                DeselectLast();
                currentTile = tile;
                SelectCurrent(tile);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        //Move the hoverSprite to the currently selected location
        hoverTransform.position = GetCurrentTile() + offset;
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
        if (selectedEntity != null && selectedEntity.IsActive())
        {

            return true;
        }
        return false;
    }

    //Transitions through the state machine to determine what action to take
    public void OnSelect(InputAction.CallbackContext context)
    {
        Vector3Int pos = GetCurrentTile();
        switch (state)
        {
            case InteractionState.Selection:
                //Attempt to set the selected Unit on the tile
                if (AttemptSelection(pos))
                {
                    //If above is true, get reference to entity and switch to movement phase
                    selectedPosition = pos;
                    selectedEntity = tileManager.GetTileDataAt(pos).GetOccupyingEntity();
                    if (selectedEntity != null && selectedEntity is Unit)
                    {
                        Unit unit = selectedEntity as Unit;
                        if (unit.isEnemy) return;
                        validLocations = unit.GetMovementRange();
                        
                        LiftHoverSprite();
                        foreach(Vector3Int tile in validLocations)
                        {
                            optionsMap.SetTile(tile, optionTile);
                        }
                        state = InteractionState.Movement;
                        return;
                    }
                    else if (selectedEntity != null && selectedEntity is Structure)
                    {
                        //do structure interaction, skip movement interaction
                        Structure structureCheck = selectedEntity as Structure;
                        List<EntityAction> actions = structureCheck.GetAvailableActions();

                        // Store the SpawnUnitAction as currAction so SelectAction can use it later
                        // when OnPurchaseComplete fires after the player picks a unit in the UI
                        if (actions != null && actions.Count > 0)
                        {
                            currAction = actions[0]; // assumes first action is SpawnUnitAction
                            state = InteractionState.ActionSelection;
                            structureCheck.Interact();
                        }
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
                    optionsMap.ClearAllTiles();
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

    public bool IsInRange(Vector3Int pos)
    {
        return validLocations.Contains(pos);
    }

    public bool AttemptTarget(Vector3Int pos)
    {
        if (currAction == null) return false;
        //Check that the position you clicked is a valid target
        if (validLocations.Count > 0 && validLocations.Contains(pos))
        {
            //Execute the action
            //Need to remain general
            currAction.PerformAt(selectedEntity as Entity, pos);
            selectedEntity.Deactivate();
            ResetData();
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

    public void SetNextUnit(int id)
    {
        nextUnitID = id;
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

    public void StopAction()
    {
        ResetData();

        //If unit was moved but not finalized, move it back
        if (afterLocation != prevLocation && (afterLocation != null && prevLocation != null))
        {
            tileManager.MoveEntity(afterLocation, prevLocation);
        }

        state = InteractionState.Selection;
    }

    private void ResetData()
    {
        //Clear tile highlights
        optionsMap.ClearAllTiles();

        validLocations.Clear();
        selectedEntity = null;
        selectedPosition = new Vector3Int(0, 0, -1);

        //Clear hover
        ClearHoverSprite();

        //Reset action
        currAction = null;
    }

    public void SelectAction()
    {
        SelectAction(currAction);
    }

    public void SelectAction(EntityAction action)
    {
        //Check for unique UI ish actions for not doing direct things
        if (action == null)
        {
            Debug.LogError("NO ACTION SELECTED!");
        }
        //once this is called, shift to tile selection based on target tiles
        if (action is WaitAction)
        {
            //Don't do anything, consider the unit moved and don't do anything else
            state = InteractionState.Selection;
            selectedEntity.Deactivate();
            ResetData();
            return;
        }
        else if (action is Cancel)
        {

            //Undo the movement from the previous action and return
            tileManager.MoveEntity(afterLocation, prevLocation);
            state = InteractionState.Selection;
            ResetData();
            return;
        }
        //Special case, requires picking an integer set by a UI (barnUI probably)
        if (action is SpawnUnitAction)
        {
            Debug.Log("SpawnUnit found!");
            SpawnUnitAction spawnAction = action as SpawnUnitAction;
            spawnAction.SetSpawnUnit(nextUnitID);
        }

        currAction = action;

        //Switch to target selection phase
        //Update the valid locations
        validLocations = action.GetValidTargets(selectedEntity);
        //Highlight the selectable locations
        foreach (Vector3Int pos in validLocations)
        {
            optionsMap.SetTile(pos, optionTile);
        }
        //Otherwise, perform the action on the selected tile
        state = InteractionState.TargetSelection;
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
        input = new AgainstTheGrainInput();
        input.Enable();
        input.Gameplay.Select.performed += OnSelect;
    }

    private void OnDisable()
    {
        input.Gameplay.Select.performed -= OnSelect;
        input.Disable();
    }

    public void DisableInputs()
    {
        isInputOn = false;
        input.Disable();
    }

    public void EnableInputs()
    {
        isInputOn = true;
        input.Enable();
    }
}
