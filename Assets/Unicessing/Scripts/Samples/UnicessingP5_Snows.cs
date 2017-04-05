using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingP5_Snows: UGraphics
{
    protected override void Setup()
    {
        // P2D or P3D : Processing Coordinate System x=1, y=-1, z=-1
        size(640, 480, P2D, 0.01f);
    }

    protected override void Draw()
    {
        background(0, 20, 50);
        noLights();
        drawSnow(1);
        drawTree();
        drawSnow(2);
    }

    void drawTree()
    {
        push();
        translate(width / 2, height * 0.05f);
        fill(0, 150, 0);
        for (var i = 0; i < 4; i++)
        {
            translate(0, height * 0.1f);
            scale(1.3f);
            float wh = height * 0.15f;
            triangle(0, 0, -wh, wh, wh, wh);
        }
        pop();
    }

    void drawSnow(float s)
    {
        randomSeed((int)s);
        fill(200);
        for (var i = 0; i < 100; i++)
        {
            float r = random(1, 2) * height * 0.01f * s;
            float z = r * 0.05f;
            float xt = (frameCount60 * -0.2f + sin(frameCount60 * random(0.04f, 0.06f))) * z;
            float yt = frameCount60 * z * 0.5f;
            float x = modulo(random(width) + xt, width);
            float y = modulo(random(height) + yt, height);
            ellipse(x, y, r, r);
        }
        noFill();
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
