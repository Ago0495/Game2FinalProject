using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipStats : Entity
{
    public GameObject[] cannonsArray;
    public InteractableRepairZone[] repairZoneArray;
    private float damageThreshold = 20;
    private float damageTracker = 0;
    public GameObject damageAudio;
    private void Start()
    {
        base.Start();
    }

    public void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
        if (flag == "Damaged")
        {
            if (IsClient)
            {
                damageAudio.GetComponent<AudioSource>().Play();
            }
        }
    }

    public override void OnDamageTaken(float damageTaken)
    {
        if (IsServer)
        {
            damageTracker += damageTaken;
            //Debug.Log("ShipHealth: " + health);
            repairZoneArray = gameObject.GetComponentsInChildren<InteractableRepairZone>();

            SendUpdate("Damaged", "111"); 

            if (damageTracker > damageThreshold && repairZoneArray.Length > 0)
            {
                int rand = Random.Range(0, repairZoneArray.Length);
                Debug.Log(rand);

                repairZoneArray[rand].GetComponent<InteractableRepairZone>().complete = false;
                repairZoneArray[rand].GetComponent<InteractableRepairZone>().SendUpdate("COMPLETE", false.ToString());
                damageTracker -= damageThreshold;
            }
        }
    }

    public override void OnDeath()
    {

    }

    public void TestDamage(InputAction.CallbackContext context)
    {
        if (IsServer && context.started)
        {
            Debug.Log("Force Take Damage");
            takeDamage(25);
        }
    }
}
