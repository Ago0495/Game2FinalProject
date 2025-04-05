using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonNetworkEntity : MonoBehaviour
{
    [SerializeField] protected Rigidbody MyRig;
    [SerializeField] protected Animator MyAnime;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int health;
    [SerializeField] protected int Defence;
    [SerializeField] protected Collider[] hitBoxes;
    [SerializeField] protected GameMaster gameMaster;
    protected bool isAlive;

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

        if (health <= 0)
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
