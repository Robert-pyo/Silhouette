using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    private static CameraManager s_instance;
    public static CameraManager Instance => s_instance;

    public CinemachineVirtualCamera[] vCamArray;

    // AreaID, VCAM
    private readonly Dictionary<int, CinemachineVirtualCamera> m_dicViewArea = new Dictionary<int, CinemachineVirtualCamera>();
    [SerializeField] private CinemachineVirtualCamera m_currentView;

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

        OnSceneChanged();

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

    private void OnSceneChanged()
    {
        GameObject[] _vCamObjs = GameObject.FindGameObjectsWithTag("VCam");
        List<CinemachineVirtualCamera> _vCams = new List<CinemachineVirtualCamera>();
        
        foreach (GameObject _obj in _vCamObjs)
        {
            _vCams.Add(_obj.GetComponent<CinemachineVirtualCamera>());
        }

        vCamArray = _vCams.ToArray();
    }
}
