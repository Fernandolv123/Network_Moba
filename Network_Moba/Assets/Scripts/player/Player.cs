using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Player : MonoBehaviour
{
    [Header("Everything needed for base Characters")]
    public Skillshot PrimaryDamagableHability;
    public Hability PrimaryMovilityHability;
    public Hability PrimaryUltimateHability;
    public float health;
    private float lastframeHealth;
    public float armor;
    public float baseDamage;
    public float movementSpeed;
    private float lastFrameSpeed;
    public abstract void Cast();
    public abstract void Die(float health);
    public abstract void UpdateHealthOnServerRPC(float health);

    public delegate void OnHealthChanged(float health);
    public OnHealthChanged onHealthChanged;

    public delegate void OnMovementSpeedChanged(float speed);
    public OnMovementSpeedChanged onMovementSpeedChanged;

    private NavMeshAgent agent;
    protected virtual void Awake(){
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
    }
    protected virtual void Start(){
        lastFrameSpeed = movementSpeed;
        lastframeHealth= health;
    }
    protected virtual void Update()
    {
        if(lastFrameSpeed != movementSpeed){
            onMovementSpeedChanged += UpdateSpeed;
            if(onMovementSpeedChanged != null) onMovementSpeedChanged(movementSpeed);
            onMovementSpeedChanged = null;
        }
        lastFrameSpeed = movementSpeed;
        if(lastframeHealth != health){
            if(onHealthChanged != null) onHealthChanged(health);
            onHealthChanged=null;
        }
        lastframeHealth=health;
        Move();
        if (Input.GetKeyDown(KeyCode.Q)){
            CastOnServerRPC(PrimaryDamagableHability);
        }
        if (Input.GetKeyDown(KeyCode.W)){
            SimulatedOnTriggerEnter();
        }
        if (Input.GetKeyDown(KeyCode.E)){
            CastOnServerRPC(PrimaryMovilityHability);
        }
        if (Input.GetKeyDown(KeyCode.R)){
            CastOnServerRPC(PrimaryUltimateHability);
        }
    }
    public void Move(){
        if(Input.GetMouseButtonDown(1)){
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
                agent.destination = hit.point;
            }
        }
    }
    private void OnTriggerEnter(Collider other){
        float healthnow=-2;
        onHealthChanged += UpdateHealthOnServerRPC;
        if(healthnow < 0){
            onHealthChanged += Die;
        }
    }

    private void SimulatedOnTriggerEnter(){
        health -= 5;
        onHealthChanged += UpdateHealthOnServerRPC;
        if(health <= 0){
            onHealthChanged += Die;
        }
    }
    private void UpdateSpeed(float speed){
        agent.speed = speed;
    }

    private void CastOnServerRPC(Hability habilityToCast){
        if(habilityToCast.canCast){
            habilityToCast.Cast(this);
            habilityToCast.canCast=false;
            //Buscar una buena forma de hacer el cooldown
            //Invoke("PrimaryDamagableHability.coolDown",PrimaryDamagableHability.coolDown);
            //Invoke("CD",habilityToCast.coolDown);
            //StartCoroutine(CD(habilityToCast.coolDown,habilityToCast));
        }
    }
    public IEnumerator CD(float coolDown,Hability skill){
        //Eventualmente hay que cambiar la corrutina por un timer
        yield return new WaitForSeconds(coolDown);
        skill.HabilityReady();
        skill.canCast=true;
    }

}
