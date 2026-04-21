using PixelCrushers.DialogueSystem;
using static UnityEngine.CullingGroup;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    //[SerializeField] private TutorialDialogueTrigger dialogueTrigger;
    [SerializeField] private bool tutorialEnabled = true;

    public static TutorialManager Instance;

    private bool robotMoved = false;

    private void OnEnable()
    {
        UnitInteractionSystem.OnUnitSelected += OnUnitClicked;
        //UnitInteractionSystem.OnUnitMoved += OnUnitClicked;
    }

    private void Awake()
    {
        Instance = this;
        if (!tutorialEnabled)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void OnUnitClicked()
    {
        DialogueManager.StartConversation("Tutorial/FirstUnitClick");
        UnitInteractionSystem.OnUnitSelected -= OnUnitClicked;
    }

    public void OnRobotMove()
    {
        robotMoved = true;
        DialogueManager.StartConversation("Tutorial/RobotMove");
    }

}