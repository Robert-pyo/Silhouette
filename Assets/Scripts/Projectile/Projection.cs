using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _obstaclesParent;

    private void Start()
    {
        CreatePhysicsScene();
    }

    void CreatePhysicsScene()
    {
        _simulationScene =
            SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform _obj in _obstaclesParent)
        {
            var _ghostObj = Instantiate(_obj.gameObject, _obj.position, _obj.rotation);
            _ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(_ghostObj, _simulationScene);
        }
    }

    public LineRenderer lineRenderer;
    [SerializeField] private int _maxPhysicsFrameIterations;
    public void SimulateTrajectory(Rock rockPrefab, Vector3 startPos, Vector3 velocity)
    {
        // 고스트 오브젝트 생성
        var _ghostObj = Instantiate(rockPrefab, startPos, Quaternion.identity);
        _ghostObj.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(_ghostObj.gameObject, _simulationScene);
        
        // PhysicsScene에서 AddForce
        _ghostObj.Init(velocity, true);

        // 라인 렌더러의 점의 개수는 프레임당 최대 반복횟수 만큼
        lineRenderer.positionCount = _maxPhysicsFrameIterations;

        // Time.fixedDeltaTime마다 시뮬레이션 해주고 궤적을 표현해줄
        // LineRenderer의 점 포지션을 _ghostObj가 프레임마다 시뮬레이션 된 포지션으로 지정해주기 
        for (int i = 0; i < _maxPhysicsFrameIterations; ++i)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, _ghostObj.transform.position);
        }
        
        // 궤적 표현이 완료되었으면 파괴
        Destroy(_ghostObj.gameObject);
    }
}
