using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RepairzoneUI : MonoBehaviour
{
    InteractableRepairZone repairZone;
    Canvas canvas;
    [SerializeField] GameObject InputPrefab;

    bool isActive = false;

    char[] possibleInput = {'w','a','s','d'};
    int maxInput = 8;

    int index = 0;

    void Start()
    {
        repairZone = transform.parent.GetComponent<InteractableRepairZone>();
        canvas = GetComponent<Canvas>();

        for (int i = 0; i < maxInput; i++)
        {
            int rand = Random.Range(0, possibleInput.Length);
            GameObject tempPrefab = Instantiate(InputPrefab, Vector3.zero, Quaternion.identity);
            tempPrefab.transform.GetChild(0).GetComponent<TMP_Text>().text = "" + possibleInput[rand];
            //tempPrefab.transform.parent = this.transform.GetChild(0);
            tempPrefab.transform.SetParent(transform.GetChild(0));
        }
    }


    void Update()
    {
        if (repairZone != null)
        {
            if (!repairZone.IsServer && repairZone.IsLocalPlayer)
            {
                canvas.enabled = true;
                isActive = true;

                UIIsOpen();
            }
            else
            {
                canvas.enabled = false;
            }
        }
    }

    void UIIsOpen()
    {
        if (Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
            {
                if (c == transform.GetChild(0).GetChild(index).GetChild(0).GetComponent<TMP_Text>().text.ToCharArray()[0])
                {
                    Color tempColor = transform.GetChild(0).GetChild(index).GetComponent<Image>().color;
                    tempColor.a = 0.1f;
                    transform.GetChild(0).GetChild(index).GetComponent<Image>().color = tempColor;
                    index++;
                }
                else
                {
                    for (int i = 0; i < index; i++)
                    {
                        Debug.Log("Reset: " + index);
                        Color tempColor = transform.GetChild(0).GetChild(i).GetComponent<Image>().color;
                        tempColor.a = 1f;
                        transform.GetChild(0).GetChild(i).GetComponent<Image>().color = tempColor;
                    }
                    index = 0;
                }
                if (index >= maxInput)
                {
                    repairZone.SendCommand("COMPLETE", true.ToString());
                }
            }
        }
    }
}
