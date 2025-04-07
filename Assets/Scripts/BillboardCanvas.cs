using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    public GameObject cameraObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraObj.transform.position);
    }
}
