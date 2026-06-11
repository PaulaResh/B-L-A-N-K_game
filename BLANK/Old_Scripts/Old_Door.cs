using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Range(0f, 1f)]
    [SerializeField] private float openChance = 0.5f;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float angle = 90f;

    private bool isOpen;

    private Quaternion closed;
    private Quaternion opened;

    void Start()
    {
        closed = transform.rotation;
        opened = Quaternion.Euler(transform.eulerAngles + new Vector3(0, angle, 0));

        isOpen = Random.value < openChance;
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            isOpen ? opened : closed,
            Time.deltaTime * speed
        );
    }

    public void Interact()
    {
        isOpen = !isOpen;
    }

    public string GetInteractionText()
    {
        return isOpen ? "Закрыть (F)" : "Открыть (F)";
    }

    public bool CanInteract() => true;
}