using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Habilities/Ultimates")]
public class Ultimate : Hability
{
    public override void Cast()
    {
        Debug.Log("Casted R hability");
    }
    public override void HabilityReady(){
        Debug.Log("R hability Ready");
    }
}
