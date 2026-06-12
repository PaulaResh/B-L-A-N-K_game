using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    [Header("UI References (optional)")]
    public Text interactionPromptText; // Assign in Inspector or leave null

    private Camera playerCamera;
    private IInteractable currentInteractable;

    private void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.F) && currentInteractable != null && currentInteractable.CanInteract())
        {
            currentInteractable.Interact();
        }
    }

    private void CheckForInteractable()
    {
        currentInteractable = null;

        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                currentInteractable = interactable;
                ShowPrompt(currentInteractable.GetInteractionPrompt());
                return;
            }
        }

        HidePrompt();
    }

    private void ShowPrompt(string prompt)
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.text = prompt + " [F]";
            interactionPromptText.enabled = true;
        }
        else
        {
            Debug.Log("[Interaction] " + prompt + " [F]");
        }
    }

    private void HidePrompt()
    {
        if (interactionPromptText != null)
            interactionPromptText.enabled = false;
    }
}