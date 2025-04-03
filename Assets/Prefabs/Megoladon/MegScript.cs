using UnityEngine;
using UnityEngine.Timeline;
using static UnityEngine.GraphicsBuffer;

public class MegScript : Enemy
{
    [SerializeField] private GameObject sharkArea;
    [SerializeField] private float radius;
    [SerializeField] private float circleTime;

    private float angle = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        sharkArea = GameObject.FindGameObjectWithTag("SArea");
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (false)
        {

        }
        else
        {
            //markPosition = sharkArea.transform.position + (Vector3.right * radius) * Mathf.Cos(Time.time * circleTime) + (Vector3.forward * radius) * Mathf.Sin(Time.time * circleTime);
            //myAgent.SetDestination(markPosition);
            //myAgent.speed = (moveSpeed);
            angle += (moveSpeed/radius) * Time.fixedDeltaTime;

            // Calculate target velocity in circular motion
            Vector3 targetVelocity = new Vector3(
                -Mathf.Sin(angle) * moveSpeed,
                0,
                Mathf.Cos(angle) * moveSpeed
            );

            // Apply velocity to Rigidbody
            MyRig.linearVelocity = targetVelocity;

            // Make shark face movement direction
            Vector3 lookPosition = sharkArea.transform.position - transform.position;
            lookPosition.y = 0; // Keep it horizontal
            transform.rotation = Quaternion.LookRotation(lookPosition);
        }
    }
}
