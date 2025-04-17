using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
using System.Collections;

public class InteractableCannon : Interactable
{
    [SerializeField] float cannonMoveSpeed = 10;
    [SerializeField] int cannonballPrefab;
    [SerializeField] float ballForce = 50;
    [SerializeField] public float atk;
    Vector2 lastInput;
    Rigidbody rb;
    private float pitch = 0f;
    private float yaw = 0f;
    private float startYaw = 0f;
    private float startPitch = 0f;
    private bool valuesSet = false;
    public bool canFire = true;
    public float reloadTime = 2;
    public float reloadTimer = 0;

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
                GameObject tempBall = MyCore.NetCreateObject(cannonballPrefab, -1, transform.position + transform.forward * 5, Quaternion.identity);
                Rigidbody tempRB = tempBall.GetComponent<Rigidbody>();
                if (tempRB != null)
                {
                    tempBall.GetComponent<CannonBall>().atk = this.atk;
                    tempRB.gameObject.layer = gameObject.layer;
                    tempRB.linearVelocity = transform.forward * ballForce;
                    canFire = false;
                    SendUpdate("CANFIRE", canFire.ToString());
                }
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
                reloadTimer -= reloadTime;
            }
            if (IsServer)
            {
                SendUpdate("CANFIRE", canFire.ToString());
            }
        }
    }

    public void FixedUpdate()
    {
        // pitch and yaw missing up rotation when creating new object
        if (IsServer && Owner >= 0)
        {
            yaw += lastInput.x * cannonMoveSpeed * Time.deltaTime;
            pitch -= lastInput.y * cannonMoveSpeed * Time.deltaTime;

            yaw = Mathf.Clamp(yaw, startYaw + -25, startYaw + 25);
            pitch = Mathf.Clamp(pitch, startPitch - 15, startPitch + 10);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }

    public override void SetValues(int oldInteractable)
    {
        InteractableCannon oldCannon = MyCore.NetObjs[oldInteractable].GetComponent<InteractableCannon>();

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
}
