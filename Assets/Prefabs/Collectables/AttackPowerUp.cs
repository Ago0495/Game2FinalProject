using System;
using UnityEngine;

public class AttackPowerUp : Collectable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnCollected()
    {
        if (IsServer)
        {
            cannons = GameObject.FindGameObjectsWithTag("PCANNON");
            Debug.Log("Attack Up Collected");
            foreach(GameObject cannon in cannons)
            {
                InteractableCannon pCannon = cannon.GetComponent<InteractableCannon>();
                pCannon.atk += 10f;
                Debug.Log("CannonAtk upgraded!");
            }
        }

    }
}
