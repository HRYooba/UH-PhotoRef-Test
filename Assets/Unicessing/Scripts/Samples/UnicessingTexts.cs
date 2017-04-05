using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingTexts : UGraphics
{
    public string typedString = "";

    protected override void Setup ()
    {
        textFont("Times New Roman");
    }

    protected override void Draw ()
    {
        fill(255);
        textSize(1);
        textAlign(CENTER);
        text("<color=cyan>Hit any key.</color>", 0, 1);
        textSize(0.7f);
        text(typedString, 0, -1);
    }

    protected override void OnKeyTyped()
    {
        for(int i=(int)KeyCode.A; i <= (int)KeyCode.Z; i++)
        {
            if (isKeyDown((KeyCode)i))
            {
                int c = (isKey(KeyCode.LeftShift) || isKey(KeyCode.RightShift)) ? 'A' : 'a';
                typedString += ((char)(c + i - (int)KeyCode.A)).ToString();
            }
        }
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
