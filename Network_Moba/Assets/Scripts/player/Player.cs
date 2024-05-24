using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public abstract class Player : NetworkBehaviour
{
    private bool isPlayer;
    private bool isAlly;
    private bool isEnemy;
    [Header("Everything needed for base Characters")]
    public Skillshot PrimaryDamagableHability;
    public Hability PrimaryMovilityHability;
    public Hability PrimaryUltimateHability;
    [SyncVar]
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
        NetworkIdentity identity =this.gameObject.GetComponent<NetworkIdentity>();
        Debug.Log("Encontrado: " + identity);
        Debug.Log("ConectionToClient: " + identity.connectionToClient);
    }
    protected virtual void Start(){
        lastFrameSpeed = movementSpeed;
        lastframeHealth= health;
    }
    protected virtual void Update()
    {
        if (!isLocalPlayer) return;
                NetworkIdentity identity =this.gameObject.GetComponent<NetworkIdentity>();
        Debug.Log("Encontrado: " + identity);
        Debug.Log("ConectionToClient: " + identity.connectionToClient + " connection to server: " + connectionToClient);
        Debug.Log("NetworkClient: " + NetworkClient.connection);
        Debug.Log("NetworkServer: "+NetworkServer.active);
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

    [Command]
    public void SimulatedOnTriggerEnter(){
        health -= 5;
        onHealthChanged += UpdateHealthOnServerRPC;
            //UpdateHealthOnServerRPC(health);
        if(health <= 0){
            onHealthChanged += Die;
                //Die(health);
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
    public bool IsAlly(){
        return isAlly;
    }
    public bool IsEnemy(){
        return isEnemy;
    }
    public bool IsPlayer(){
        return isPlayer;
    }

    public override void OnStartLocalPlayer(){
        Debug.Log("he entrado");
    }
}
