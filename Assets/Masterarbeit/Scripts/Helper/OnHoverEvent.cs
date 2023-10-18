using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onHover = new UnityEvent();
    Coroutine hoveringCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartHoveringCoroutine();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopHoveringCoroutine();
    }

    void OnDestroy()
    {
        StopHoveringCoroutine();
    }

    void StartHoveringCoroutine()
    {
        StopHoveringCoroutine();
        hoveringCoroutine = StartCoroutine(HoveringCoroutine());
    }


    void StopHoveringCoroutine()
    {
        if(hoveringCoroutine != null)
        {
            StopCoroutine(hoveringCoroutine);
        }
    }

    IEnumerator HoveringCoroutine()
    {
        while(true)
        {
            onHover.Invoke();
            yield return null;
        }
    }
}
