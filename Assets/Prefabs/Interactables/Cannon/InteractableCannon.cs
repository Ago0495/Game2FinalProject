using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
using System.Collections;

public class InteractableCannon : Interactable
{
    [SerializeField] float cannonMoveSpeed = 10;
    [SerializeField] int cannonballPrefab;
    [SerializeField] float ballForce = 50;
    [SerializeField] public float atk = 10;
    [SerializeField] GameObject cannonAudioSource;
    Vector2 lastInput;
    Rigidbody rb;
    private float pitch = 0f;
    private float yaw = 0f;
    private float startYaw = 0f;
    private float startPitch = 0f;
    private bool valuesSet = false;
    public bool canFire = true;
    public bool fireAttempt = false;
    public float reloadTime = 2;
    public float reloadTimer = 0;

    //camera
    protected GameObject cameraObj;
    [SerializeField] private Transform cameraHolderPos;

    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);

        if (flag == "MOVE")
        {
            if (IsServer) 
            {
                lastInput = NetworkCore.Vector2FromString(value);
            }
        }
        if (flag == "FIRE")
        {
            if (IsServer && canFire)
            {
                ShootCannonBall();
            }
            else if (IsServer && MyCore.NetObjs[user].GetComponent<PlayerStats>().skill == 0 && reloadTimer > reloadTime * 0.40 && reloadTimer < reloadTime * 0.60 && !fireAttempt)
            {
                //Debug.Log(reloadTimer);
                ShootCannonBall();
                reloadTimer = 0;
                SendUpdate("SETTIMER", reloadTimer.ToString());
            }
            else
            {
                fireAttempt = true;
            }
        }
        if (flag == "CANFIRE")
        {
            if (IsServer)
            {
                canFire = bool.Parse(value);
                SendUpdate("CANFIRE", canFire.ToString());
            }
            if (IsClient)
            {
                canFire = bool.Parse(value);
            }
        }
        if (flag == "SETTIMER")
        {
            if (IsClient)
            {
                reloadTimer = float.Parse(value);
                //Debug.Log("SETTIMER: " + reloadTimer);
            }
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
        base.NetworkedStart();

        //shipRB = GameObject.FindGameObjectWithTag("SHIP").GetComponent<Rigidbody>();
        if (user >= 0 && IsLocalPlayer)
        {
            MyCore.NetObjs[user].GetComponent<NetworkPlayerController>().overrideCamera = true;
            cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    public void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();

        if (!valuesSet)
        {
            yaw = transform.rotation.eulerAngles.y;
            pitch = transform.rotation.eulerAngles.x;
            startYaw = yaw;
            startPitch = pitch;
        }

        transform.SetParent(GameObject.FindGameObjectWithTag("SHIP").transform);
    }

    public void Update()
    {
        base.Update();

        if (!canFire)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer > reloadTime)
            {
                canFire = true;
                fireAttempt = false;

                reloadTimer -= reloadTime;
            }
            if (IsServer)
            {
                SendUpdate("CANFIRE", canFire.ToString());
            }
        }

        if (IsLocalPlayer && cameraHolderPos != null && cameraObj != null)
        {
            if (!gm.gameFinished)
            {
                cameraObj.transform.position = cameraHolderPos.transform.position;
                cameraObj.transform.rotation = cameraHolderPos.transform.rotation;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        //if (IsLocalPlayer)
        //{
        //    Debug.Log(reloadTimer);
        //}
    }

    public void FixedUpdate()
    {
        // pitch and yaw missing up rotation when creating new object
        if (IsServer && Owner >= 0)
        {
            yaw += lastInput.x * cannonMoveSpeed * Time.deltaTime;
            pitch -= lastInput.y * cannonMoveSpeed * Time.deltaTime;

            //yaw = Mathf.Clamp(yaw, startYaw + -25, startYaw + 25);
            //pitch = Mathf.Clamp(pitch, startPitch - 15, startPitch + 10);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        }
    }

    public override void SetValues(int oldInteractable)
    {
        InteractableCannon oldCannon = MyCore.NetObjs[oldInteractable].GetComponent<InteractableCannon>();

        atk = oldCannon.atk;
        yaw = oldCannon.yaw;
        pitch = oldCannon.pitch;
        startYaw = oldCannon.startYaw;
        startPitch = oldCannon.startPitch;

        valuesSet = false;

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            if (context.started || context.performed)
            {
                SendCommand("MOVE", context.ReadValue<Vector2>().ToString());
            }
            if (context.canceled)
            {
                SendCommand("MOVE", Vector2.zero.ToString());
            }
        }
    }

    public void FireCannon(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            if (context.started)
            {
                SendCommand("FIRE", "");
            }
        }
    }

    public void ShootCannonBall()
    {
        GameObject tempBall = MyCore.NetCreateObject(cannonballPrefab, -1, transform.position + transform.forward * 5, Quaternion.identity);
        Rigidbody tempRB = tempBall.GetComponent<Rigidbody>();
        if (tempRB != null)
        {
            tempBall.GetComponent<CannonBall>().attack = this.atk;
            tempRB.gameObject.layer = gameObject.layer;
            tempRB.linearVelocity = transform.forward * ballForce;
            canFire = false;
            SendUpdate("CANFIRE", canFire.ToString());
            SendUpdate("FIRESOUND", "1001");
        }
    }
}
