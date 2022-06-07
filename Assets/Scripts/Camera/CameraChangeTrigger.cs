using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CameraChangeTrigger : MonoBehaviour
{
    [SerializeField] private int areaID;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CameraManager.Instance.onCameraChangeEvent?.Invoke(areaID);
        Debug.Log("Camera Change Event Called!");
    }
}
