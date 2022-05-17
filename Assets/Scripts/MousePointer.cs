using UnityEngine;

public class MousePointer : MonoBehaviour
{
    public Camera cam;

    public LayerMask hitLayer;
    //public GameObject targetedObj;
    private RaycastHit _rayHit;

	void Update () 
	{
        // 마우스 포인터 포지션으로 이동
        RayPointer();
	}

    private void RayPointer()
    {
        Ray _ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_ray, out _rayHit, float.MaxValue, hitLayer))
        {
            transform.position = _rayHit.point;
            
            // 플레이어 상태가 상호작용 가능한 상태일 때만 타겟 선정
            //if (!GameManager.Instance.player.isInteractable) return;
            //targetedObj = FindRayOnItemOrNull();
        }
    }

    // private GameObject FindRayOnItemOrNull()
    // {
    //     return m_rayHit.transform.gameObject.layer == LayerMask.NameToLayer("Item") ? m_rayHit.transform.gameObject : null;
    // }
}
