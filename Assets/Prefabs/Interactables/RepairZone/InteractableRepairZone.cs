using UnityEngine;

public class InteractableRepairZone : Interactable
{
    ShipStats playerShip;
    float damage = 2;
    float damageCooldownTimer = 0f;
    float damageCooldownTime = 2f;
    private void Start()
    {
        base.Start();
        playerShip = transform.parent.GetComponent<ShipStats>();
    }

    private void Update()
    {
        if (IsServer)
        {
            damageCooldownTimer += Time.deltaTime;
            if (damageCooldownTimer > damageCooldownTime)
            {
                playerShip.takeDamage(damage);
                damageCooldownTimer -= damageCooldownTime;
            }
        }
        base.Update();
    }
}
