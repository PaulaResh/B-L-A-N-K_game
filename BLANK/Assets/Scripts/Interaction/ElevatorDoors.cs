using UnityEngine;

public class ElevatorDoors : MonoBehaviour
{
    [Header("Двери")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Настройки анимации")]
    [Tooltip("На сколько метров открываются двери")]
    public float openDistance = 1.2f;

    [Tooltip("Скорость открытия/закрытия")]
    public float speed = 1.5f;

    [Tooltip("Задержка перед открытием дверей")]
    public float openDelay = 0.8f;

    [Tooltip("Через сколько секунд двери автоматически закроются (0 = не закрывать)")]
    public float autoCloseAfter = 4f;        // ← Новый параметр

    [Header("Триггер")]
    public bool openOnTrigger = true;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool isOpen = false;
    private float currentProgress = 0f;
    private float delayTimer = 0f;
    private float autoCloseTimer = 0f;
    private bool delayActive = false;

    private void Start()
    {
        if (leftDoor != null)
        {
            leftClosedPos = leftDoor.localPosition;
            leftOpenPos = leftClosedPos + Vector3.left * openDistance;
        }

        if (rightDoor != null)
        {
            rightClosedPos = rightDoor.localPosition;
            rightOpenPos = rightClosedPos + Vector3.right * openDistance;
        }
    }

    private void Update()
    {
        // Задержка перед открытием
        if (delayActive)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= openDelay)
            {
                delayActive = false;
                isOpen = true;
                autoCloseTimer = 0f;
            }
            return;
        }

        // Анимация
        float targetProgress = isOpen ? 1f : 0f;
        currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, Time.deltaTime * speed);

        // Обновляем позицию дверей
        if (leftDoor != null)
            leftDoor.localPosition = Vector3.Lerp(leftClosedPos, leftOpenPos, currentProgress);

        if (rightDoor != null)
            rightDoor.localPosition = Vector3.Lerp(rightClosedPos, rightOpenPos, currentProgress);

        // Автозакрытие
        if (isOpen && autoCloseAfter > 0f)
        {
            autoCloseTimer += Time.deltaTime;
            if (autoCloseTimer >= autoCloseAfter)
            {
                isOpen = false;
                autoCloseTimer = 0f;
                Debug.Log("Двери автоматически закрылись");
            }
        }
    }

    public void OpenDoors()
    {
        if (isOpen) return;

        delayTimer = 0f;
        delayActive = true;
        Debug.Log($"Двери начнут открываться через {openDelay} сек");
    }

    public void CloseDoors()
    {
        delayActive = false;
        isOpen = false;
        Debug.Log("Двери закрываются");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (openOnTrigger && other.CompareTag("Player"))
        {
            OpenDoors();
        }
    }
}