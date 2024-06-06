using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Habilities/damage")]
public class Skillshot : Hability
{
    //Referencias
    //https://bronsonzgeb.com/index.php/2021/09/25/the-command-pattern-with-scriptable-objects/
    //https://www.youtube.com/watch?v=tdyP1vTrQfY
    
    //https://mirror-networking.gitbook.io/docs/manual/guides/authority
    public GameObject prefab;
    private GameObject goHability;
    private bool charging=false;
    public GameObject chargingBarPrefab;
    private Scrollbar chargingBar;
    private float scaleX;
    private float scaleY;
    private float scaleZ;

    [Server] public override void Cast(Transform playerPosition, float damageMultiplier){
        
        if (castBarPosition == null) castBarPosition = GameObject.FindWithTag("CastBarPosition").GetComponent<Transform>();
        chargingBar = Instantiate(chargingBarPrefab,castBarPosition.position,Quaternion.identity).GetComponent<Scrollbar>();
        chargingBar.transform.parent = castBarPosition.transform;
        chargingBar.value=0;
        scaleX = 0;
        scaleY = 0;
        scaleZ = 0;
        //goHability = GameObject.FindWithTag("Finish");
        goHability = Instantiate(prefab,playerPosition.position + Vector3.up*2,Quaternion.identity);
        goHability.transform.parent = playerPosition.transform;
        goHability.GetComponent<BaseDamageController>().SetDamage(damageMultiplier); 
        NetworkServer.Spawn(goHability);
        //NetworkIdentity identity =caster.gameObject.GetComponent<NetworkIdentity>();
        //Debug.Log("Encontrado: " + identity);
        //Debug.Log("ConectionToClient: " + identity.connectionToClient);
        charging=true;
        Charge();
    }
    [Command]
    public async void Charge(){
        float timer =0;
        while(charging && timer < 2.5){
            timer += Time.deltaTime * 16.6f;
            //Debug.Log(timer);
            if(Input.GetKey(KeyCode.Q)){
                Debug.Log("Charging Q hability");
                scaleX += 0.1f;
                scaleY += 0.1f;
                scaleZ += 0.1f;
                scaleX = Mathf.Clamp(scaleX,-3,3);
                scaleY = Mathf.Clamp(scaleY,-3,3);
                scaleZ = Mathf.Clamp(scaleZ,-3,3);
                //goHability.transform.localScale = Vector3.Lerp(goHability.transform.localScale,new Vector3(goHability.transform.localScale.x+3,goHability.transform.localScale.y+3,goHability.transform.localScale.z+3),timer/2.5f);
                goHability.transform.localScale = new Vector3(scaleX,scaleY,scaleZ);
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
        Debug.Log(chargingBar.value);
        charging=false;
        Destroy(chargingBar.gameObject,0.2f);
        Cooldown();
        //Destroy(goHability,5f);
    }
    [Command]
    public async void CastChargedHability(){
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
                //goHability.transform.position = hit.point;
                goHability.transform.parent = null;
                float arrivalTime=0;
                Vector3 goHabilityStartPosition = goHability.transform.position;
                while(arrivalTime < 1){
                    //Debug.Log("Entra " + Vector3.Lerp(goHability.transform.position,hit.point,0.5f));
                    arrivalTime +=0.05f/(scaleX <= 1 ? 1 : scaleX);
                    goHability.transform.position = Vector3.Lerp(goHabilityStartPosition,hit.point,arrivalTime);
                    //goHability.transform.position = (hit.point - goHability.transform.position) * arrivalTime + goHability.transform.position;
                    await Task.Delay(10);
                }
                Debug.Log("Sale");
                Destroy(goHability,2f);
        }
    }
    [Command]
    public override void HabilityReady(){
        Debug.Log("Q hability Ready");
        canCast=true;
    }
    [Command]
    public void RegisterHability(){
        NetworkClient.RegisterPrefab(prefab);
    }
}
