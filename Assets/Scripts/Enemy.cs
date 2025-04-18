using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected ShipMovement target;
    [SerializeField] protected GameObject dropOnDeath;
    [SerializeField] protected Collider DetectRange;
    [SerializeField] protected int scoreValue;
    [SerializeField] protected MusicMasterScript musicMaster;
    public bool attacking;
    public bool transition;

    public override void NetworkedStart()
    {
        target = GameObject.FindAnyObjectByType<ShipMovement>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);
        if(other.gameObject.tag == "SHIP")
        {
            attacking = true;
            transition = true;
            Entity temp = other.GetComponent<Entity>();

            if (temp != null)
            {
                temp.takeDamage(attack);
            }

            if (IsServer)
            {
                SendUpdate("Battle", "hope");
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "SHIP")
        {
            attacking = false;
            transition = true;
            SendUpdate("ENDBATTLE", "hope");
        }
    }

    public override void takeDamage(float damage)
    {
        base.takeDamage(damage);
    }

    public override void OnDamageTaken(float damageTaken)
    {
        //spawn dropOnDeath object
        //add score
    }

    public override void OnDeath()
    {
        SendUpdate("ENDBATTLE", "hope");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        base.Start();
        musicMaster = FindAnyObjectByType<MusicMasterScript>();
    }
}
