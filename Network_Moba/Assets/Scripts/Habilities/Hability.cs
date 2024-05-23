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
    public abstract void Cast(Player taumaturgo);
    public abstract void HabilityReady();
    public async virtual void Cooldown(){
        await Task.Delay((int) coolDown*1000);
        HabilityReady();
    }
}
