using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingCustom : UGraphics
{
    UShape circle, star;

    protected override void Setup()
    {
        circle = createEllipse(0, 0, 6, 6);
        circle.enableStyle();
        circle.fill(color(0, 64));
        circle.blendMode(UMaterials.BlendMode.Transparent);

        star = createShape();
        star.beginShape(UShape.VertexType.TRIANGLE_FAN);
        star.isClockwise = false;
        star.vertex(0.0f, 0.0f);
        star.vertex(0.0f, -0.5f);
        star.vertex(0.14f, -0.20f);
        star.vertex(0.47f, -0.15f);
        star.vertex(0.23f, 0.07f);
        star.vertex(0.29f, 0.40f);
        star.vertex(0.0f, 0.25f);
        star.vertex(-0.29f, 0.4f);
        star.vertex(-0.23f, 0.07f);
        star.vertex(-0.47f, -0.15f);
        star.vertex(-0.14f, -0.2f);
        star.vertex(0.0f, -0.5f);
        star.endShape();

        rotateDegrees();
    }

    protected override void Draw()
    {
        drawBigStar();
        drawLittleStars();
        drawCircles();
    }

    void drawBigStar()
    {
        pushMatrix();
        scale(4);
        rotateY(frameSec * 45.0f);
        fill(255, 255, 0);
        draw(star);

        rotateY(180);
        fill(255);
        draw(star);
        popMatrix();
    }

    void drawLittleStars()
    {
        for (int i = 0; i < 360; i += 30)
        {
            pushMatrix();
            fill(255, 255, (int)map(i, 0, 360, 0, 255));
            rotateZ(i);
            translate(3.0f, 0, 0);
            rotateZ(frameCount * -0.5f);
            draw(star);
            popMatrix();
        }
    }

    void drawCircles()
    {
        Vector3 mpos1 = planeMousePos(0, 0, 2);
        draw(circle, mpos1.x, mpos1.y, 2);
        Vector3 mpos2 = planeMousePos(0, 0, 5);
        draw(circle, mpos2.x, mpos2.y, 5);
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
