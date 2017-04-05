using UnityEngine;
using Unicessing;

public class UnicessingSea : UGraphics
{
    protected override void Draw()
    {
        float t = frameSec * 4.0f;
        for (int z = -20; z < 20; z++)
        {
            for (int x = -30; x < 30; x++)
            {
                pushMatrix();
                float y = cos((x + t * 1.2f) / TWO_PI) + sin((z + t) / TWO_PI);
                int c = (int)max(0, y * 200);
                fill(c, 55 + c, 255);
                translate(x, y, z);
                box(0.9f, 0.9f, 0.9f);
                popMatrix();
            }
        }
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
