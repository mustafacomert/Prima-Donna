using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public abstract void Move();
    public abstract void Idle();
    public abstract void AreaAttack();
    public abstract void DirectAttack();
    public abstract void Die();
}
