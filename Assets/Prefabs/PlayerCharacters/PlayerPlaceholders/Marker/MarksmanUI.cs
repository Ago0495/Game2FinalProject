using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using UnityEngine.UI;

public class MarksmanUI : MonoBehaviour
{

    [SerializeField] GameObject UICanvasObj;
    Canvas UICanvas;
    [SerializeField] Image background;
    [SerializeField] Image progressBar;
    MarksmanPlayerController marksman;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UICanvas = GetComponent<Canvas>();
        marksman = transform.parent.GetComponent<MarksmanPlayerController>();

        if (marksman != null)
        {
            if (marksman.IsLocalPlayer)
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
        if (!marksman.canFire && marksman.IsLocalPlayer)
        {
            UICanvas.enabled = true;

            progressBar.fillAmount = marksman.reloadTimer / marksman.reloadTime;
        }
        else
        {
            UICanvas.enabled = false;
        }
    }
}
