using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingCurves : UGraphics
{
    Vector2 mpos = new Vector2();

    protected override void Setup()
    {
        blendMode(UMaterials.BlendMode.Add);
    }

    protected override void Draw()
    {
        mpos = Vector2.Lerp(mpos, new Vector2(mouseX, mouseY), 0.05f);

        scale(0.2f, 0.2f, 0.2f);
        float t = frameSec * 0.7f;
        float width = 100.0f;
        float s = width / 3 + width / 3 * sin(t);
        float r = width / 100;
        stroke(20 + (int)(cos(t * 1.5f) * 20), 20 + (int)(sin(t) * 20), 200, 255);
        beginShape(UShape.VertexType.CURVE_LINE_STRIP);
        for (int i = 0; i < 100; i++)
        {
            float mx = mpos.x + cos(i * 0.1f + t * 1.0f) * s;
            float my = mpos.y + sin(i * 0.1f + t * 1.0f) * s;
            float mz = cos(i * 1.0f + t * 1.0f) * s * 10.0f;
            curveVertex(mx, -s * 0.5f + s + i * r, mz * 0.5f);
            curveVertex(mx, my, mz);
            curveVertex(my, mx, mz);
            curveVertex(-s * 0.5f + s + i * r, my, mz * 0.5f);
        }
        endShape();
        stroke(255);
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
