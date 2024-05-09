using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Habilities/Ultimates")]
public class Ultimate : Hability
{
    public override void Cast(Player taumaturgo)
    {
        Debug.Log("Casted R hability");
        Cooldown();
    }
    public override void HabilityReady(){
        Debug.Log("R hability Ready");
    }
}
