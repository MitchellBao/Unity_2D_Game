using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    public void ShowResult(string message)
    {
        resultText.text = message;
        
    }
}
