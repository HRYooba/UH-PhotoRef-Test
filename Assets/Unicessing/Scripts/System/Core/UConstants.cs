#define DEBUG_uProcessing

using UnityEngine;
using System.Collections;

namespace Unicessing
{
    public class UConstants : MonoBehaviour
    {
        #region Processing Members
        public const int CORNER = -7;
        public const int CORNER_P5 = -6;
        public const int CENTER = -5;
        public const int TOP = -4;
        public const int BOTTOM = -3;
        public const int BASELINE = -2;
        public const int CODED = -1;
        public const int NONE = 0;
        public const int UP = 1;
        public const int DOWN = 2;
        public const int LEFT = 3;
        public const int RIGHT = 4;
        public const int MIDDLE = 5;
        public const int SHIFT = 10;
        public const int CTRL = 11;
        public const int BACKSPACE = 20;
        public const int TAB = 21;
        public const int ENTER = 22;
        public const int RETURN = ENTER;
        public const int ESC = 23;
        public const int DELETE = 24;
        public enum AxisMode { U3D, P2D, P3D };
        public const AxisMode U3D = AxisMode.U3D;
        public const AxisMode P2D = AxisMode.P2D;
        public const AxisMode P3D = AxisMode.P3D;
        public const UMaterials.BlendMode ADD = UMaterials.BlendMode.Add;
        public const UMaterials.BlendMode OPAQUE = UMaterials.BlendMode.Opaque;
        public const UMaterials.BlendMode TRANSPARENT = UMaterials.BlendMode.Transparent;
        public const UShape.VertexType POINTS = UShape.VertexType.POINTS;
        public const UShape.VertexType LINES = UShape.VertexType.LINES;
        public const UShape.VertexType LINE_STRIP = UShape.VertexType.LINE_STRIP;
        public const UShape.VertexType TRIANGLES = UShape.VertexType.TRIANGLES;
        public const UShape.VertexType TRIANGLE_FAN = UShape.VertexType.TRIANGLE_FAN;
        public const UShape.VertexType TRIANGLE_STRIP = UShape.VertexType.TRIANGLE_STRIP;
        public const UShape.VertexType QUADS = UShape.VertexType.QUADS;
        public const UShape.VertexType QUAD_STRIP = UShape.VertexType.QUAD_STRIP;
        public const UShape.CloseType CLOSE = UShape.CloseType.CLOSE;
        public enum ColorMode { RGB, HSB };
        public const ColorMode RGB = ColorMode.RGB;
        public const ColorMode HSB = ColorMode.HSB;

        private const float inv255 = 1.0f / 255.0f;
        protected virtual Color convertColorSpace(float r, float g, float b, float a) { return new Color(r * inv255, g * inv255, b * inv255, a * inv255); }

        public Color color(Color baseColor, float alpha = 255) { return new Color(baseColor.r, baseColor.g, baseColor.b, convertColorSpace(0, 0, 0, alpha).a); }
        public Color color(float gray, float alpha = 255) { return convertColorSpace(gray, gray, gray, alpha); }
        public Color color(float r, float g, float b, float a = 255) { return convertColorSpace(r, g, b, a); }

        public static void println(object message) { Debug.Log(message); }
        #endregion

        #region Processing Extra Members
        public static void debuglog(object message) { Debug.Log(message); }
        public static void debuglogWaring(object message) { Debug.LogWarning(message); }
        public static void debuglogError(object message) { Debug.LogError(message); }

        [System.Diagnostics.Conditional("DEBUG_uProcessing")]
        public static void assert(bool condition, string message = null)
        {
            if (!condition)
            {
                if (message == null) { message = "assertion failed"; }
                Debug.LogError(message);
            }
        }

        public bool editorDialog(string title, string message, string ok = "OK", string cancel = null)
        {
            bool result = false;
        #if UNITY_EDITOR
            result = UnityEditor.EditorUtility.DisplayDialog(title, message, ok, cancel);
        #endif
            return result;
        }

        public const float INFINITY = Mathf.Infinity;
        public static float deltaTime { get { return Time.deltaTime; } }

        public static bool isVR { get { return UnityEngine.VR.VRSettings.enabled; } }
        #endregion
    }

}
