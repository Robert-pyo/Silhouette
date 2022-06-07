using UnityEngine;
using UnityEngine.Events;

public class StopMoving : MonoBehaviour
{
    public UnityEvent onStopMoving;

    public void StopActingToggle()
    {
        onStopMoving?.Invoke();
    }
}
