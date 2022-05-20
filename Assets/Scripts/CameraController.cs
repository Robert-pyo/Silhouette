using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera m_vCam;

    public float rotateSpeed;

    private void Awake()
    {
        m_vCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void LateUpdate()
    {
        RollLeft();
        RollRight();
    }
    
    private void RollLeft()
    {
        if (!Input.GetKey(KeyCode.Q)) return;
        
        transform.RotateAround(m_vCam.Follow.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void RollRight()
    {
        if (!Input.GetKey(KeyCode.E)) return;
        
        transform.RotateAround(m_vCam.Follow.position, Vector3.up, -rotateSpeed * Time.deltaTime);
    }
}
