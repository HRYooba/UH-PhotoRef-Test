using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingStars : UGraphics
{
    public float rangeMin = 0.0f;
    public float rangeMax = 5.0f;
    UShape stars;

	protected override void Setup ()
    {
        stars = createShape();
		stars.beginShape(UShape.VertexType.POINTS);
		randomSeed(0);
		for (int i = 0; i < 500; i++) {
			stroke (random(255), random(255), random(255));
			stars.vertex(randomUnitVec3() * random(rangeMin, rangeMax));
		}
		for (int i = 0; i < 2500; i++) {
            stroke(random(20, 120));
            stars.vertex(randomUnitVec3() * random(rangeMin, rangeMax));
        }
        stars.endShape();
    }

    protected override void Draw ()
    {
        blendMode(UMaterials.BlendMode.Add);
        drawStatic();
        //drawRealtime();
    }

	void drawStatic()
    {
        pushMatrix();
        translate(0, 3, -10);
        draw(stars);
        popMatrix();
    }

    void drawRealtime()
    {
		randomSeed(0);
		for (int i = 0; i < 500; i++) {
			stroke(random(255), random(255), random(255));
            point(randomUnitVec3() * random(rangeMin, rangeMax));
        }
        for (int i = 0; i < 2500; i++) {
            stroke(random(100, 255));
            point(randomUnitVec3() * random(rangeMin, rangeMax));
        }
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
