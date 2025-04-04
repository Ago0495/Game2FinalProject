using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected ShipMovement target;
    [SerializeField] protected GameObject dropOnDeath;
    [SerializeField] protected float detectRange;
    [SerializeField] protected Collider DetectRange;
    [SerializeField] protected int scoreValue;
    public bool attacking;
    public bool transition;

    public override void NetworkedStart()
    {
        target = GameObject.FindAnyObjectByType<ShipMovement>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(IsServer && other.gameObject == target)
        {
            attacking = true;
            transition = true;
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if(IsServer && other.gameObject == target)
        {
            attacking = false;
            transition = true;
        }
    }

    public override void takeDamage(int damage)
    {
        base.takeDamage(damage);
        //spawn dropOnDeath object
        //add score
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        base.Start();
    }
}
