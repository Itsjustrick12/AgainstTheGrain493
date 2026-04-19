using UnityEngine;

public class BarnUIActivator : MonoBehaviour
{
    public BarnUIMenu barnMenu;
    private void OnEnable(){
        Barn.OnBarnInteraction += OpenMenu;
    }
    private void OnDisable(){
        Barn.OnBarnInteraction -= OpenMenu;
    }
    private void OpenMenu(){
        barnMenu.ShowMenu();
    }
}