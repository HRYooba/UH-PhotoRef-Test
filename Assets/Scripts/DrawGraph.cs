using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unicessing;

public class DrawGraph : UGraphics
{

    public UH uh;

    protected override void Setup()
    {

    }
    protected override void Draw()
    {
		translate(3, 0);
        for (int i = 0; i < 8; i++)
        {
            fill(0, 255, 0);
            rect(i + 0.3f, 0, 0.5f, uh.UHPR[i] / 100.0f);
			textSize(0.5f);
			text("" + i, i + 0.3f, -0.5f);
        }
    }
}
