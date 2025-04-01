using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.forward * 10000f);
        rb.AddTorque(transform.up * 20000f);
    }
}
