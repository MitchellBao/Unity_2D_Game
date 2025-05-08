using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondCheck : MonoBehaviour
{
    public float checkRaduis;
    public LayerMask Diamond;


    // Update is called once per frame
    void Update()
    {
        DiamondChecking();
    }

    private void DiamondChecking()
    {
            Physics2D.OverlapCircle(transform.position, checkRaduis,Diamond);
    }
}
