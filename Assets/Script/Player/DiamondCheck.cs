using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondCheck : MonoBehaviour
{
    public float checkRaduis;
    public LayerMask Diamond;
    public PlayerControl PlayerControl;

    private void Awake()
    {
        PlayerControl = GetComponent<PlayerControl>();
    }
    private void Update()
    {
        DiamondChecking();
    }

    private void DiamondChecking()
    {
        Collider2D other = Physics2D.OverlapCircle(transform.position, checkRaduis, Diamond);
       if (Physics2D.OverlapCircle(transform.position, checkRaduis, Diamond))
       {
            PlayerControl.isGetDiamond = true;
            PlayerControl.diamondKind=other.gameObject.GetComponent<Diamond>().DiamondKinds;
            Destroy(other.gameObject);
       }
    }
}
