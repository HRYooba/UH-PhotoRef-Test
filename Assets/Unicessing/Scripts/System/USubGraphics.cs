using UnityEngine;
using System.Collections;

namespace Unicessing
{
    public class USubGraphics : MonoBehaviour
    {
        public UGraphics g;
        public System.Action<UGraphics> customDraw;
        public int customID = 0;
        public Object customObj = null;

        bool isSetuped = false;
        UGraphics.UStyle style;

        protected virtual void Start()
        {
            if (g) g.add(this);
            else Debug.LogError("g (UGraphics) is null.");
        }

        public void SetupDraw()
        {
            g.push();
            if (!isSetuped)
            {
                Setup();
                isSetuped = true;
                recordStyle();
            }
            applyStyle();
            g.applyWorldMatrix(transform.localToWorldMatrix);
            Draw();
            g.pop();
        }

        protected virtual void OnDestroy() { if (g) g.remove(this);}

        protected void recordStyle() { if (g) style = g.getStyle().Clone(); }
        protected void applyStyle() { if (g) g.setStyle(style); }

        protected virtual void Setup() { }

        protected virtual void Draw()
        {
            if (customDraw != null) customDraw(g);
        }
    }
}