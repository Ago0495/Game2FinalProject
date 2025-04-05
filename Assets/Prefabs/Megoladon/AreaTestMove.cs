using UnityEngine;

public class AreaTestMove : MonoBehaviour
{
    public Rigidbody body;
    public int speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.linearVelocity = new Vector3(speed, 0,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
