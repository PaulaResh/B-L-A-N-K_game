using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Чувствительность мыши")]
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("Камера")]
    [SerializeField] private Transform playerCamera;   // сюда перетащи дочернюю камеру

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;           // вертикальный поворот камеры
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Прячем и блокируем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera == null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        // Проверка земли
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;   // прилипаем к земле

        // === ДВИЖЕНИЕ ===
        float x = Input.GetAxis("Horizontal");   // A/D
        float z = Input.GetAxis("Vertical");     // W/S

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Движение относительно направления взгляда
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // === ПРЫЖОК ===
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Гравитация
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // === СМОТР МЫШКОЙ ===
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Вертикальный поворот камеры (с ограничением)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Горизонтальный поворот всего игрока
        transform.Rotate(Vector3.up * mouseX);

        // Выход из режима (Esc)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}