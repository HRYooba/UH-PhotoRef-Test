using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingPteridophyte : UGraphics
{
    float x = 0.0f;
    float y = 0.0f;

	protected override void Setup ()
    {
    }

    protected override void Draw ()
    {
        float t = modulo(frameSec * 0.3f, 3.0f);
        if (t > 2) t = 3 - t;
        else if (t > 1) t = 1;
        else
        {
            t = 1 - t;
            t *= t; t *= t;
            t = 1 - t;
        }

        randomSeed(0);
        for (int i = 0; i < 100 * 100; i++)
        {
            float tx = 0;
            float ty = 0;

            Color col;
            if (random(10) < 3) col = color(128, 255, 128);
            else col = color(128, 128, 64);
            col *= t;
            stroke(col);

            float ir = i * 0.01f;
            float px = x * t + random(-ir, ir) * (1 - t);
            float py = y * t + random(-ir, ir) * (1 - t);
            float pz = 0.0f * t + random(-ir, ir) * (1 - t);
            point(px, py, pz);

            float sw = random(100);
            if (sw > 15)
            {
                tx = 0.85f * x + 0.04f * y;
                ty = -0.04f * x + 0.85f * y + 1.6f;
            }
            else if (sw > 8)
            {
                tx = -0.15f * x + 0.28f * y;
                ty = 0.26f * x + 0.24f * y + 0.44f;
            }
            else if (sw > 1)
            {
                tx = 0.2f * x - 0.26f * y;
                ty = 0.23f * x + 0.22f * y + 1.6f;
            }
            else
            {
                tx = 0;
                ty = y * 0.16f;
            }

            x = tx;
            y = ty;
        }
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
