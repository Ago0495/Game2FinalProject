using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using UnityEngine.UI;

public class CannonUI : MonoBehaviour
{
    [SerializeField] GameObject UICanvasObj;
    Canvas UICanvas;
    [SerializeField] Image background;
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

            progressBar.fillAmount = cannon.reloadTimer / cannon.reloadTime;
        }
        else
        {
            UICanvas.enabled = false;
        }
    }
}
