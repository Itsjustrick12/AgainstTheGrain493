using PixelCrushers.DialogueSystem;
using UnityEngine.InputSystem;
using UnityEngine;

public class ContinueInput : MonoBehaviour
{
    private AgainstTheGrainInput input;

    private void OnEnable()
    {
        input = new AgainstTheGrainInput();
        input.Enable();
        // reuse whatever button feels right, select or a dedicated confirm
        input.Dialogue.Confirm.performed += OnContinue;
    }

    private void OnDisable()
    {
        input.Dialogue.Confirm.performed -= OnContinue;
        input.Disable();
    }

    private void OnContinue(InputAction.CallbackContext context)
    {
        // Only advance if dialogue is actually playing
        if (DialogueManager.IsConversationActive)
        {
            DialogueManager.PlaySequence("Continue()");
        }
    }
}