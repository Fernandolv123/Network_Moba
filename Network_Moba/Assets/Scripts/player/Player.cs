using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    private NetworkIdentity identity;
    protected virtual void Awake(){
        PrimaryDamagableHability.RegisterHability();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        identity  =this.gameObject.GetComponent<NetworkIdentity>();
        if(isLocalPlayer)identity.RemoveClientAuthority();
        // Debug.Log("Encontrado: " + identity);
        // Debug.Log("ConectionToClient: " + identity.connectionToClient);
        //if (isLocalPlayer) NetworkServer.AddPlayerForConnection(identity.connectionToClient,gameObject);
    }
    protected virtual void Start(){
        lastFrameSpeed = movementSpeed;
        lastframeHealth= health;
    }
    protected virtual void Update()
    {
        if (!isLocalPlayer) return;
        // Debug.Log("Tiene autoridad: " + identity.isOwned);
        // Debug.Log("habilidades registradas: " + NetworkClient.prefabs.Count);
        // Debug.Log("Numero de conexiones: " + NetworkServer.connections.Count);
        //         NetworkIdentity identity =this.gameObject.GetComponent<NetworkIdentity>();
        // Debug.Log("Encontrado: " + identity);
        // Debug.Log("ConectionToClient: " + identity.connectionToClient + " connection to server: " + connectionToClient);
        // Debug.Log("NetworkClient: " + NetworkClient.connection);
        // Debug.Log("NetworkServer: "+NetworkServer.active);
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
            Cast(transform,baseDamage);
            //CastOnServerRPC(PrimaryDamagableHability);
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
            habilityToCast.Cast(transform,baseDamage);
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
        Debug.Log("{OnStartLocalPlayer} he entrado");
    }






    
    [Header("Prueba")]
    public GameObject prefab;
    private GameObject goHability;
    private bool charging=false;
    public GameObject chargingBarPrefab;
    private Scrollbar chargingBar;
    private float scaleX;
    private float scaleY;
    private float scaleZ;
    public float range;
    public float damage;
    public float wide;
    public float coolDown;
    public bool canCast = true;
    protected Transform castBarPosition;
    
    
    
    
    
    
    
    [Command]
    public async void Cooldown(){
        await Task.Delay((int) coolDown*1000);
        HabilityReady();
    }
    [Command]
    public void HabilityReady(){
        //Debug.Log("Q hability Ready");
        canCast=true;
    }





    [Command]
    public void SpawnNetworkGO(GameObject gameObjectHability, Transform playerPosition, float damageMultiplier){
        
        //Debug.Log("{SpawnNetworkGO} " + goHability);
                goHability = Instantiate(prefab,playerPosition.position + Vector3.up*2,Quaternion.identity);
        //goHability.transform.parent = playerPosition.transform;
        goHability.GetComponent<BaseDamageController>().SetDamage(damageMultiplier);
        NetworkServer.Spawn(goHability,identity.connectionToClient);
    }
    public void Cast(Transform playerPosition, float damageMultiplier){
        
        if (castBarPosition == null) castBarPosition = GameObject.FindWithTag("CastBarPosition").GetComponent<Transform>();
        chargingBar = Instantiate(chargingBarPrefab,castBarPosition.position,Quaternion.identity).GetComponent<Scrollbar>();
        chargingBar.transform.parent = castBarPosition.transform;
        chargingBar.value=0;
        scaleX = 0;
        scaleY = 0;
        scaleZ = 0;
        //goHability = GameObject.FindWithTag("Finish");
        // goHability = Instantiate(prefab,playerPosition.position + Vector3.up*2,Quaternion.identity);
        // goHability.transform.parent = playerPosition.transform;
        // goHability.GetComponent<BaseDamageController>().SetDamage(damageMultiplier);
        // if (isServer){
        //     NetworkServer.Spawn(goHability,identity.connectionToClient);
        // } else if (isClientOnly){
            //Debug.Log("{CastBeforeSpawn} " + goHability);
            SpawnNetworkGO(goHability, playerPosition, damageMultiplier); 
        // }
        //NetworkIdentity identity =caster.gameObject.GetComponent<NetworkIdentity>();
        //Debug.Log("Encontrado: " + identity);
        //Debug.Log("ConectionToClient: " + identity.connectionToClient);
        charging=true;
        Charge();
    }

    [Command]
    public void UpdateChargeLevel(Vector3 newScale){
        // Debug.Log("{UpdateChargeLevel} " + newScale);
        // Debug.Log("{Antes} " + goHability.transform.localScale);
        //Debug.Log("{UpdateChargeLevel}" + goHability);
        //Debug.Log("{UpdateChargeLevel}" + goHability.GetComponent<BaseDamageController>());
        goHability.GetComponent<BaseDamageController>().ChangeScale(newScale);
        // Debug.Log("{Despues} " + goHability.transform.localScale);
    }
    public async void Charge(){
        float timer =0;
        while(charging && timer < 2.5){
            timer += Time.deltaTime * 16.6f;
            //Debug.Log(timer);
            if(Input.GetKey(KeyCode.Q)){
                //Debug.Log("Charging Q hability");
                scaleX += 0.1f;
                scaleY += 0.1f;
                scaleZ += 0.1f;
                scaleX = Mathf.Clamp(scaleX,-3,3);
                scaleY = Mathf.Clamp(scaleY,-3,3);
                scaleZ = Mathf.Clamp(scaleZ,-3,3);
                //goHability.transform.localScale = Vector3.Lerp(goHability.transform.localScale,new Vector3(goHability.transform.localScale.x+3,goHability.transform.localScale.y+3,goHability.transform.localScale.z+3),timer/2.5f);
                //goHability.transform.localScale = new Vector3(scaleX,scaleY,scaleZ);
                UpdateChargeLevel(new Vector3(scaleX,scaleY,scaleZ));
            }
            if(!Input.GetKey(KeyCode.Q)){
                Debug.Log("Casted Q hability");
                charging=false;
                CastChargedHability();
                break;
            }
            chargingBar.value = timer/2.5f;
            await Task.Delay(100);
        }
        if (chargingBar.value >=1){
            Destroy(goHability,1f);
        }
        //Debug.Log(chargingBar.value);
        charging=false;
        Destroy(chargingBar.gameObject,0.2f);
        Cooldown();
        Destroy(goHability,5f);
    }
    public void CastChargedHability(){
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
            //Debug.Log("{CastChargedHability} hit: " + hit.point);
            CastHabilityOnServer(hit.point);
                //goHability.transform.position = hit.point;
                //goHability.transform.parent = null;
                //float arrivalTime=0;
                //Vector3 goHabilityStartPosition = GetPositionOnServer();
                // while(arrivalTime < 1){
                //     //Debug.Log("Entra " + Vector3.Lerp(goHability.transform.position,hit.point,0.5f));
                //     arrivalTime +=0.05f/(scaleX <= 1 ? 1 : scaleX);
                //     //goHability.transform.position = Vector3.Lerp(goHabilityStartPosition,hit.point,arrivalTime);
                //     //goHability.transform.position = (hit.point - goHability.transform.position) * arrivalTime + goHability.transform.position;
                //     await Task.Delay(10);
                // }
                //Debug.Log("{CastChargedHability} Sale");
                // Destroy(goHability,2f);
        }
    }

    [Command]
    public async void CastHabilityOnServer(Vector3 point){
        //Debug.Log("{CastHabilityOnServer} hit: " + point);
        float arrivalTime=0;
        Vector3 goHabilityStartPosition = goHability.transform.position;
        while(arrivalTime < 1){
            //Debug.Log("{CastHabilityOnServer} Entra");
            arrivalTime +=0.05f/(scaleX <= 1 ? 1 : scaleX);
            goHability.GetComponent<BaseDamageController>().ChangePosition(Vector3.Lerp(goHabilityStartPosition,point,arrivalTime));
            goHability.transform.position = Vector3.Lerp(goHabilityStartPosition,point,arrivalTime);
            //goHability.transform.position = (hit.point - goHability.transform.position) * arrivalTime + goHability.transform.position;
            await Task.Delay(10);
        }
        Destroy(goHability,2f);
    }
    [Command]
    public void GetPositionOnServer(){
        //goHability.transform.position;
    }
    private void OnTriggerEnter(Collider other){
        Debug.Log("{OnTriggerEnter} {Player}" + other.gameObject);
    }
}
