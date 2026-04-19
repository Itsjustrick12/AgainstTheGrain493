using PixelCrushers.DialogueSystem;
using static UnityEngine.CullingGroup;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    //[SerializeField] private TutorialDialogueTrigger dialogueTrigger;
    [SerializeField] private bool tutorialEnabled = true;

    private HashSet<string> completedSteps = new HashSet<string>();
    public static TutorialManager Instance;

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
    }

}