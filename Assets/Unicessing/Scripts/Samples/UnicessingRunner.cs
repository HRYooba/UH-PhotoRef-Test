using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingRunner : UGraphics
{
    public Transform player;
    public Transform playerCamera;
    public Vector3 blockSize = new Vector3(3.0f, 0.5f, 3.0f);
    public int blockAreaX = 2;
    public int blockAreaZ = 20;

    enum BlockType
    {
        Normal, Step, Fall, Elevetor
    }

    protected override void Setup()
    {
        createStage();
    }

    protected override void Draw()
    {
        updatePlayer();
    }

    void createStage()
    {
        Vector3 pos = Vector3.zero;
        for (int z = -2; z <= blockAreaZ; z++)
        {
            for (int x = -blockAreaX; x <= blockAreaX; x++)
            {
                pos.x = blockSize.x * x;
                pos.y = blockSize.y * -0.5f;
                pos.z = blockSize.z * z;
                BlockType type = BlockType.Normal;
                if (z > 1 && (z + x % 3) != 0)
                {
                    float rnd = random(100) * map(z, -2, blockAreaZ, 0.5f, 1.5f);
                    if (rnd < 30) { type = BlockType.Normal; }
                    else if (rnd < 40) { continue; }
                    else if (rnd < 60) { createBlock(new Vector3(pos.x, pos.y + blockSize.y, +pos.z), blockSize, BlockType.Step); }
                    else if (rnd < 100) { type = BlockType.Fall; }
                    else if (rnd < 120) { type = BlockType.Elevetor; }
                    else { continue; }
                }
                createBlock(pos, blockSize, type);
            }
        }
    }

    void createBlock(Vector3 pos, Vector3 scale, BlockType type)
    {
        Color col = Color.white;
        float mass = 5.0f;
        switch(type)
        {
            case BlockType.Normal:
                col = Color.white;
                break;
            case BlockType.Step:
                mass = 3.0f;
                col = Color.black;
                break;
            case BlockType.Fall:
                mass = 0.2f;
                col = Color.red;
                break;
            case BlockType.Elevetor:
                mass = 100.0f;
                col = Color.green;
                scale *= 0.95f;
                break;
        }

        GameObject fixedJointObj = null;
        createSubGraphics("Box",
            obj => {    // GameObject Setup
                BoxCollider collider = obj.AddComponent<BoxCollider>();
                obj.transform.localPosition = pos;
                collider.size = scale;

                Rigidbody rb = null;
                if (type != BlockType.Normal)
                {
                    rb = obj.AddComponent<Rigidbody>();
                    rb.mass = mass;
                    rb.useGravity = false;
                }

                if (type == BlockType.Elevetor)
                {
                    fixedJointObj = new GameObject();
                    fixedJointObj.transform.SetParent(system.transform);
                    fixedJointObj.transform.localPosition = pos;
                    {
                        Rigidbody frb = fixedJointObj.AddComponent<Rigidbody>();
                        frb.useGravity = false;
                        frb.isKinematic = true;

                        FixedJoint fj = fixedJointObj.AddComponent<FixedJoint>();
                        fj.connectedBody = rb;
                    }
                }
            },
            sub => { }, // USubGraphics Setup
            g => {      // Draw
                float s = map((player.position - pos).magnitude - 20.0f, 0, 30.0f, 1.0f, 0.0f);
                s = constrain(s, 0.0f, 1.0f);
                if (s <= 0.01f) return;

                if (type == BlockType.Elevetor)
                {
                    fixedJointObj.transform.localPosition = pos + new Vector3(0, sin(pos.z + frameSec * 0.7f) * 3.0f, 0);
                    stroke(col);
                    noFill();
                }
                else
                {
                    stroke(Color.black);
                    fill(col);
                }
                float ms = 1 - s;
                rotateZ(radians(90.0f * ms));
                translate(-pos.x * ms, -10.0f * ms, 0);
                rotateX(radians(180.0f * (1 - s)));
                box(scale * s);
            }
        );
    }

    void updatePlayer()
    {
        if (!playerCamera || !player) return;

        if(player.position.y < -10.0f) player.position = new Vector3(0, 3, 0);

        playerCamera.transform.position = new Vector3(0, 0, player.position.z);
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
