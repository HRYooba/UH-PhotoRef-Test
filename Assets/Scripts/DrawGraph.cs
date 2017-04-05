using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unicessing;

public class DrawGraph : UGraphics
{

	public UH uh;

    protected override void Setup()
    {
		uh = GameObject.Find("UnlimitedHand").GetComponent<UH>();
    }
    protected override void Draw()
    {
		
    }
}
