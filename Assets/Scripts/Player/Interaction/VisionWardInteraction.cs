using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class VisionWardInteraction : InteractionCommand
{
    public VisionWardInteraction(PlayerController player) : base(player) { }

    public override void Execute()
    {
        OnWardEnable();
        GameManager.Instance.OnWardEnabled();
    }

    public void OnWardEnable()
    {
        Wire _wire = controller.EnableWire();
        _wire.segmentStart = controller.targetObj.transform;
        _wire.segmentEnd = controller.wireTiedPosition;

        // TODO : ¿Ã∆Â∆Æ?
    }
}
