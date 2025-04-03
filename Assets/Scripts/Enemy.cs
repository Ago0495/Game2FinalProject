using UnityEngine;
using UnityEngine.AI;

public class Enemy : NonNetworkEntity //reset
{
    //[SerializeField] private ShipMovement target;
    [SerializeField] protected GameObject target; //reset
    [SerializeField] protected GameObject dropOnDeath;
    [SerializeField] protected float detectRange;
    [SerializeField] protected Collider DetectRange;
    [SerializeField] protected int scoreValue;
    //[SerializeField] protected NavMeshAgent myAgent;
    //[SerializeField] protected Vector3 markPosition;

    public bool attacking;

    private void findTarget()
    {
        //myAgent = GetComponent<NavMeshAgent>();
        //target = GameObject.FindAnyObjectByType<ShipMovement>();
        target = GameObject.FindGameObjectWithTag("SHIP");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == target)
        {
            attacking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == target)
        {
            attacking = false;
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
        findTarget();
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }
}
