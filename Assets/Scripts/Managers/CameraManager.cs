using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    private static CameraManager s_instance;
    public static CameraManager Instance => s_instance;

    public Cinemachine.CinemachineVirtualCamera[] vCamArray;

    // AreaID, VCAM
    private readonly Dictionary<int, Cinemachine.CinemachineVirtualCamera> m_dicViewArea = new Dictionary<int, Cinemachine.CinemachineVirtualCamera>();
    [SerializeField] private Cinemachine.CinemachineVirtualCamera m_currentView;

    [HideInInspector]
    public UnityEvent<int> onCameraChangeEvent;

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }

        // dictionary에 Virtual Camera 추가
        for (int i = 0; i < vCamArray.Length; ++i)
        {
            m_dicViewArea.Add(i, vCamArray[i]);
        }

        onCameraChangeEvent.AddListener(ChangeView);

        m_dicViewArea.TryGetValue(0, out m_currentView);
    }

    private void ChangeView(int areaID)
    {
        m_currentView.gameObject.SetActive(false);

        m_dicViewArea.TryGetValue(areaID, out m_currentView);

        if (m_currentView)
        {
            m_currentView.gameObject.SetActive(true);
        }
    }
}
