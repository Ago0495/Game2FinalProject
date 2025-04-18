using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class KrakenTentacle : Enemy
{
    [SerializeField] private KrakenHead head;

    public float maxDistence;
    public bool slamming;
    public bool waiting;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "Idle")
        {
            MyAnime.SetBool("Slam", false);
        }
        if(flag == "Slam")
        {
            MyAnime.SetBool("Slam", true);
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
                if (IsDirty)
                {
                    if (slamming)
                    {
                        SendUpdate("Slam", "404");
                    }
                    else
                    {
                        SendUpdate("Idle", "404");
                    }
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
        MyAnime = GetComponent<Animator>();
    }

    public IEnumerator attack()
    {
        //animation
        MyAnime.SetBool("Slam", true);
        SendUpdate("Slam", "404");
        //Debug.Log("Tentacle Attack");
        yield return new WaitForSeconds(3.3f);
        MyAnime.SetBool("Slam", false);
        SendUpdate("Idle", "404");
        slamming = false;
        yield return new WaitForSeconds(3.3f);
        waiting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (attacking && !slamming && !waiting)
            {
                slamming = true;
                waiting = true;
                StartCoroutine(attack());
            }
            else if (!slamming)
            {
                Vector3 direction = (transform.position - target.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(-direction) * Quaternion.Euler(0, 90f, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if((transform.position - target.transform.position).magnitude > maxDistence)
            {
                head.removeTentacle();
                //Debug.Log("Tentacle to far");
                MyCore.NetDestroyObject(this.NetId);
            }
        }
    }
}
