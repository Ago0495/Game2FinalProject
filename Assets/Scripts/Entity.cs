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
    [SerializeField] protected float defence;
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
                Debug.Log("Ship Health: " + health);
            }
        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    public float getHealth()
    {
        return health;
    }

    public void setHealth(int hp)
    {
        health = hp;
    }

    public virtual void takeDamage(float damage)
    {
        //TO-DO
        //Implement Defence
        health -= damage/* * (1-defence)*/;
        SendUpdate("HP", health.ToString());

        if(health <= 0)
        {
            isAlive = false;
            //play death animation
        }


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        MyRig = this.GetComponent<Rigidbody>();
        //MyAnime = this.GetComponent<Animator>();
        gameMaster = GameObject.FindAnyObjectByType<GameMaster>();
    }
}
