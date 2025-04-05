using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Entity : NetworkComponent
{
    [SerializeField] protected Rigidbody MyRig;
    [SerializeField] protected Animator MyAnime;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int health;
    [SerializeField] protected int Defence;
    [SerializeField] protected Collider[] hitBoxes;
    [SerializeField] protected GameMaster gameMaster;
    protected bool isAlive;

    public Vector3 lastPosition;
    public Vector3 lastRotation;

    public float Threashhold;
    public float Ethreashhold;


    public override void HandleMessage(string flag, string value)
    {
        throw new System.NotImplementedException();
    }

    public override void NetworkedStart()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator SlowUpdate()
    {
        throw new System.NotImplementedException();
    }

    public int getHealth()
    {
        return health;
    }

    public void setHealth(int hp)
    {
        health = hp;
    }

    public virtual void takeDamage(int damage)
    {
        //TO-DO
        //Implement Defence
        health -= damage;

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
