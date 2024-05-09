using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Habilities/damage")]
public class Skillshot : Hability
{
    public override void Cast(){
        Debug.Log("Casted Q hability");
    }
    public override void HabilityReady(){
        Debug.Log("Q hability Ready");
    }
}
