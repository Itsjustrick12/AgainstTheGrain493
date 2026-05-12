using UnityEngine;

public class JobBoardUIActivator : MonoBehaviour
{
    public JobBoardUI jobBoard;
    private void OnEnable()
    {
        Farmhouse.OnFarmhouseInteraction += OpenMenu;
    }
    private void OnDisable()
    {
        Farmhouse.OnFarmhouseInteraction -= OpenMenu;
    }
    private void OpenMenu()
    {
        jobBoard.ShowMenu();
    }
}
