using UnityEngine;
using System.Collections;

public class KrakenTentacle : Enemy
{
    [SerializeField] private KrakenHead head;

    public float maxDistence;

    public override void HandleMessage(string flag, string value)
    {
    }

    public void NetworkedStart()
    {
        base.NetworkedStart();
    }

    public override IEnumerator SlowUpdate()
    {
        
            yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindAnyObjectByType<ShipMovement>();
        head = GameObject.FindFirstObjectByType<KrakenHead>();
    }

    public IEnumerator attack()
    {
        //animation
        Debug.Log("Tentacle Attack");
        yield return new WaitForSeconds(2.0f);
        head.removeTentacle();
        MyCore.NetDestroyObject(this.NetId);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (attacking)
            {
                StartCoroutine(attack());
            }
            else
            {
                Vector3 direction = (transform.position - target.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(-direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            if((transform.position - target.transform.position).magnitude > maxDistence)
            {
                head.removeTentacle();
                Debug.Log("Tentacle to far");
                MyCore.NetDestroyObject(this.NetId);
            }
        }
    }
}
