using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;

public class KrakenHead : Enemy
{
    [SerializeField] private int tentaclePrefab;
    [SerializeField] private GameObject krakenArea;
    [SerializeField] private float stopDistance;
    [SerializeField] private float bashDistance;
    public bool canSummon;
    public int maxTentacles;
    public int totalTentacles;
    public float summonDalaySeconds;
    public bool bashing;

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

    public IEnumerator spawnDelay()
    {
        yield return new WaitForSeconds(summonDalaySeconds);
        canSummon = true;
    }

    public IEnumerator bashDelay()
    {
        //play animation
        Debug.Log("Bash");
        yield return new WaitForSeconds(5.0f);
        bashing = false;
    }

    public void spawnTentacle()
    {
        if(canSummon && totalTentacles < maxTentacles)
        {
            canSummon = false;
            int randX = Random.Range(50, 160);
            int randY = Random.Range(-40, 40);
            Vector3 randPos = (target.transform.forward * randX) + (target.transform.right * randY);
            Vector3 direction = (transform.position - krakenArea.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(-direction);
            MyCore.NetCreateObject(tentaclePrefab, -1, randPos, targetRotation);
            totalTentacles++;
            StartCoroutine(spawnDelay());
        }
    }

    public void rotateTowardsPlayer()
    {
        Vector3 direction = (transform.position - target.transform.position).normalized; 
        Quaternion targetRotation = Quaternion.LookRotation(-direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void removeTentacle()
    {
        totalTentacles--;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        krakenArea = GameObject.FindGameObjectWithTag("KArea");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            float distence = (target.transform.position - transform.position).magnitude;
            if (distence <= bashDistance && !bashing)
            {
                bashing = true;
                StartCoroutine(bashDelay());
                //spawn tentacle
            }
            else if (attacking && distence <= stopDistance && !bashing)
            {
                rotateTowardsPlayer();
                if(MyRig.linearVelocity.magnitude > 0)
                {
                    MyRig.linearVelocity = Vector3.zero;
                }
                spawnTentacle();
                //spawn tent
            }
            else if (attacking && !bashing)
            {
                rotateTowardsPlayer();
                Vector3 moveDirection = (target.transform.position - transform.position).normalized;
                MyRig.linearVelocity = moveDirection * moveSpeed;
                spawnTentacle();
            }
            else if (!bashing)
            {
                if((transform.position - krakenArea.transform.position).magnitude > 50)
                {
                    Vector3 direction = (transform.position - krakenArea.transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    Vector3 moveDirection = (krakenArea.transform.position - transform.position).normalized;
                    MyRig.linearVelocity = moveDirection * moveSpeed;
                }
                else
                {
                    MyRig.linearVelocity = Vector3.zero;
                }
                    
            }

        }

        if (IsClient)
        {
            float distance = (this.transform.position - this.lastPosition).magnitude;
            if (distance > Ethreashhold)
            {
                this.transform.position = this.lastPosition;
            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, lastPosition, Time.deltaTime * moveSpeed);
            }

            this.transform.rotation = Quaternion.Euler(lastRotation);
        }
    }
}
