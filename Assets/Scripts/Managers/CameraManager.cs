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

    [HideInInspector] public UnityEvent<int> onCameraChangeEvent;
    //[HideInInspector] public UnityEvent onCameraDirectionReset;

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            return;
        }

        OnSceneChanged();

        onCameraChangeEvent.AddListener(ChangeView);
        SceneController.Instance.onSceneChangeEvent += OnSceneChanged;
    }

    public void ChangeView(int areaID)
    {
        StartCoroutine(ChangeDirection(areaID));
    }

    // 입력이 계속되고 있는 동안에 활성 카메라가 바뀔 시 방향이 바뀌어버리는 문제로 인해
    // 코루틴으로 WaitUntil을 사용하여 구현함
    private IEnumerator ChangeDirection(int areaID)
    {
        yield return new WaitUntil(() => PlayerInput.Instance.VInput < 0.1f && PlayerInput.Instance.HInput < 0.1f);

        if (!m_currentView) yield break;
        m_currentView.gameObject.SetActive(false);

        m_dicViewArea.TryGetValue(areaID, out m_currentView);

        if (m_currentView)
        {
            m_currentView.gameObject.SetActive(true);
        }

        yield return null;
    }

    private void OnSceneChanged()
    {
        GameObject[] _vCamObjs = GameObject.FindGameObjectsWithTag("VCam");
        //Debug.Log(_vCamObjs.Length);
        List<CinemachineVirtualCamera> _vCams = new List<CinemachineVirtualCamera>();
        
        foreach (GameObject _obj in _vCamObjs)
        {
            _vCams.Add(_obj.GetComponent<CinemachineVirtualCamera>());

            if (_obj == m_currentView) continue;

            _obj.gameObject.SetActive(false);
        }

        vCamArray = _vCams.ToArray();

        m_dicViewArea.Clear();
        for (int i = 0; i < vCamArray.Length; ++i)
        {
            m_dicViewArea.Add(i, vCamArray[i]);
        }

        m_dicViewArea.TryGetValue(0, out m_currentView);

        if (m_currentView)
        {
            m_currentView.gameObject.SetActive(true);
        }
    }
}
