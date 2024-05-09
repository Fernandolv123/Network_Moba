using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Player
{
    [Header("Reffered to Mage Passives")]
    public float magePasive; //++damage --health
    public override void Cast()
    {
        throw new System.NotImplementedException();
    }
    public override void Move()
    {
        throw new System.NotImplementedException();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        health -= magePasive;
        baseDamage += magePasive;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Die(float health)
    {
        Debug.Log("Has muerto");
    }
    public override void UpdateHealthOnServerRPC(float health){
        Debug.Log("Vida Cambiada");
    }
}
