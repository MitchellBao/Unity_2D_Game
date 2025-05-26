using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Selector02 : MonoBehaviour
{
    public TMP_Dropdown selector;
    public TMP_Text text;  // �������ʾ��ɫ���� Text
    public PlayerStatusBar playerStatusBar02;

    Data.CharacterInfo[] characterList;

    public bool IsSelected;

    private void Start()
    {
        selector.onValueChanged.AddListener(SelectChange);
        Setoptions();

        // Ĭ��ѡ���һ��ѡ��
        selector.value = 0;  // ����Ϊ��һ��ѡ��
        SelectChange(0);  // ��������ѡ���߼�
        IsSelected = true;  // ȷ����ɫ��ѡ��

        // ֻ���� Dropdown ��ʾ��Ĭ���ı�Ϊ "Virtual Guy"
        selector.captionText.text = "Virtual Guy";  // ֱ���޸� Dropdown ѡ������ʾ�ı�
    }

    public int GetSelectedIndex()
    {
        return selector.value;
    }

    private void Setoptions()
    {
        selector.options.Clear();
        characterList = Data.GetCharacterList();

        // ��ʵ�ʵĽ�ɫ������Ϊѡ����ӵ� Dropdown ��
        for (int i = 0; i < characterList.Length; i++)
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
        Sprite sprite = Resources.Load<Sprite>(info.avatar);
        playerStatusBar02.avatarImage.sprite = sprite;

        // ���� Dropdown ��ʾ����ı�
        selector.captionText.text = info.name;  // ����Ϊ��ǰѡ���ɫ������
    }
}
