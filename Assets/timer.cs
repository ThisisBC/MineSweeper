using TMPro;
using UnityEngine;

public class timer : MonoBehaviour
{
    public float timeCheck = 0;
    public GameObject timeText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<main>().gameProcedure==1)timeCheck += Time.deltaTime;
        timeText.GetComponent<TMP_Text>().text = timeCheck.ToString("0.00");
    }
}
