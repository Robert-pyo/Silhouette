using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public ushort MaxHp { get; }
    public short CurHp { get; }
    public bool IsDead { get; }
    
    public void Hit(ushort damage);
    public void Die();
}
