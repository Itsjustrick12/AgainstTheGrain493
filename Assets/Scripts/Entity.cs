using UnityEngine;

//This is the base script that is used for all things that can occupy tiles in the game
//This includes units, obstacles, interactable objects, and crops
public class Entity : MonoBehaviour
{
    //Stores the location of where this entity actually is
    private Vector3Int gridPos;
    //Determines where or not something can pathfind through the tile this entity is on
    [SerializeField] private bool isObstacle;
    //Determines if this entity can be clicked on or affected in any way
    [SerializeField] private bool isInteractable;
    //Helper function for all derived entities to use to determine whether or not something is occupying the tile
    public Vector3Int GetGridPos()
    {
        return gridPos;
    }
    public void SetGridPos(Vector3Int pos)
    {
        gridPos = pos;
    }
    public virtual bool IsObstacle()
    {
        return isObstacle;
    }
    public void SetIsObstacle(bool obstacle)
    {
        isObstacle = obstacle;
    }
    public bool IsInteractable()
    {
        return isInteractable;
    }

    public void UpdateTransform(Vector3Int pos)
    {
        //Update the Transform to refelct the gameobject visually
        this.gameObject.transform.position = pos + (new Vector3(0.5f, 0.5f, 0f));
    }
}
