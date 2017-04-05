using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unicessing;

public class UnicessingPrefabs : UGraphics
{
    GameObject capsulePrefab;
    GameObject cylinderPrefab;
    List<GameObject> cylinderObjs = new List<GameObject>();
    const int CylinderMax = 1000;

    public enum CylinderDrawMode
    {
        Created,
        Realtime,
        RealtimeMesh,
    }
    public CylinderDrawMode mode = CylinderDrawMode.RealtimeMesh;

    protected override void Setup()
    {
        capsulePrefab = loadPrefab("Unicessing/Prefabs/Capsule");
        cylinderPrefab = loadPrefab("Unicessing/Prefabs/Cylinder");
        for (int i = 0; i < CylinderMax; i++)
        {
            cylinderObjs.Add( createObj(cylinderPrefab) );
        }
    }

    protected override void Draw()
    {
        drawPrefabCapsules();
        switch(mode)
        {
            case CylinderDrawMode.Created:
                drawCreatedCylinders();
                break;
            case CylinderDrawMode.Realtime:
                drawRealtimeCylinders();
                break;
            case CylinderDrawMode.RealtimeMesh:
                drawRealtimeMeshCylinders();
                break;
        }
    }

    void drawPrefabCapsules()
    {
        pushMatrix();
        translate(0, 9, 0);
        float t = frameSec * 1.0f;
        float r = 1.0f;
        for (int i = 0; i < 150; i++)
        {
            rotateZ(0.2f);
            translate(r + cos(t) * 0.2f, 0, r * 0.2f);
            draw(capsulePrefab, 0, 0, sin(t));
            //entryObj(capsulePrefab, 0, 0, sin(t));
        }
        popMatrix();
    }

    void drawCreatedCylinders()
    {
        pushMatrix();
        int ti = (int)(frameSec * 60) % 1000;
        float r = 0.1f;
        for (int i = 0; i < cylinderObjs.Count && i < ti; i++)
        {
            rotateZ(r * 0.5f);
            translate(r * 7.0f, 0, r * 0.2f);
            draw(cylinderObjs[i], 0, 0, -r * 0.5f);
            //dispObj(cylinderObjs[i], 0, 0, -r * 0.5f);
        }
        popMatrix();
    }

    void drawRealtimeCylinders()
    {
        pushMatrix();
        int ti = (int)(frameSec * 60) % CylinderMax;
        float r = 0.1f;
        for (int i = 0; i < CylinderMax && i < ti; i++)
        {
            rotateZ(r * 0.5f);
            translate(r * 7.0f, 0, r * 0.2f);
            draw(cylinderPrefab, 0, 0, -r * 0.5f);
            //entryObj(cylinderPrefab, 0, 0, -r * 0.5f);
        }
        popMatrix();
    }

    void drawRealtimeMeshCylinders()
    {
        pushMatrix();
        int ti = (int)(frameSec * 60) % CylinderMax;
        float r = 0.1f;
        //Mesh cylinderMesh = cylinderPrefab.GetComponent<MeshFilter>().sharedMesh;
        for (int i = 0; i < CylinderMax && i < ti; i++)
        {
            fill(255, i % 255, 255);

            if (i % 10 == 0) { noFill(); stroke(0, 255, 255); }
            else { noStroke(); }

            rotateZ(r * 0.5f);
            translate(r * 7.0f, 0, r * 0.2f);
            mesh(cylinderPrefab, 0, 0, -r * 0.5f);
            //mesh(cylinderMesh, 0, 0, -r * 0.5f);
        }
        popMatrix();
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
