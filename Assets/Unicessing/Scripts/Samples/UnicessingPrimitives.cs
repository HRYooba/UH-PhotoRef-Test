using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingPrimitives : UGraphics
{
	protected override void Setup ()
    {
    }

    protected override void Draw ()
    {
        drawLinePoints();
        drawEllipses();
        drawRects();
        drawText();
        drawSpheres();
        drawBoxes();
        drawCurves();
    }

    void drawLinePoints()
    {
        stroke(0, 255, 255);
        line(-10, 0, mouseX, mouseY);

        for (int i = 0; i < 255; i+=3) {
            stroke(255, i, i);
            point(-10 + i * 0.05f, 10 - i * 0.02f);
        }
    }

    void drawEllipses()
    {
        stroke(0, 255, 255);
        fill(255);
        ellipse(0, 0, 8, 6);
        noStroke();

        fill(0);
        ellipse(constrain(mouseX, -2.0f, 2.0f), constrain(mouseY, -0.8f, 0.8f), 2, 4);
    }

    void drawRects()
    {
        fill(255, 255, 0);
        rect(0, 8, 2, 2);

        pushStyle();

        blendMode(UMaterials.BlendMode.Transparent);
        fill(0, 255, 255, 128);
        rect(1, 8.2f, 2, 2);

        blendMode(UMaterials.BlendMode.Add);
        pushMatrix();
        fill(0, 255, 255, 128);
        rotate(frameSec * 0.3f);
        rect(-1, 5, 2, 2);
        popMatrix();

        popStyle(); //blendMode(UMaterials.BlendMode.Opaque);
    }

    void drawText()
    {
        fill(200);
        textSize(1);
        textAlign(CENTER, CENTER);
        text("Primitives", 0, 7);
    }

    void drawSpheres()
    {
        for (int i = 0; i < 6; i++)
        {
            if (i % 2 == 1)
            {
                stroke(0, 255, 0);
                noFill();
            }
            else
            {
                noStroke();
                fill(255 - i * 30, 255, 255);
            }
            pushMatrix();
            sphereDetail(4 + i * 2);
            translate(-3 * 2.5f + 3 * i, 4, 0);
            rotateY(frameSec * 0.5f);
            sphere(1);
            popMatrix();
        }
    }

    void drawBoxes()
    {
        pushStyle();
        for (int i = 0; i < 6; i++)
        {
            if (i % 2 == 0)
            {
                stroke(0, 255, 0);
                noLights();
                //noFill();
                fill(0);
            }
            else
            {
                noStroke();
                lights();
                fill(255, 255, 255 - i * 30);
            }
            pushMatrix();
            sphereDetail(4 + i * 2);
            translate(-3 * 2.5f + 3 * i, 2, 0);
            rotateX(frameSec * 0.3f);
            box(1, 1, 0.1f + i * 0.3f);
            popMatrix();
        }
        popStyle();
    }

    void drawCurves()
    {
        stroke(255, 128, 0);
        curve(-10, 0, -10, 0, mouseX, mouseY, 10, 0);

        beginShape(UShape.VertexType.CURVE_LINE_STRIP);
        stroke(0, 255, 0);
        curveVertex(-10, 0);
        curveVertex(-10, 0);
        stroke(255, 0, 0);
        curveVertex(-5, 8);
        stroke(255);
        curveVertex(mouseX, mouseY);
        stroke(0, 0, 255);
        curveVertex(10, 0);
        curveVertex(10, 0);
        endShape();
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
