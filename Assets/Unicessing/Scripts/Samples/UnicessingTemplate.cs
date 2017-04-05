using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingTemplate : UGraphics {

    /*
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start () {
        base.Start();	
	}

    protected override void Update ()
    {
        base.Update();
    }
    */

    protected override void Setup()
    {
    }

    protected override void Draw()
    {
        drawSample();
    }

    protected override void OnMousePressed() { }
    protected override void OnMouseReleased() { }
    protected override void OnMouseMoved() { }
    protected override void OnMouseDragged() { }
    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
    protected override void OnKeyTyped() { }

    void drawSample()
    {
        fill(0, 128, 255);
        rect(0, 0, 5, 10);

        fill(255, 0, 0);
        ellipse(0, -5, 5, 5);

        point(0, -2.5f);

        stroke(0, 255, 0);
        line(0, 0, mouse3D.x, mouse3D.y);
        noStroke();

        pushMatrix();
            fill(255);
            translate(mouse3D);
            sphere(1);
            translate(0, 0, -3);
            rotateY(frameSec * 2.0f);
            box(1, 2, 3);
        popMatrix();
    }
}
