using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Selector02 : MonoBehaviour
{
    public TMP_Dropdown selector;
    public TMP_Text text;  // 这个是显示角色名的 Text
    public PlayerStatusBar playerStatusBar02;

    Data.CharacterInfo[] characterList;

    public bool IsSelected;

    private void Start()
    {
        selector.onValueChanged.AddListener(SelectChange);
        Setoptions();

        // 默认选择第一个选项
        selector.value = 0;  // 设置为第一个选项
        SelectChange(0);  // 立即触发选择逻辑
        IsSelected = true;  // 确保角色已选择

        // 只设置 Dropdown 显示框默认文本为 "Virtual Guy"
        selector.captionText.text = "Virtual Guy";  // 直接修改 Dropdown 选择框的显示文本
    }

    public int GetSelectedIndex()
    {
        return selector.value;
    }

    private void Setoptions()
    {
        selector.options.Clear();
        characterList = Data.GetCharacterList();

        // 将实际的角色名称作为选项添加到 Dropdown 中
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

        // 更新 Dropdown 显示框的文本
        selector.captionText.text = info.name;  // 更新为当前选择角色的名字
    }
}
