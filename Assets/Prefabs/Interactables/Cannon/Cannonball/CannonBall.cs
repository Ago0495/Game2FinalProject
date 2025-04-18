using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class CannonBall : Entity
{
    public float despawnTime = 6f;
    public float despawnTimer = 0;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        despawnTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        despawnTimer += Time.deltaTime;
        if (despawnTimer > despawnTime) 
        {
            MyCore.NetDestroyObject(NetId);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            Entity entity = other.GetComponentInParent<Entity>();

            if (entity != null && other.gameObject.layer != this.gameObject.layer)
            {
                //damage other
                if (other.tag == "Marker")
                {
                    entity.takeDamage(attack * 2);
                    MyCore.NetDestroyObject(other.GetComponent<NetworkID>().NetId);
                }
                else
                {
                    entity.takeDamage(attack);
                }
                MyCore.NetDestroyObject(NetId);
            }
        }
    }
}
