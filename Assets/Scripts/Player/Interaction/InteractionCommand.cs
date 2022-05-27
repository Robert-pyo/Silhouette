using System.Collections;
using System.Collections.Generic;
using Player;

public abstract class InteractionCommand
{
    protected PlayerController controller;

    public InteractionCommand(PlayerController player)
    {
        controller = player;
    }

    public abstract void Execute();
}
