using UnityEngine;

public class JobBoardUIActivator : MonoBehaviour
{
    public BarnUIMenu barnMenu;
    private void OnEnable()
    {
        Barn.OnBarnInteraction += OpenMenu;
    }
    private void OnDisable()
    {
        Barn.OnBarnInteraction -= OpenMenu;
    }
    private void OpenMenu()
    {
        barnMenu.ShowMenu();
    }
}
