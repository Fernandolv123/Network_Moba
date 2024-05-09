using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Habilities/Movility/Blink")]
public class Blink : Hability
{
    public override void Cast(Player taumaturgo)
    {
        Vector3 desiredPosition;
        RaycastHit hit;
        Debug.Log(range);
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
            canCast = false;
            desiredPosition = new Vector3(Mathf.Clamp(hit.point.x,taumaturgo.transform.position.x-range,taumaturgo.transform.position.x+range),taumaturgo.transform.position.y,Mathf.Clamp(hit.point.z,taumaturgo.transform.position.z-range,taumaturgo.transform.position.z+range));
            taumaturgo.transform.position = desiredPosition;
            Debug.Log("Casted E hability");
            taumaturgo.GetComponent<NavMeshAgent>().destination = hit.point;
            Cooldown();
        }
    }
    public override void HabilityReady(){
        Debug.Log("E hability Ready");
        canCast = true;
    }
}
