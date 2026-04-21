using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
//code referenced from: https://www.youtube.com/watch?v=R6scxu1BHhs&t=56s&ab_channel=ShackMan
public class CameraController : MonoBehaviour
{
    public Camera cam;
    private Vector3 dragOrigin;
    private Vector3 targetPos;

    public float minX = -1;
    public float maxX = 1;
    public float minY = -1;
    public float maxY = 1;

    //needed to prevent camera from jerking around
    public float smoothing = 0.125f;

    public bool levelEditor = false;

    //used for clamping to box shaped screen
    [SerializeField] bool clampToBox = true;
    [SerializeField] MapSize mapSize = MapSize.SMALL;
    AgainstTheGrainInput inputActions;
    [SerializeField] private GameManager gameManager;

    //needed for determining whether or not the camera can be moved due to UI freezing

    public void Start()
    {
        mapSize = gameManager.mapSize;
        UpdateCameraBounds(mapSize);

    }

    private void OnEnable()
    {
        inputActions = new AgainstTheGrainInput();
        inputActions.Gameplay.Next.performed += FocusOnNextUnit;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Next.performed -= FocusOnNextUnit;
        inputActions.Disable();
    }

    public void Update()
    {
        if (gameManager.isPlayerTurn)
        {
            MoveCamera();   
        }
    }
    //When called, find the next active unit you can move and focus the camera on it
    public void FocusOnNextUnit(InputAction.CallbackContext context)
    {
        FocusOnNextUnit();
    }

    public void FocusOnNextUnit()
    {
        //get unit position and move camera to it
        Unit unit = gameManager.GetNextActiveUnit();
        if (unit != null)
        {
            StartCoroutine(FocusOnPosition(unit.GetGridPos(), 0.25f));
        }
    }

    public void PanToNextUnit()
    {
        FocusOnNextUnit();
    }

    public void FocusOnTilePosition(Vector3Int pos, float time = 0.5f)
    {
        StartCoroutine(FocusOnPosition(pos, time));
    }

    public void MoveCamera()
    {
        // Prevent dragging when clicking on UI
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        // Get mouse position from Input System
        Vector2 mouseScreenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);

        // When drag starts
        if (inputActions.Gameplay.Paint.WasPressedThisFrame())
        {
            dragOrigin = mouseWorldPos;
        }

        // While holding drag
        if (inputActions.Gameplay.Paint.IsPressed() && !levelEditor)
        {
            Vector3 diff = dragOrigin - mouseWorldPos;

            targetPos = cam.transform.position + diff;

            // Clamp to bounds
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            targetPos.z = cam.transform.position.z;

            // Smooth movement
            cam.transform.position = Vector3.Lerp(
                cam.transform.position,
                targetPos,
                smoothing
            );
        }
    }

    public void UpdateCameraBounds(MapSize size)
    {
        mapSize = size;
        if (clampToBox)
        {
            float camYSize = cam.orthographicSize * 2;
            float camXSize = camYSize * (16 / 9.0f);
            float boxSize = GameConstants.MapSizeToInt(size) + (GameConstants.SCREEN_BORDER_THICKNESS)*2;
            minX = Mathf.Abs(boxSize - camXSize) / -2;
            maxX = Mathf.Abs(boxSize - camXSize) / 2;
            minY = Mathf.Abs(boxSize - camYSize) / -2;
            maxY = Mathf.Abs(boxSize - camYSize) / 2;
        }
    }

    public void FocusOnRobot()
    {
        Unit robot = gameManager.GetAllEnemyUnits()[0]; // however you get your robot
        if (robot != null)
        {
            StartCoroutine(FocusOnPosition(robot.GetGridPos(), 0.5f));
        }
    }

    public void PanToPosition(string coords)
    {
        Debug.Log("PanToPosition called with: " + coords);
        var parts = coords.Split('|');
        var pos = new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        FocusOnTilePosition(pos, 0.5f);
    }

    public void MoveToPosition(Vector3 pos)
    {
        this.transform.position = pos;
    }
    //Used by the game manager to focus on a target unit
    //Used AI for this full transparency
    public IEnumerator FocusOnPosition(Vector3Int target, float duration = 0.5f)
    {
        //Start
        Vector3 start = cam.transform.position;
        Vector3 end = new Vector3(
            Mathf.Clamp(target.x, minX, maxX),
            Mathf.Clamp(target.y, minY, maxY),
            cam.transform.position.z
        );

        //Slowly move the camera to the desired position
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cam.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        //Move the camera
        cam.transform.position = end;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        //updown
        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(minX, maxY));
        Gizmos.DrawLine(new Vector3(maxX, minY), new Vector3(maxX, maxY));
        //left right
        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(maxX, minY));
        Gizmos.DrawLine(new Vector3(minX, maxY), new Vector3(maxX, maxY));
    }


}
