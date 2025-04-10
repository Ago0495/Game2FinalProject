using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class EnemyCannon : NetworkComponent
{
    public GameObject target;
    public float detectionRange = 20f;
    public float fireCooldown = 2f;
    public Transform firePoint;

    public float lastFireTime = 0f;

    public float reloadTime;
    public bool reloading;

    public GameObject cannonballPrefab;
    public Rigidbody MyRig;

    //protected int cannonballPrefab;

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            //GameObject[] players = GameObject.FindGameObjectsWithTag("SHIP");
            //look at player 
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist <= detectionRange && !reloading)
            {
                FireAtPlayer(target.transform);
                lastFireTime = Time.time;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
        
    }

    public IEnumerator reload()
    {
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("SHIP");
        MyRig = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //RotateTowards(transform.position - target.transform.position);
        Vector3 direction = (transform.position - target.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(-direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            20 * Time.deltaTime
        );
        if (!reloading)
        {
            GameObject tmp = (GameObject)Instantiate(cannonballPrefab, transform.position + transform.forward * 5, Quaternion.identity);
            reloading = true;
            StartCoroutine(reload());
        }
    }

    void FireAtPlayer(Transform target)
    {
        Vector3 direction = (target.position - firePoint.position).normalized;
        //GameObject projectile = MyCore.NetCreateObject(cannonballPrefab, -1, firePoint.position, Quaternion.LookRotation(direction));
        //Debug.Log("Cannon fired at: " + target.name);
        reloading = true;
    }
}
