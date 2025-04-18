using System;
using UnityEngine;

public class DefensePowerUp : Collectable
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
            Debug.Log("Defense Power Up collected");
            Entity entity = ship.GetComponent<Entity>();
            entity.defense = 0.5f;
            Debug.Log("entity defense is " + entity.defense);
        }

    }
}
