using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractableHelm : Interactable
{
    [SerializeField] int speed;
    Vector2 lastInput;
    Rigidbody rb;
    [SerializeField] Rigidbody shipRB;
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
    }

    public override void NetworkedStart()
    {
        base.NetworkedStart();

        //shipRB = GameObject.FindGameObjectWithTag("SHIP").GetComponent<Rigidbody>();
    }

    public void Start()
    {
        base.Start();
    }

    public void Update()
    {
        base.Update();

        if (IsServer)
        {
            if (shipRB != null)
            {
                shipRB.AddForce(transform.forward * lastInput.y * speed);
            }
            else
            {
                shipRB = GameObject.FindGameObjectWithTag("SHIP").GetComponent<Rigidbody>();
            }
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

}
