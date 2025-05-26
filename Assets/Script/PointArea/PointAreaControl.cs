using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAreaControl : MonoBehaviour
{
    public int Point = 0;
    public float checkRaduis;
    public LayerMask player;
    void Update()
    {
        DiamondGet();
    }

    public void DiamondGet()
    {
        Collider2D collider2D= Physics2D.OverlapCircle(transform.position, checkRaduis, player);
        if (collider2D != null)
        {
            bool isCarryDiamond = collider2D.gameObject.GetComponent<PlayerControl>().isGetDiamond;
            if (isCarryDiamond)
            {
                collider2D.gameObject.GetComponent<PlayerControl>().isGetDiamond = false;
                DiamondKindCheck(collider2D);
            }
        }
    }

    public void DiamondKindCheck(Collider2D collider2D)
    {
        DiamondKinds diamondKind=collider2D.gameObject.GetComponent<PlayerControl>().diamondKind;
        if (diamondKind == DiamondKinds.poisonousDiamond)
        {
            Point -= 1;
        }
        else
        {
            Point += 1;
        }
        
    }
}
