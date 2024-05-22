using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDamageController : MonoBehaviour
{
    private float damage;
    public void OnTriggerEnter(Collider other){
        if (other.gameObject.GetComponent<Player>().IsEnemy() == true){
            other.gameObject.GetComponent<Player>().SimulatedOnTriggerEnter();
        }
    }
    //public void OnTriggerStay
    public void SetDamage(float newDamage){
        damage = newDamage;
    }
}
