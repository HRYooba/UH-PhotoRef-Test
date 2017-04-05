using UnityEngine;
using System.Collections;

namespace Unicessing
{
    public class UCamera : UMath
    {
        public Camera targetCamera;

        public void InitCamera()
        {
            if (targetCamera == null) targetCamera = Camera.main;
        }

        public int displayWidth { get { return Screen.width; } }
        public int displayHeight { get { return Screen.height; } }
        public static float displayAspectW { get { return (float)Screen.width / Screen.height; } }
        public static float displayAspectH { get { return (float)Screen.height / Screen.width; } }

        public void background(float gray) { background(gray, gray, gray); }
        public void background(float r, float g, float b) { background(color(r, g, b)); }
        public void background(Color col)
        {
            if (!targetCamera) return;
            targetCamera.backgroundColor = col;
            targetCamera.clearFlags = CameraClearFlags.SolidColor;
        }
        public void backgroundFlags(CameraClearFlags flags)
        {
            if (!targetCamera) return;
            targetCamera.clearFlags = flags;
        }
        public void backgroundSkybox() { backgroundFlags(CameraClearFlags.Skybox); }

        public RaycastHit raycast(Vector3 origin, Vector3 direction, float distance = INFINITY, int layerMask = -1)
        {
            RaycastHit hitInfo = new RaycastHit();
            Physics.Raycast(origin, direction, out hitInfo, distance, layerMask);
            return hitInfo;
        }

        public RaycastHit2D raycast(Vector2 origin, Vector2 direction, float distance = INFINITY, int layerMask = -1)
        {
            return Physics2D.Raycast(origin, direction, distance, layerMask);
        }
    }
}
