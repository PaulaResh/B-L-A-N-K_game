using UnityEngine;

public interface IInteractable
{
    void Interact();
    string GetInteractionText();   // что писать на экране ("Открыть дверь", "Взять ключ" и т.д.)
    bool CanInteract();            // можно ли сейчас взаимодействовать
}