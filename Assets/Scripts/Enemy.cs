using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject dropOnDeath;
    [SerializeField] private float detectRange;
    [SerializeField] private Collider DetectRange;
    [SerializeField] private int scoreValue;
    private bool attacking;

    private void findTarget()
    {
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
