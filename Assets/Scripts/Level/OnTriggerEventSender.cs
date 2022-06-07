using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEventSender : MonoBehaviour
{
    public GameObject targetObject;

    public UnityEvent onTriggerEnterEvent;
    //public UnityEvent onTriggerStayEvent;
    public UnityEvent onTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetObject.tag)) return;

        onTriggerEnterEvent?.Invoke();
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (!other.CompareTag(targetObject.tag)) return;

    //    onTriggerStayEvent?.Invoke();
    //}

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(targetObject.tag)) return;

        onTriggerExitEvent?.Invoke();
    }
}
