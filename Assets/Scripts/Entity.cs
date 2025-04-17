using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Entity : NetworkComponent
{
    [SerializeField] protected Rigidbody MyRig;
    [SerializeField] protected Animator MyAnime;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] public float defense;
    [SerializeField] protected Collider[] hitBoxes;
    [SerializeField] protected GameMaster gameMaster;
    protected bool isAlive;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "HP")
        {
            if (IsClient)
            {
                health = int.Parse(value);
            }
        }
    }

    public override void NetworkedStart()
    {
        if (IsServer)
        {
            SendUpdate("HP", health.ToString());
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    public float getHealth()
    {
        return health;
    }
    public float getMaxHealth()
    {
        return maxHealth;
    }

    public void setHealth(int hp)
    {
        health = hp;
    }

    public virtual void takeDamage(float damage)
    {
        //TO-DO
        //Implement Defense
        
        float totalDamage = damage * (1-this.defense);
        health -= totalDamage;
        SendUpdate("HP", health.ToString());

        if(health <= 0)
        {
            isAlive = false;
            OnDeath();
        }

        OnDamageTaken(totalDamage);
    }

    public virtual void OnDamageTaken(float damageTaken)
    {

    }

    public virtual void OnDeath()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        MyRig = this.GetComponent<Rigidbody>();
        //MyAnime = this.GetComponent<Animator>();
        gameMaster = GameObject.FindAnyObjectByType<GameMaster>();
        health = maxHealth;
    }
}
