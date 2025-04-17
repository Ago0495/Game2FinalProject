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

    //Animations
    private bool bash;
    private bool move = false;
    private bool idle = false;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "Bash" && IsClient)
        {
            MyAnime.SetBool("Bash", true);
        }
        if (flag == "Move" && IsClient)
        {
            MyAnime.SetBool("Move", true);
        }
        if (flag == "Idle" && IsClient)
        {
            MyAnime.SetBool("Bash", false);
            MyAnime.SetBool("Move", false);
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
                    if (move)
                    {
                        SendUpdate("Move", "404");
                    }
                    if (bash)
                    {
                        SendUpdate("Bash", "404");
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

    public IEnumerator spawnDelay()
    {
        yield return new WaitForSeconds(summonDalaySeconds);
        canSummon = true;
    }

    public IEnumerator bashDelay()
    {
        //play animation
        idle = false;
        bash = true;
        MyAnime.SetBool("Bash", true);
        SendUpdate("Bash", "404");
        yield return new WaitForSeconds(2.0f);
        SendUpdate("Idle", "404");
        MyAnime.SetBool("Bash", false);
        idle = true;
        bash = false;
        yield return new WaitForSeconds(4.0f);
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
        MyAnime = GetComponent<Animator>();
        target = GameObject.FindAnyObjectByType<ShipMovement>();
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
                spawnTentacle();
            }
            else if (attacking && distence <= stopDistance && !bashing)
            {
                rotateTowardsPlayer();
                if(MyRig.linearVelocity.magnitude > 0)
                {
                    MyRig.linearVelocity = Vector3.zero;
                    MyAnime.SetBool("Move", false);
                    move = false;
                    idle = true;
                    SendUpdate("Idle", "404");
                }
                spawnTentacle();
            }
            else if (attacking && !bashing)
            {
                rotateTowardsPlayer();
                Vector3 moveDirection = (target.transform.position - transform.position).normalized;
                MyRig.linearVelocity = moveDirection * moveSpeed;
                spawnTentacle();
                if (!move)
                {
                    SendUpdate("Move", "404");
                    move = true;
                    idle = false;
                    MyAnime.SetBool("Move", true);
                }
            }
            else if (!bashing)
            {
                if((transform.position - krakenArea.transform.position).magnitude > 50)
                {
                    Vector3 direction = (transform.position - krakenArea.transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(-direction);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    Vector3 moveDirection = (krakenArea.transform.position - transform.position).normalized;
                    MyRig.linearVelocity = moveDirection * moveSpeed;
                    if (!move)
                    {
                        Debug.Log("Here");
                        SendUpdate("Move", "404");
                        move = true;
                        idle = false;
                        MyAnime.SetBool("Move", true);
                    }
                }
                else
                {
                    if (MyRig.linearVelocity.magnitude > 0)
                    {
                        MyRig.linearVelocity = Vector3.zero;
                        MyAnime.SetBool("Move", false);
                        move = false;
                        idle = true;
                        SendUpdate("Idle", "404");
                    }
                }
                    
            }

        }
    }
}
