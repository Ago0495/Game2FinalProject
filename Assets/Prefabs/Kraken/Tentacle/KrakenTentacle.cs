using UnityEngine;
using System.Collections;

public class KrakenTentacle : Enemy
{
    [SerializeField] private KrakenHead head;

    public float maxDistence;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "POS" && IsClient)
        {
            lastPosition = NetworkCore.Vector3FromString(value);
        }
        if (flag == "ROT" && IsClient)
        {
            lastRotation = NetworkCore.Vector3FromString(value);
        }
    }

    public void NetworkedStart()
    {
        base.NetworkedStart();
    }

    public override IEnumerator SlowUpdate()
    {
        while (true)
        {
            if (IsServer)
            {
                float distance = (this.transform.position - lastPosition).magnitude;
                if (distance > Threashhold)
                {
                    SendUpdate("POS", this.transform.position.ToString());
                    lastPosition = this.transform.position;
                }
                if ((this.transform.rotation.eulerAngles - lastRotation).magnitude > Threashhold)
                {
                    lastRotation = this.transform.rotation.eulerAngles;
                    SendUpdate("ROT", lastRotation.ToString());
                }

                if (IsDirty)
                {
                    SendUpdate("POS", lastPosition.ToString());
                    SendUpdate("ROT", lastRotation.ToString());
                    //animation

                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
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
