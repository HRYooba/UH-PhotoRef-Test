using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingImages : UGraphics
{
    UImage skyImg;
    public Texture appleTex;

    protected override void Setup()
    {
        skyImg = loadImage("Unicessing/Textures/sky");
        if (appleTex == null)
        {
            appleTex = loadImage("Unicessing/Textures/apple").texture;
        }
    }

    protected override void Draw ()
    {
        noLights();

        fill(255);
        imageMode(CORNER);      // Left Bottom
        //imageMode(CORNER_P5); // Left Top
        //imageMode(CENTER);    // Center
        image(skyImg, mouseX, mouseY);

        drawApples();

        ellipseMode(CENTER);
        blendMode(UMaterials.BlendMode.Opaque);
        texture(skyImg);
        fill(255);
        ellipse(0, 0, 2, 2);
        fill(255, 255, 0); // tint
        ellipse(1, 0, 2, 2);
        noTexture();
        fill(255);
        ellipse(2, 0, 2, 2);
    }

    void drawApples()
    {
        noDepthStep();
        blendMode(UMaterials.BlendMode.Transparent);
        for (int ix = 0; ix < 20; ix++)
        {
            for (int iy = 0; iy < 10; iy++)
            {
                float x = (ix - 10) * 1.5f;
                float y = (iy - 5) * 1.5f;
                float len = dist(x, y, mouseX, mouseY);
                float s = constrain(len * 0.1f, 0.2f, 1.0f);
                float r = s;
                texture(appleTex);
                fill(255, (int)(255 * s));
                //image(skyImg, x, y, s*2, s*2);
                ellipse(x, y, r, r);
            }
        }
        depthStep();
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
