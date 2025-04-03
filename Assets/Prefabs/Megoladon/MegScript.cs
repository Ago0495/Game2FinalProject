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
        if (attacking)
        {
            angle += (moveSpeed / radius) * Time.deltaTime;

            Vector3 centerPoint = sharkArea.transform.position;

            Vector3 orbitPosition = centerPoint + new Vector3(
                Mathf.Cos(angle) * radius, 
                0,                         
                Mathf.Sin(angle) * radius  
            );

            Vector3 targetVelocity = (orbitPosition - transform.position).normalized * moveSpeed;

            MyRig.linearVelocity = targetVelocity;

            Vector3 lookPosition = centerPoint - transform.position;
            lookPosition.y = 0; 
            transform.rotation = Quaternion.LookRotation(lookPosition);
        }
        else
        {
            //markPosition = sharkArea.transform.position + (Vector3.right * radius) * Mathf.Cos(Time.time * circleTime) + (Vector3.forward * radius) * Mathf.Sin(Time.time * circleTime);
            //myAgent.SetDestination(markPosition);
            //myAgent.speed = (moveSpeed);
            angle += (moveSpeed / radius) * Time.deltaTime;

            Vector3 centerPoint = sharkArea.transform.position;

            Vector3 orbitPosition = centerPoint + new Vector3(
                Mathf.Cos(angle) * radius, 
                0,                         
                Mathf.Sin(angle) * radius  
            );

            Vector3 targetVelocity = (orbitPosition - transform.position).normalized * moveSpeed;

            MyRig.linearVelocity = targetVelocity;

            Vector3 lookPosition = centerPoint - transform.position;
            lookPosition.y = 0; 
            transform.rotation = Quaternion.LookRotation(lookPosition);
        }
    }
}
