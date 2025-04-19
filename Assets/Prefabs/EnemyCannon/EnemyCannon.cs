using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class EnemyCannon : NetworkComponent
{
    public MusicMasterScript musicMaster;
    public GameObject target;
    public float atk;
    public float detectionRange = 20f;
    public int cannonBallSpeed;
    public float gravity;

    public float reloadTime;
    public bool reloading;
    public Vector3 aimOffset;

    public Rigidbody MyRig;

    public int cannonballPrefab;
    [SerializeField] GameObject cannonAudioSource;

    public bool attacking;

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "Battle")
        {
            musicMaster.fortAttack();
            musicMaster.enemyCannons++;
        }
        if (flag == "ENDBATTLE")
        {
            musicMaster.enemyCannons--;
            if(musicMaster.enemyCannons <= 0)
            {
                musicMaster.enemyCannons = 0;
            }
            musicMaster.background();
        }
        if (flag == "FIRESOUND")
        {
            if (IsClient)
            {
                cannonAudioSource.GetComponent<AudioSource>().Play();
            }
        }
    }

    public override void NetworkedStart()
    {
        target = GameObject.FindGameObjectWithTag("SHIP");
        musicMaster = FindAnyObjectByType<MusicMasterScript>();
    }

    public IEnumerator reload()
    {
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MyRig = this.GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("SHIP");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            Vector3 toTarget = target.transform.position - transform.position + aimOffset;

            Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

            float grav = Mathf.Abs(Physics.gravity.y);
            float v2 = cannonBallSpeed * cannonBallSpeed;
            float v4 = v2 * v2;
            float gx = grav * toTargetXZ.magnitude;

            float discriminant = v4 - grav * (grav * toTargetXZ.magnitude * toTargetXZ.magnitude + 2 * toTarget.y * v2);

            Quaternion targetRotation = Quaternion.identity;

            float sqrtDisc = Mathf.Sqrt(discriminant);
            float angle = Mathf.Atan((v2 - sqrtDisc) / gx);

            Vector3 flatDir = toTargetXZ.normalized;
            Quaternion yawRotation = Quaternion.LookRotation(flatDir);
            if(Mathf.Abs(angle) > 0)
            {
                targetRotation = yawRotation * Quaternion.Euler(-Mathf.Rad2Deg * angle, 0, 0);

                transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                20 * Time.deltaTime);

                if (!reloading)
                {
                    FireAtPlayer();
                    StartCoroutine(reload());
                }
                if (!attacking)
                {
                    attacking = true;
                    SendUpdate("Battle", "hope");
                }
            }
            else
            {
                if (attacking)
                {
                    attacking = false;
                    SendUpdate("ENDBATTLE", "hope");
                }
            }
        }
    }

    void FireAtPlayer()
    {
        GameObject tempBall = MyCore.NetCreateObject(cannonballPrefab, -1, transform.position + transform.forward * 5, Quaternion.identity);
        Rigidbody tempRB = tempBall.GetComponent<Rigidbody>();
        tempRB.gameObject.layer = gameObject.layer;
        if (tempRB != null)
        {
            tempBall.GetComponent<CannonBall>().attack = atk;
            tempRB.linearVelocity = transform.forward * cannonBallSpeed;
        }
        SendUpdate("FIRESOUND", "1001");
        reloading = true;
    }
}
