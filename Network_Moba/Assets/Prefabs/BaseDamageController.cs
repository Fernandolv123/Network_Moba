using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BaseDamageController : NetworkBehaviour
{
    [SyncVar]
    Vector3 globalScale;
        [SyncVar]
    Vector3 globalPosition;
    private float damage;
    
    private void OnTriggerEnter(Collider other){
        Debug.Log("{OnTriggerEnter} {BaseDamageController}" + other.gameObject);
        if (other.gameObject.GetComponent<Player>()/*.IsEnemy() == true*/){
            other.gameObject.GetComponent<Player>().SimulatedOnTriggerEnter();
        }
    }
    //public void OnTriggerStay
    public void SetDamage(float newDamage){
        //Debug.Log("{SetDamage} Entra");
        damage = newDamage;
    }
    void Awake(){
        if(isLocalPlayer)GetComponent<NetworkIdentity>().RemoveClientAuthority();
        globalScale = gameObject.transform.localScale;
        globalPosition = gameObject.transform.position;
    }
    void Update(){
        if(gameObject.transform.localScale != globalScale)
        gameObject.transform.localScale = globalScale;

        gameObject.transform.position = globalPosition;
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player")){
            Debug.Log("{FindGameObjectsWithTag}");
            if(Vector3.Distance(gameObject.transform.position,go.transform.position) >= 4){
                Debug.Log("{FindGameObjectsWithTag} {DISTANCE}");
                go.GetComponent<Player>().SimulatedOnTriggerEnter();
            }
        }
    }
    public void ChangeScale(Vector3 newScale){
        //Debug.Log("{ChangeScale} Entra " +newScale);
        globalScale = newScale;
    }
    public void ChangePosition(Vector3 newPosition){
        //Debug.Log("{ChangePosition} Entra " +newPosition);
        globalPosition = newPosition;
    }
}
