using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class UpEvent : UnityEvent<bool> { }

[RequireComponent(typeof(Selectable))]

public class PointerUpEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UpEvent pointerUpEvent;

    private Selectable selectable;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectable.IsInteractable())
            pointerUpEvent?.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectable.IsInteractable())
            pointerUpEvent?.Invoke(false);
    }


}
