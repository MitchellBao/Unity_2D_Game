using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnStatusBar : MonoBehaviour
{
    public Image turnImage;
    public TextMeshProUGUI playerText01;
    public TextMeshProUGUI playerText02;

    public void OnTurnChange(float percentage)
    {
        turnImage.fillAmount = percentage;
    }

    public void OnPointChange(int p1, int p2)
    {
        playerText01.text = p1.ToString();
        playerText02.text = p2.ToString();  
    }
}
