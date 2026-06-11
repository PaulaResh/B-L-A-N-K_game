using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    [Header("Note Settings")]
    [TextArea(3, 10)]
    public string noteText = "Здесь была записка...";
    public string interactionPrompt = "Прочитать записку";

    private bool isRead = false;

    public void Interact()
    {
        if (DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.ShowThought(noteText, 4f);
        }
        else
        {
            Debug.Log("[Note] " + noteText);
        }

        isRead = true;
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Paper");
    }

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    public bool CanInteract()
    {
        return true; // Can read multiple times
    }
}