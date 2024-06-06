using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Habilities/Movility")]
public class Dash : Hability
{
    public override void Cast(Transform newPosition, float damageMultiplier)
    {
        Debug.Log("Casted E hability");
        Cooldown();
    }
    public override void HabilityReady(){
        Debug.Log("E hability Ready");
    }
}
