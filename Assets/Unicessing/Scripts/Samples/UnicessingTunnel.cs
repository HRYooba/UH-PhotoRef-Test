using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingTunnel: UGraphics
{
    public float range = 100.0f;
    public float speed = 1.0f;

    protected override void Setup ()
    {
    }

    protected override void Draw ()
    {
        drawTurnnel();
    }

    void drawTurnnel()
    {
        float s = range;
        float t = -frameSec * speed;
        for(int iz=0; iz<20; iz++)
        {
            float z = modulo(iz + t, 20) * s;

            beginShape(UShape.VertexType.CURVE_LINE_STRIP);
            for (int a = 0; a < 360; a += 10)
            {
                float x = sin(radians(a)) * 2 * s;
                x *= 2;
                float y = cos(radians(a)) * s;

                stroke(0, 128, 255);
                curveVertex(x, y, z);
                if(a==0) curveVertex(x, y, z);
                float r = 3.0f;

                if ((a + iz * 10) % 30 == 0)
                {
                    pushMatrix();
                    blendMode(UMaterials.BlendMode.Opaque);
                    translate(x, y, z + s * 0.5f);
                    rotateZ(t);
                    scale(modulo(t, 5.0f));
                    fill(10, 100, 255);
                    box(0, 0, 0, r, r, r);
                    popMatrix();
                }
            }
            blendMode(UMaterials.BlendMode.Add);
            endShape(UShape.CloseType.CLOSE);
        }
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
