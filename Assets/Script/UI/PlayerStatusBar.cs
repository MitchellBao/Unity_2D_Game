using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusBar : MonoBehaviour
{
    public Image avatarImage;
    public Image actionPointImage;
    public Image actionPointDelayImage;
    public Image skillImage;
    public Image diamondStatusImage;
    public Image frozenStatusImage;

    // 钻石类型的精灵（在 Inspector 中赋值）
    public Sprite redDiamondSprite;
    public Sprite blueDiamondSprite;
    public Sprite greenDiamondSprite;
    public Sprite poisonousDiamondSprite;

    public void Update()
    {
        if (actionPointDelayImage.fillAmount > actionPointImage.fillAmount)
        {
            actionPointDelayImage.fillAmount -= Time.deltaTime ;
        }
        else
        {
            actionPointDelayImage.fillAmount = actionPointImage.fillAmount;
        }
    }

    // Update Action Point
    public void OnActionPointChange(float percentage)
    {
        actionPointImage.fillAmount = percentage;
    }

    // Update Diamond Status 
    public void OnDiamondStatusChange(bool hasDiamond, DiamondKinds kind)
    {
        diamondStatusImage.enabled = hasDiamond;
        if (hasDiamond)
        {
            switch (kind)
            {
                case DiamondKinds.redDiamond:
                    diamondStatusImage.sprite = redDiamondSprite;
                    break;
                case DiamondKinds.blueDiamond:
                    diamondStatusImage.sprite = blueDiamondSprite;
                    break;
                case DiamondKinds.greenDiamond:
                    diamondStatusImage.sprite = greenDiamondSprite;
                    break;
                case DiamondKinds.poisonousDiamond:
                    diamondStatusImage.sprite = poisonousDiamondSprite;
                    diamondStatusImage.rectTransform.sizeDelta = new Vector2(40, 40);
                    break;
                default:
                    Debug.LogWarning("未知的钻石类型，使用默认精灵");
                    break;
            }
        }
    }

    // Update Frozen Status 
    public void OnFrozenStatusChange(bool isFrozen)
    {
        frozenStatusImage.enabled = isFrozen;
    }

    // Update Skill Status
    public void OnSkillUseChange(bool isSkillUsable)
    {
        skillImage.fillAmount = isSkillUsable ? 1f : 0f; // 技能使用中填充
    }

}
