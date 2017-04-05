using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingZenTexts: UGraphics
{
    int seed = 0;
    protected override void Setup()
    {
        textSize(10);
        textAlign(CENTER, CENTER);
        seed = random(100);
    }

    protected override void Draw()
    {
        float zenLevel = frameSec * 0.02f;

        Color bgCol = Color.Lerp(color(255), color(0, 20, 10), zenLevel);
        background(bgCol);

        drawZenTexts(zenLevel);
    }


    void drawZenTexts(float level)
    {
        level = constrain(level, 0, 1);
        randomSeed(seed);
        for(int i=0; i<100; i++)
        {
            pushMatrix();
            float r = random(50, 100) + level * 200;
            float x = cos(radians(random(0, 360))) * r;
            float y = sin(radians(random(0, 360))) * r;
            float z = cos(radians(random(0, 360))) * r;
            translate(x, y, z);
            lookAtCamera();
            Color col = color(255, random(100, 255), random(255));
            col = lerpColor(col, color(brightness(col), 0), level);
            fill(col);
            text("Zen");
            popMatrix();
        }
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
