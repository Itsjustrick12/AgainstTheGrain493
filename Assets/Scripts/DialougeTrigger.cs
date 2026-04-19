using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private UnitInteractionSystem interactionSystem;

    private void OnEnable()
    {
        DialogueManager.instance.conversationStarted += OnConversationStarted;
        DialogueManager.instance.conversationEnded += OnConversationEnded;
    }

    private void OnDisable()
    {
        if (DialogueManager.instance == null) return;
        DialogueManager.instance.conversationStarted -= OnConversationStarted;
        DialogueManager.instance.conversationEnded -= OnConversationEnded;
    }

    private void OnConversationStarted(Transform actor)
    {
        interactionSystem.DisableInputs();
    }

    private void OnConversationEnded(Transform actor)
    {
        interactionSystem.EnableInputs();
    }

    public void StartConversation(string conversationTitle)
    {
        DialogueManager.StartConversation(conversationTitle);
    }
}