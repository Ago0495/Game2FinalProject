using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class EnemyCannon : InteractableCannon
{
    public GameObject target;
    public float detectionRange = 20f;
    public float fireCooldown = 2f;
    public Transform firePoint;

    public float lastFireTime = 0f;

    public float reloadTime;
    public bool reloading;

    public void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
    }

    public override void NetworkedStart()
    {
        target = GameObject.FindGameObjectWithTag("SHIP");
    }

    public IEnumerator reload()
    {
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }

    public override IEnumerator SlowUpdate()
    {
        /*while (IsServer)
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
        }*/
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FireAtPlayer(Transform target)
    {
        Vector3 direction = (target.position - firePoint.position).normalized;
        GameObject projectile = MyCore.NetCreateObject(cannonballPrefab, -1, firePoint.position, Quaternion.LookRotation(direction));
        //Debug.Log("Cannon fired at: " + target.name);
        reloading = true;
    }

}
