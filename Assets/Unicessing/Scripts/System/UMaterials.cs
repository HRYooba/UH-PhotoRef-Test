using UnityEngine;
using System.Collections;

namespace Unicessing
{
    public class UMaterials
    {
        public enum BlendMode
        {
            Opaque,
            Transparent,
            Add,
            _Max,
        }

        public static UMaterials instance;
        public static Material[] fillMaterials = new Material[(int)BlendMode._Max];
        public static Material[] fillUnlitMaterials = new Material[(int)BlendMode._Max];
        public static Material[] strokeMaterials = new Material[(int)BlendMode._Max];

        public static void Init()
        {
            if (instance != null) return;

            instance = new UMaterials();

            // rect、ellipseなどマテリアルカラー使用

#if UNITY_STANDALONE  
            // GPUインスタンシングあり
            fillMaterials[(int)BlendMode.Opaque] = instance.load("Unicessing/Materials/FillOpaqueInstanced");
            //fillMaterials[(int)BlendMode.Transparent] = instance.load("Unicessing/Materials/FillTransInstanced");
            fillMaterials[(int)BlendMode.Transparent] = instance.load("Unicessing/Materials/FillTrans");
            fillMaterials[(int)BlendMode.Add] = instance.load("Unicessing/Materials/FillAddInstanced");
#else
            fillMaterials[(int)BlendMode.Opaque] = instance.load("Unicessing/Materials/FillOpaque");
            fillMaterials[(int)BlendMode.Transparent] = instance.load("Unicessing/Materials/FillTrans");
            fillMaterials[(int)BlendMode.Add] = instance.load("Unicessing/Materials/FillAdd");
#endif

            fillUnlitMaterials[(int)BlendMode.Opaque] = instance.load("Unicessing/Materials/FillUnlitOpaque");
            fillUnlitMaterials[(int)BlendMode.Transparent] = instance.load("Unicessing/Materials/FillUnlitTrans");
            fillUnlitMaterials[(int)BlendMode.Add] = instance.load("Unicessing/Materials/FillUnlitAdd");

            // line、pointなど頂点カラー使用
            strokeMaterials[(int)BlendMode.Opaque] = instance.load("Unicessing/Materials/StrokeOpaque");
            strokeMaterials[(int)BlendMode.Transparent] = instance.load("Unicessing/Materials/StrokeTrans");
            strokeMaterials[(int)BlendMode.Add] = instance.load("Unicessing/Materials/StrokeAdd");
        }

        private Material load(string name)
        {
            Material resMat = Resources.Load<Material>(name);
            if (resMat) return new Material(resMat);

            Shader shader = Shader.Find(name);
            if (shader) return new Material(shader);

            return null;
        }

        public static Material getFillMaterial(BlendMode blendMode)
        {
            return fillMaterials[(int)blendMode];
        }
        public static Material getFillUnlitMaterial(BlendMode blendMode)
        {
            return fillUnlitMaterials[(int)blendMode];
        }

        public static Material getStrokeMaterial(BlendMode blendMode)
        {
            return strokeMaterials[(int)blendMode];
        }
    }
}