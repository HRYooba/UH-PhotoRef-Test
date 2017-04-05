using UnityEngine;
using System.Collections.Generic;
using Unicessing;

public class UnicessingMenu : UGraphics
{
    List<UnicessingMenuButton> buttonList;
    public Color buttonColor = Color.blue;
    public Color fontColor = Color.white;

    protected override void Setup()
    {
        buttonList = new List<UnicessingMenuButton>();
        float x = -4.1f; float y = 4.5f;
        addButton("Curves", x, y); y--;
        addButton("Custom", x, y); y--;
        addButton("Earth", x, y); y--;
        addButton("Images", x, y); y--;
        addButton("Prefabs", x, y); y--;
        addButton("Primitives", x, y); y--;
        addButton("Pteridophyte", x, y); y--;
        addButton("P5_Snows", x, y); y--;
        x = 4.1f; y = 4.5f;
        addButton("Sea", x, y); y--;
        addButton("Stars", x, y); y--;
        addButton("Template", x, y); y--;
        addButton("Texts", x, y); y--;
        addButton("Tunnel", x, y); y--;
        addButton("ZenTexts", x, y); y--;
        addButton("Maze", x, y); y--;
        addButton("Runner", x, y); y--;

        textAlign(CENTER, CENTER);
        textFont("Times New Roman");
    }

    /*
    void addButtonBasic(string name, float x, float y)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = transform;
        
        BoxCollider collider = obj.AddComponent<BoxCollider>();
        collider.center = new Vector3(x, y, 0);
        collider.size = new Vector3(8, 0.9f, 0.1f);

        UnicessingMenuButton button = obj.AddComponent<UnicessingMenuButton>();
        button.g = this;
        button.buttonName = name;
        button.buttonColor = buttonColor;
        button.fontColor = fontColor;
        buttonList.Add(button);
    }
    */

    void addButton(string name, float x, float y)
    {
        createSubGraphics<UnicessingMenuButton>(name,
            obj => {
                BoxCollider collider = obj.AddComponent<BoxCollider>();
                collider.center = new Vector3(x, y, 0);
                collider.size = new Vector3(8, 0.9f, 0.1f);
            },
            button => {
                button.buttonName = name;
                button.buttonColor = buttonColor;
                button.fontColor = fontColor;
                buttonList.Add(button);
            }
        );
    }

    protected override void Draw()
    {
        fill(255);

        if(isVR)
        {
            // Eye Cursor
            pushMatrix();
            translate(0, 0, -0.1f);
            ellipse(mouseX, mouseY, 0.1f, 0.1f);
            popMatrix();
        }

        textSize(0.7f);
        text("Back to Menu : Enter or BS key", 0, -4.0f);
        textSize(0.4f);
        text("( Please add Unicessing/Scenes/* to the build settings. )", 0, -5.0f);

        hitTestButtons();
    }

    void hitTestButtons()
    {
        bool isClicked = mousePressed || isKeyDown(KeyCode.Return);
        RaycastHit hit = raycastScreen();
        foreach (UnicessingMenuButton button in buttonList)
        {
            button.isHit = (hit.collider == button.colider);
            if (button.isHit && isClicked) { button.OnClick(); }
        }
    }
}
