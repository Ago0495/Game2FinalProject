using UnityEngine;

public class ShipStats : Entity
{
    public GameObject[] cannonsArray;
    public GameObject[] repairZoneArray;
    private float damageThreshold = 20;
    private float damageTracker = 0;
    private void Start()
    {
        base.Start();

        foreach (GameObject rz in repairZoneArray)
        {
            InteractableRepairZone tempRZ = rz.GetComponent<InteractableRepairZone>();
            if (tempRZ != null)
            {
                tempRZ.complete = true;
            }
        }

        takeDamage(25);
    }

    public override void OnDamageTaken(float damageTaken)
    {
        damageTracker += damageTaken;
        if (IsServer)
        {
            if (damageTracker > damageThreshold && repairZoneArray.Length > 0)
            {
                int rand = Random.Range(0, repairZoneArray.Length);
                repairZoneArray[rand].GetComponent<InteractableRepairZone>().complete = false;
                damageTracker -= damageThreshold;
            }
        }
    }
}
