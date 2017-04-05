using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingEarth : UGraphics
{
    public float earthR = 6.378f;
    public float moonR = 1.737f;
    UImage earthImg, moonImg, ringImg;

    protected override void Setup()
    {
        earthImg = loadImage("Unicessing/Textures/earth");
        moonImg = loadImage("Unicessing/Textures/moon");
        ringImg = loadImage("Unicessing/Textures/ring");
    }

    protected override void Draw ()
    {
        translate(3, 0, 0);
        blendMode(UMaterials.BlendMode.Transparent);

        // Earth
        texture(earthImg);
        pushMatrix();
            rotateZ(radians(-23.45f));
            rotateY(frameSec * 0.05f);
            sphere(earthR);
        popMatrix();

        // EarthRing
        push();
            noLights();
            blendMode(UMaterials.BlendMode.Add);
            lookAtCamera();
            translate(0, 0, -0.01f);
            fill(0, 128, 255, 220);
            texture(ringImg);
            float earthRingWH = earthR * 2.2f;
            rectMode(CENTER);
            rect(0, 0, earthRingWH, earthRingWH);
        pop();

        // Moon
        rotateY(frameSec * 0.2f);
        translate(earthR * 2 + moonR, 0, 0);
        rotateY(frameSec * -0.2f);
        texture(moonImg);
        sphere(moonR);
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
