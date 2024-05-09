using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Habilities/damage")]
public class Skillshot : Hability
{
    public GameObject prefab;
    private GameObject goHability;
    private bool charging=false;
    private float scaleX;
    private float scaleY;
    private float scaleZ;
    public override void Cast(Player taumaturgo){
        scaleX = 0;
        scaleY = 0;
        scaleZ = 0;
        goHability = Instantiate(prefab,taumaturgo.transform.position + Vector3.up*2,Quaternion.identity);
        goHability.transform.parent = taumaturgo.transform;
        charging=true;
        Charge();
    }
    public async void Charge(){
        float timer =0;
        while(charging){
            timer += Time.deltaTime *16.6f;
            //Debug.Log(timer);
            if(Input.GetKey(KeyCode.Q)){
                Debug.Log("Charging Q hability");
                scaleX += 0.1f;
                scaleY += 0.1f;
                scaleZ += 0.1f;
                scaleX = Mathf.Clamp(scaleX,-3,3);
                scaleY = Mathf.Clamp(scaleY,-3,3);
                scaleZ = Mathf.Clamp(scaleZ,-3,3);
                goHability.transform.localScale = new Vector3(scaleX,scaleY,scaleZ);
            }
            if(!Input.GetKey(KeyCode.Q)){
                Debug.Log("Casted Q hability");
                charging=false;
                Destroy(goHability,2f);
                Cooldown();
                CastChargedHability();
                return;
            }
            await Task.Delay(100);
        }
        charging=false;
        Destroy(goHability,2f);
    }
    public void CastChargedHability(){
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)){
                goHability.transform.position = hit.point;
                goHability.transform.parent = null;
        }
    }
    public override void HabilityReady(){
        Debug.Log("Q hability Ready");
        canCast=true;
        
    }
}
