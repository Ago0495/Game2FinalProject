using UnityEngine;
using NETWORK_ENGINE;

public class InteractableRepairZone : Interactable
{
    ShipStats playerShip;
    float damage = 2;
    float damageCooldownTimer = 0f;
    float damageCooldownTime = 2f;

    

    private void Start()
    {
        base.Start();
        if (transform.parent != null)
        {
            playerShip = transform.parent.GetComponent<ShipStats>();
        }
    }

    private void Update()
    {
        base.Update();

        if (IsServer && playerShip != null)
        {
            damageCooldownTimer += Time.deltaTime;
            if (damageCooldownTimer > damageCooldownTime)
            {
                playerShip.takeDamage(damage);
                damageCooldownTimer -= damageCooldownTime;
            }
        }
        else if (playerShip == null && transform.parent != null)
        {
            playerShip = transform.parent.GetComponent<ShipStats>();
        }
    }
}
