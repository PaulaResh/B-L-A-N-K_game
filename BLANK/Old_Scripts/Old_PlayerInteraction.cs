using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float distance = 3f;

    private Camera cam;
    private IInteractable current;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Check();

        if (current != null && Input.GetKeyDown(KeyCode.F))
        {
            current.Interact();
        }

        if (Input.GetKeyDown(KeyCode.Q))
            Inventory.Instance.Previous();

        if (Input.GetKeyDown(KeyCode.E))
            Inventory.Instance.Next();
    }

    void Check()
    {
        current = null;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            current = hit.collider.GetComponentInParent<IInteractable>();
        }
    }
}