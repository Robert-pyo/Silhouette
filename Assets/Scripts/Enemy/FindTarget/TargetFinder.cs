using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetFinder
{
    protected Enemy owner;

    public abstract Transform FindTarget();
}
