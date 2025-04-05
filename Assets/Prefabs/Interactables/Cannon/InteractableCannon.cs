using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

public class InteractableCannon : Interactable
{
    [SerializeField] float cannonMoveSpeed = 10;
    [SerializeField] int cannonballPrefab;
    Vector2 lastInput;
    Rigidbody rb;
    private float pitch = 0f;
    private float yaw = 0f;
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
            if (IsServer)
            {
                GameObject tempBall = MyCore.NetCreateObject(cannonballPrefab, -1, transform.position + transform.forward * 5, Quaternion.identity);
                Rigidbody tempRB = tempBall.GetComponent<Rigidbody>();
                if (tempRB != null)
                {
                    tempRB.linearVelocity = transform.forward * 20;
                }
            }
        }
    }

    public void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        base.Update();

    }

    public void FixedUpdate()
    {
        if (IsServer)
        {
            yaw += lastInput.x * cannonMoveSpeed * Time.deltaTime;
            pitch -= lastInput.y * cannonMoveSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
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
