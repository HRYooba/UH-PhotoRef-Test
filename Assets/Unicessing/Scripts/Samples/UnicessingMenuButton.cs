using UnityEngine;
using System.Collections.Generic;
using Unicessing;

[RequireComponent(typeof(BoxCollider))]
public class UnicessingMenuButton : USubGraphics
{
    public UnityEngine.BoxCollider colider;

    public string buttonName = "Button";
    public Color buttonActiveColor = Color.red;
    public Color buttonColor = Color.blue;
    public Color fontColor = Color.white;
    public bool isHit = false;

    protected override void Setup()
    {
        colider = GetComponent<BoxCollider>();
        g.textAlign(UGraphics.CENTER, UGraphics.CENTER);
    }

    protected override void Draw()
    {
        if(isHit) g.fill(buttonActiveColor);
        else g.fill(buttonColor);

        g.translate(colider.center.x, colider.center.y, colider.center.z);
        g.rect(-colider.size.x * 0.5f, -colider.size.y * 0.5f, colider.size.x, colider.size.y);

        g.fill(fontColor);
        g.textSize(colider.size.y * 0.8f);
        g.text(buttonName, 0, 0);
    }

    public void OnClick()
    {
        g.loadScene("Unicessing/Scenes/" + buttonName);
    }
}
