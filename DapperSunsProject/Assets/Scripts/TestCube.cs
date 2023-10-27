using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : Beat
{
    bool isBig;

   protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void DoBeat()
    {
        if (isBig)
        {
            transform.localScale -= Vector3.one;
            isBig = !isBig;
        }
        else
        {
            transform.localScale += Vector3.one;
            isBig = !isBig;
        }
    }
}