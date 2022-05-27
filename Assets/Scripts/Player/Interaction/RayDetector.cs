using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class RayDetector : InteractDetectStrategy
{
    private PlayerController m_player;
    private Transform m_raycastOrigin;
    private float m_rayDistance;

    public RayDetector(PlayerController player)
    {
        m_player = player;
        m_raycastOrigin = player.detectOrigin;
        m_rayDistance = player.detectDistance;
    }

    public override bool CanInteract()
    {
        if (Physics.Raycast(m_raycastOrigin.position, m_raycastOrigin.forward, out var _hit, m_rayDistance, LayerMask.GetMask("Interactable")))
        {
            switch (_hit.transform.tag)
            {
                case "PushOrPull":
                {
                    m_player.interactionType = EInteractionType.PushOrPull;
                }
                break;

                case "Item":
                {
                    m_player.interactionType = EInteractionType.Item;
                }
                break;

                case "VisionWard":
                    {
                        m_player.interactionType = EInteractionType.VisionWard;
                    }
                    break;

                default:
                break;
            }

            m_player.targetObj = _hit.transform.gameObject;

            return true;
        }

        return false;
    }
}
