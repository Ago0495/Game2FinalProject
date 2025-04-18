using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using UnityEngine.UI;

public class CannonUI : MonoBehaviour
{
    [SerializeField] GameObject UICanvasObj;
    Canvas UICanvas;
    [SerializeField] Image background;
    [SerializeField] Image background2;
    [SerializeField] Image progressBar;
    InteractableCannon cannon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UICanvas = GetComponent<Canvas>();
        cannon = transform.parent.GetComponent<InteractableCannon>();

        if (cannon != null )
        {
            if (cannon.IsLocalPlayer)
            {
                UICanvas.enabled = true;
            }
            else
            {
                UICanvas.enabled = false; 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!cannon.canFire && cannon.IsLocalPlayer)
        {
            UICanvas.enabled = true;

            if (cannon.MyCore.NetObjs[cannon.user].GetComponent<PlayerStats>().skill == 0)
            {
                background.enabled = false;
                background2.enabled = true;
                background2.transform.GetChild(0).GetComponent<Image>().enabled = true;
                background2.transform.GetChild(1).GetComponent<Image>().enabled = true;
            }
            else
            {
                background.enabled = true;
                background2.enabled = false;
                background2.transform.GetChild(0).GetComponent<Image>().enabled = false;
                background2.transform.GetChild(1).GetComponent<Image>().enabled = false;
            }

            progressBar.fillAmount = cannon.reloadTimer / cannon.reloadTime;
        }
        else
        {
            UICanvas.enabled = false;
        }
    }
}
