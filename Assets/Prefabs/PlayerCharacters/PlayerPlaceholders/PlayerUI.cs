using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    NetworkPlayerController playerController;
    ShipStats shipStats;
    Image hpBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject tempShip = GameObject.FindGameObjectWithTag("SHIP");
        GameObject tempPlayer = transform.parent.gameObject;

        if (tempShip != null)
        {
            shipStats = tempShip.GetComponent<ShipStats>();
            if ( shipStats == null ) 
            {
                Debug.LogError("ERROR: No ShipStats Found, tempShip = " + tempShip.name);
            }
        }

        playerController = tempPlayer.GetComponent<NetworkPlayerController>();
        if (playerController != null )
        {
            if (!playerController.IsServer && playerController.IsLocalPlayer)
            {
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("ERROR: No NetworkPlayerController Found, tempPlayer = " + tempPlayer.name);
        }

        hpBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.fillAmount = shipStats.getHealth() / shipStats.getMaxHealth();
    }
}
