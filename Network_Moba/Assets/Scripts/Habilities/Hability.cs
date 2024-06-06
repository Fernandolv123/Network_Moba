using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;

public abstract class Hability : ScriptableObject
{
    public float range;
    public float damage;
    public float wide;
    public float coolDown;
    public bool canCast = true;
    protected Transform castBarPosition;
    [Command] public abstract void Cast(Transform newPosition, float damageMultiplier);
    public abstract void HabilityReady();
    public async virtual void Cooldown(){
        await Task.Delay((int) coolDown*1000);
        HabilityReady();
    }
}
