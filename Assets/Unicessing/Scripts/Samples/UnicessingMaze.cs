using UnityEngine;
using System.Collections;
using Unicessing;

public class UnicessingMaze : UGraphics
{
    int[,] maze;
    const int X_MAX = 30;
    const int Z_MAX = 30;
    public float boxSize = 3.0f;
    public Transform player;
    int px = 0;
    int pz = 0;

    protected override void Setup()
    {
        createMaze();
        updatePlayerXZ();
    }

    protected override void Draw ()
    {
        drawMaze();
    }

    void createMaze()
    {
        randomSeed(0);
        maze = new int[Z_MAX, X_MAX];
        for (int z = 0; z < Z_MAX; z++)
        {
            for (int x = 0; x < X_MAX; x++)
            {
                maze[z, x] = (z == 0 || z == Z_MAX - 1 || x == 0 || x == X_MAX - 1) ? 1 : 0;
            }
        }

        for (int z = 2; z < Z_MAX - 2; z += 2)
        {
            for (int x = 2; x < X_MAX - 2; x += 2)
            {
                maze[z, x] = 1;
                int rmin = (z==2) ? 0 : 1;
                int rmax = (maze[z, x - 1] > 0) ? 3 : 4;
                int dir = random(rmin, rmax);
                switch (dir)
                {
                    case 0: maze[z - 1, x] = 1; break; // UP
                    case 1: maze[z + 1, x] = 1; break; // DOWN
                    case 2: maze[z, x + 1] = 1; break; // RIGHT
                    case 3: maze[z, x - 1] = 1; break; // LEFT
                }
            }
        }
    }

    void drawMaze()
    {
        pushMatrix();

        // Maze Roof & Floor
        noStroke();
        fill(0, 30, 0);
        box(0, -boxSize * 1.02f, 0, X_MAX * boxSize, boxSize, Z_MAX * boxSize);
        box(0, boxSize * 1.02f, 0, X_MAX * boxSize, boxSize, Z_MAX * boxSize);

        // Maze
        translate(X_MAX / 2 * -boxSize, 0, Z_MAX / 2 * -boxSize);
        float fillSize = boxSize - 0.01f;
        for (int x = 0; x < X_MAX; x++)
        {
            for (int z = 0; z < Z_MAX; z++)
            {
                if (maze[z, x] != 0)
                {
                    stroke(0, 255, 0);
                    noFill();
                    box(x * boxSize, 0, z * boxSize, boxSize, boxSize, boxSize);

                    noStroke();
                    fill(0, 30, 0);
                    box(x * boxSize, 0, z * boxSize, fillSize, fillSize, fillSize);
                }
            }
        }

        // Player
        fill(255, 0, 0);
        sphere(px * boxSize, boxSize, pz * boxSize, boxSize * 0.3f, boxSize * 0.3f, boxSize * 0.3f);

        popMatrix();
    }

    void updatePlayerXZ()
    {
        px = (int)((player.position.x + boxSize * X_MAX / 2 + boxSize * 0.5f) / boxSize);
        pz = (int)((player.position.z + boxSize * Z_MAX / 2 + boxSize * 0.5f) / boxSize);
    }

    protected override void OnKeyTyped()
    {
        if (!player || !targetCamera) return;

        if (isKeyDown(KeyCode.LeftArrow))
        {
            player.Rotate(0, -90, 0);
        }
        else if (isKeyDown(KeyCode.RightArrow))
        {
            player.Rotate(0, 90, 0);
        }

        Vector3 forward = targetCamera.transform.forward;
        forward.y = 0.0f;
        if (abs(forward.x) > abs(forward.z))
        {
            forward.x = forward.x < 0 ? -1 : 1;
            forward.z = 0.0f;
        }
        else
        {
            forward.x = 0.0f;
            forward.z = forward.z < 0 ? -1 : 1;
        }

        Vector3 oldPos = player.position;
        if (isKeyDown(KeyCode.UpArrow))
        {
            player.position = oldPos + forward * boxSize;
        }
        else if (isKeyDown(KeyCode.DownArrow))
        {
            player.position = oldPos - forward * boxSize;
        }

        updatePlayerXZ();
        if(maze[pz, px] > 0)
        {
            player.position = oldPos;
            updatePlayerXZ();
        }
    }

    protected override void OnKeyPressed()
    {
        if (isKeyDown(KeyCode.Return) || isKeyDown(KeyCode.Backspace)) loadScene("Unicessing/Scenes/Menu");
    }
}
