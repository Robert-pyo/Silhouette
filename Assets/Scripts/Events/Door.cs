using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IEventReceiver
{
    private Animator m_anim;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();

        GameManager.Instance.conditionCompleteEvent = ExecuteEvent;
    }

    public void ExecuteEvent(bool condition)
    {
        if (condition)
        {
            m_anim.SetTrigger("OnDoorOpen");
            return;
        }

        m_anim.SetTrigger("OnDoorClose");
    }
}
