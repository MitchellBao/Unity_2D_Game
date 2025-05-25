using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class Selector02 : MonoBehaviour
{

    public TMP_Dropdown selector;
    public TMP_Text text;

    Data.CharacterInfo[] characterList;

    public bool IsSelected;

    private void Start()
    {
        selector.onValueChanged.AddListener(SelectChange);
        Setoptions();
    }

    public int GetSelectedIndex()
    {
        return selector.value;
    }
    private void Setoptions()
    {
        selector.options.Clear();
        characterList = Data.GetCharacterList();
        for (int i = 0; i <  characterList.Length; i++)
        {
            TMP_Dropdown.OptionData item = new TMP_Dropdown.OptionData();
            item.text = characterList[i].name;
            selector.options.Add(item);
        }

    }

    private void SelectChange(int index)
    {
        IsSelected = true;
        Data.CharacterInfo info = characterList[index];
        Header02.Instance.SetInfo(info);
    }



}
