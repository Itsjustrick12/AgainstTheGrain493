using PixelCrushers.DialogueSystem;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public void Start()
    {
        DialogueManager.StartConversation("Tutorial/Welcome");
    }

    public void Awake()
    {
        
    }

    public void UnitSelected()
    {
        //when the unit is first selected, pause the game and explain stuff about the unit
    }

    public void OnPlantSelected()
    {

    }

    public void CropPlanted()
    {

    }

    public void CropWatered()
    {

    }

    public void CropHarvested()
    {

    }

    public void BarnOpened()
    {

    }

    public void AnimalPurchased()
    {

    }

    public void OnEnemyTurnStart()
    {

    }

}
