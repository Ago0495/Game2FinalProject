using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class CannonBall : Entity
{
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        Entity entity = other.GetComponent<Entity>();

        if (other.gameObject.layer != this.gameObject.layer)
        {
            //damage other
            entity.takeDamage(25);
        }
    }
}
