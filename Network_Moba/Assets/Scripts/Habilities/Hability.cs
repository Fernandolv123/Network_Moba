using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hability : ScriptableObject
{
    public float range;
    public float damage;
    public float wide;
    public float coolDown;
    public bool canCast = true;
    public abstract void Cast();
    public abstract void HabilityReady();
}
