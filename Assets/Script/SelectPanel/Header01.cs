using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Header01 : MonoBehaviour
{
    public static Header01 Instance;
    public TMP_Text characterName;
    public Image avatar;

    private void Awake()
    {
        Instance = this;
    }

    public void SetInfo(Data.CharacterInfo info)
    {
        characterName.text = info.name;
        Sprite sprite = Resources.Load<Sprite>(info.avatar);
        avatar.sprite = sprite;
    }

}
