using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectButton : MonoBehaviour
{
    public Selector01 selector01;
    public Selector02 selector02;

    public int PlayerIndex01;
    public int PlayerIndex02;

    [Header("Events")]
    public UnityEvent<SelectButton> OnSelectionFinished;
    public void OnClick()
    {
        if (selector01.IsSelected && selector02.IsSelected)
        {
            PlayerIndex01 = selector01.GetSelectedIndex() + 1;
            PlayerIndex02 = selector02.GetSelectedIndex() + 1;

            Debug.Log("ѡ���ɫ��" + PlayerIndex01 + " �� " + PlayerIndex02);

            OnSelectionFinished?.Invoke(this);
        }
        else
        {
            Debug.LogWarning("�𼱣�������ɫ��ѡ������");
        }
    }

}
