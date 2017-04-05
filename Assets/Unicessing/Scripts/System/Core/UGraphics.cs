using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Unicessing
{
    /** @class UGraphics

        @lang_en --------
        Base class for drawing by Unicessing
        @end_lang

        @lang_jp --------
        Unicessingによる描画の基本クラス
        @end_lang
    */
    public class UGraphics : UResource
    {
        #region Settings
        [SerializeField] private float DefaultDepthStep = 0.003f;
        public float width = 1.0f;
        public float height = 1.0f;
        #endregion

        #region System Variables

        public class UStyle
        {
            public Color fillColor;
            public Color strokeColor;
            public ColorMode colorMode;
            public Vector4 colorScale;
            public Vector4 invColorScale;
            public int layer;
            public int layerMask;
            public float depth;
            public float depthStep;
            public Texture texture;
            public float textSize;
            public float textScale;
            public float textQuality;
            public int textAlignX;
            public int textAlignY;
            public int rectMode;
            public int ellipseMode;
            public int imageMode;
            public int sphereDetailKey;
            public const int sphereDetailKeyDefault = (30 << 16) | 30;
            public Font font;
            public UMaterials.BlendMode blendMode;
            public bool isLighting;
            public bool isStroke;
            public bool isFill;

            public UStyle(UGraphics g) { init(g); }

            public UStyle Clone() { return (UStyle)MemberwiseClone(); }

            public void init(UGraphics g)
            {
                fillColor = Color.white;
                strokeColor = Color.white;
                colorMode = ColorMode.RGB;
                const float inv255 = 1.0f / 255.0f;
                colorScale = new Vector4(255, 255, 255, 255);
                invColorScale = new Vector4(inv255, inv255, inv255, inv255);
                layer = (g != null) ? g.gameObject.layer : -1;
                layerMask = 1 << layer;
                depth = 0.0f;
                depthStep = (g != null) ? g.DefaultDepthStep : 0.003f;
                texture = null;
                textSize = 13;
                textScale = 1.0f;
                textQuality = 1.0f;
                textAlignX = LEFT;
                textAlignY = BASELINE;
                rectMode = CORNER;
                ellipseMode = CENTER;
                imageMode = CORNER;
                sphereDetailKey = sphereDetailKeyDefault;
                font = null;
                blendMode = UMaterials.BlendMode.Opaque;
                isLighting = true;
                isStroke = false;
                isFill = true;
            }
        }

        private bool isLoop = true;
        private bool isRotateRadians = true;
        private bool isSphereRadius = true;

        protected struct SystemObject
        {
            public Transform transform;
            public Vector3 axis;
            public Vector3 translateScale;
            public UStyle style;
			public UShape pointShape;
			public UShape lineShape;
            public UShape curveShape;
            public UShape customShape;
            public GameObject imageBox;
            public GameObject objBox;
        }
        protected SystemObject system;
        private Stack<UTransform> matrixStack = new Stack<UTransform>();
        private Stack<UStyle> styleStack = new Stack<UStyle>();
        private Stack<int> layerStack = new Stack<int>();
        private Stack<int> layerMaskStack = new Stack<int>();
        private List<GameObject> tempGameObjs = new List<GameObject>();
        private List<GameObject> keepGameObjs = new List<GameObject>();

        class BasicShapes
        {
            public UShape point = null;
            public UShape rect = null;
            public UShape ellipse = null;
            public UShape box = null;
            public UShape sphere = null;
            public Dictionary<int, UShape> detailSpheres = null;
            public GameObject text = null;
        }
        private BasicShapes basicShapes = new BasicShapes();

        private List<UShape> shapes = new List<UShape>();

        #endregion

        #region System Functions

        protected virtual void Awake()
        {
            InitMath();
            InitInput();
            InitGraphics();
        }

        protected virtual void Start()
        {
            Setup();
        }

        void InitGraphics()
        {
			UMaterials.Init();
            InitCamera();

            system.style = new UStyle(this);
            GameObject sysTransObj = new GameObject("SystemTrans");
            sysTransObj.transform.SetParent(gameObject.transform);
            system.transform = sysTransObj.transform;
            system.axis = Vector3.one;
            system.imageBox = new GameObject("ImageBox");
            system.imageBox.transform.SetParent(gameObject.transform);
            system.objBox = new GameObject(gameObject.name + " Objects");
            system.pointShape = new UShape(this);
            system.lineShape = new UShape(this);
            system.curveShape = new UShape(this);
            system.customShape = null;

            basicShapes.point = createPoint(0, 0, 0);
            basicShapes.rect = createRect(0, 0, 1, 1);
            basicShapes.ellipse = createEllipse(0, 0, 1, 1);
            basicShapes.box = createBox(1, 1, 1);
            basicShapes.sphere = createSphere(1, 1, 1);
            basicShapes.detailSpheres = new Dictionary<int, UShape>();
            basicShapes.text = loadPrefab("Unicessing/Prefabs/Text");
        }

        void RecreateBasicFillShapes()
        {
            basicShapes.rect = createRect(0, 0, 1, 1);
            basicShapes.ellipse = createEllipse(0, 0, 1, 1);
            basicShapes.box = createBox(1, 1, 1);
            basicShapes.sphere = createSphere(1, 1, 1);
            basicShapes.detailSpheres.Clear();
        }

        private void UpdateTranslateScale()
        {
            var ts = transform.localScale;
            system.translateScale.x = ts.x < 0.0f ? -1 : 1;
            system.translateScale.y = ts.y < 0.0f ? -1 : 1;
            system.translateScale.z = ts.z < 0.0f ? -1 : 1;
        }

        private void PreDraw()
        {
            clear();

            UpdateTranslateScale();
            noDepthStep();
            UpdateInput();
            BeginSystemShapes();
            push();
            depthStep();
        }

        protected override Vector3 OnUpdateMouse3D(Vector3 pos)
        {
            return isP5 ? new Vector3(pos.x + width * 0.5f, pos.y + height * 0.5f, pos.z) : pos;
        }

        private void PostDraw()
        {
            DrawShapes();
            pop();
            EndSystemShapes();
        }

        private void DrawShapes() { foreach (UShape shape in shapes) draw(shape); }

        private void FlushSystemShapes()
        {
            EndSystemShapes();
            BeginSystemShapes();
        }

        private void DrawSubGraphics()
        {
            foreach (USubGraphics sg in subGraphics) sg.SetupDraw();
        }

        private void BeginSystemShapes(UShape.VertexType vertexType = UShape.VertexType.NONE)
        {
            system.pointShape.beginShape(UShape.VertexType.POINTS);
            system.lineShape.beginShape(UShape.VertexType.LINES);
            system.curveShape.beginShape(UShape.VertexType.CURVE_LINES);
        }

        private void EndSystemShapes(UShape.VertexType vertexType = UShape.VertexType.NONE)
        {
            if (system.pointShape != null)
            {
                system.pointShape.endShape();
                drawShape(system.pointShape, Matrix4x4.identity);
            }
            if (system.lineShape != null)
            {
                system.lineShape.endShape();
                drawShape(system.lineShape, Matrix4x4.identity);
            }
            if (system.curveShape != null)
            {
                system.curveShape.endShape();
                drawShape(system.curveShape, Matrix4x4.identity);
            }
        }

        protected virtual void Update()
        {
            if (!isLoop) return;
            PreDraw();
            push();
            Draw();
            pop();
            DrawSubGraphics();
            PostDraw();
        }

#if false // UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (!targetCamera) return;
            Transform trans = (mousePlane) ? mousePlane : transform;
            Gizmos.matrix = trans.localToWorldMatrix;
        }
#endif

        private Vector2 GetModePos(int mode, float x, float y, float w, float h)
        {
            switch (mode)
            {
                case CORNER: return new Vector2(x, y);
                case CORNER_P5: return new Vector2(x, y - h);
                case CENTER: default: return new Vector2(x - (w * 0.5f), y - (h * 0.5f));
            }
        }
        private float rotVal(float angle) { return isRotateRadians ? degrees(angle) : angle; } // for Unicessing

        #endregion

        #region Processing Events
        protected virtual void Setup() { }
        protected virtual void Draw() { }
        #endregion

        #region Processing Members

        /// @i{Specify width and height sizes\, coordinate system mode\, overall scale,widthとheightの大きさ、座標系モード、全体のスケールを指定}
        /**
          @lang_en --------
          Unicessing uses Camera in the scene, so you do not need to use size ().

          @param width The value to return when referencing width. It does not affect the camera
          @param height The value to return when referencing height. It does not affect the camera
          @param mode The default is U3D. When it is set to P2D or P3D, it becomes the coordinate system of processing (y and z are opposite to each other). P2D starts with noLights()
          @param scale Scale value for various objects to draw with Unicessing
          @sa AxisMode
          @end_lang

          @lang_jp --------
          UnicessingではCameraを自分でシーンに配置して使うため、size()を省略できる。

          @param width widthを参照したときに返したい値。カメラには影響しない
          @param height heightを参照したときに返したい値。カメラには影響しない
          @param mode デフォルトはU3D。P2D、P3DにするとProcessingの座標系(yとzが逆方向）になる。P2DはnoLights()でスタートする
          @param scale Unicessingで描画する各種オブジェクトにかかるスケール値
          @sa AxisMode
          @end_lang
        */
        public void size(float width, float height, AxisMode mode = U3D, float scale = 1.0f)
        {
            this.width = width;
            this.height = height;
            if (mode == P2D || mode == P3D)
            {
                axis(scale, -scale, -scale);
                translate(-width / 2, -height / 2);
                if(mode == P2D) { noLights(); }
            }
            else { axis(scale, scale, scale); }
        }

        /// @i{start Draw() loop,描画ループを許可}
        public void loop() { isLoop = true; }

        /// @i{stop Draw() loop,描画ループを止める}
        public void noLoop() { isLoop = false; }

        /// @i{delete temporary objects,一時オブジェクトを消す}
        public void clear()
        {
            foreach (GameObject obj in keepGameObjs) { obj.SetActive(false); }
            foreach (GameObject obj in tempGameObjs) { Destroy(obj); }
            tempGameObjs.Clear();
        }

        /// @i{Changes the way Unicessing interprets color data. mode : Either RGB or HSB\, corresponding to Red/Green/Blue and Hue/Saturation/Brightness,Unicessingでの色の解釈modeをRGB、HSBで切り替える。値の範囲のスケールも可能。}
        public void colorMode(ColorMode mode, float max = 255) { colorMode(mode, max, max, max, max); }
        public void colorMode(ColorMode mode, float maxR, float maxG, float maxB, float maxA)
        {
            system.style.colorMode = mode;
            system.style.colorScale = new Vector4(maxR, maxG, maxB, maxA);
            system.style.invColorScale = new Vector4(1 / maxR, 1 / maxG, 1 / maxB, 1 / maxA);
        }

        /// @i{Convert color according to color space mode,色空間モードにあわせて色を変換する}
        protected override Color convertColorSpace(float r, float g, float b, float a)
        {
            r = r * system.style.invColorScale.x;
            g = g * system.style.invColorScale.y;
            b = b * system.style.invColorScale.z;
            a = a * system.style.invColorScale.w;
            if (system.style.colorMode == ColorMode.HSB)
            {
                Color col = Color.HSVToRGB(r, g, b);
                col.a = a;
                return col;
            }
            else { return new Color(r, g, b, a); }
        }
        /// @i{Extracts the red value from a color,色の赤色成分を返す}
        public float red(Color col) { return col.r * system.style.colorScale.x; }
        /// @i{Extracts the green value from a color,色の緑色成分を返す}
        public float green(Color col) { return col.g * system.style.colorScale.y; }
        /// @i{Extracts the blue value from a color,色の青色成分を返す}
        public float blue(Color col) { return col.b * system.style.colorScale.z; }
        /// @i{Extracts the alpha value from a color,色の透明色成分を返す}
        public float alpha(Color col) { return col.a * system.style.colorScale.w; }
        /// @i{Extracts the gray scale value from a color,色のグレイスケール成分を返す}
        public float gray(Color col) { return col.grayscale * system.style.colorScale.w; }
        /// @i{Extracts the hue value from a color,色の色相を返す}
        public float hue(Color col)
        {
            float h = 0, s = 0, v = 0;
            Color.RGBToHSV(col, out h, out s, out v);
            return h * system.style.colorScale.x;
        }
        /// @i{Extracts the saturation value from a color,色の彩度を返す}
        public float saturation(Color col)
        {
            float h = 0, s = 0, v = 0;
            Color.RGBToHSV(col, out h, out s, out v);
            return s * system.style.colorScale.y;
        }
        /// @i{Extracts the brightness value from a color,色の明度を返す}
        public float brightness(Color col)
        {
            float h = 0, s = 0, v = 0;
            Color.RGBToHSV(col, out h, out s, out v);
            return v * system.style.colorScale.z;
        }
        /// @i{Calculates a color or colors between two color at a specific increment,２つの色の補間した色を返す}
        public Color lerpColor(Color c1, Color c2, float amt) { return Color.Lerp(c1, c2, amt); }

        /// @i{Enable fill,塗りつぶす}
        public void fill() { system.style.isFill = true; }

        /// @i{Sets the color used to fill shapes,塗りつぶし色を指定}
        public void fill(float gray, float alpha = 255) { fill(gray, gray, gray, alpha); }
        public void fill(float r, float g, float b, float a = 255) { fill(color(r, g, b, a)); }
        public void fill(Color col) { system.style.fillColor = col; system.style.isFill = true; }

        /// @i{Disable fill,塗りつぶさない}
        public void noFill() { system.style.isFill = false; }

        /// @i{Enable stroke,線を描く}
        public void stroke() { system.style.isStroke = true; }

        /// @i{Sets the color used to draw lines and borders around shape\, wireframes,線の色を指定}
        public void stroke(float gray, float alpha = 255) { stroke(gray, gray, gray, alpha); }
        public void stroke(float r, float g, float b, float a = 255) { stroke(color(r, g, b, a)); }
        public void stroke(Color col) { system.style.strokeColor = col; system.style.isStroke = true; }

        /// @i{Disable stroke,線を描かない}
        public void noStroke() { system.style.isStroke = false; }

        /// @i{Sets a texture to be applied to vertex points,塗りつぶしにテクスチャーを使うように指定}
        public void texture(UImage img) { system.style.texture = img.texture; }
        public void texture(Texture tex) { system.style.texture = tex; }

        /// @i{Disable texture,塗りつぶしにテクスチャーを使わない}
        public void noTexture() { system.style.texture = null; }

        /// @i{Blends the pixels in the display window according to a defined mode OPAQUE\, TRANSPARENT\, ADD,合成モードを指定。デフォルトは不透明のOPAQUEで、半透明のTRANSPARENT、加算のADDなどがある}
        public void blendMode(UMaterials.BlendMode mode) { system.style.blendMode = mode; }

        /// @i{The pushStyle() function saves the current style settings and popStyle() restores the prior settings,現在のスタイルをスタックに積んで保存}
        public void pushStyle() { styleStack.Push(system.style.Clone()); }

        /// @i{The pushStyle() function saves the current style settings and popStyle() restores the prior settings restores the prior settings,保存したスタイルをスタックから降ろして反映}
        public void popStyle() { system.style = styleStack.Pop(); }

        /// @i{Get style,現在のスタイルクラスを直接参照する}
        public UStyle getStyle() { return system.style; }

        /// @i{Set style,現在のスタイルクラスを変更する}
        public void setStyle(UStyle style) { this.style(style); }

        public void style(UStyle style)
        {
            Debug.Assert(style != null);
            int layer = system.style.layer;
            int layerMask = system.style.layerMask;
            bool isFlush = (style.layer != layer || style.layerMask != layerMask);
            system.style = style.Clone();
            if (style.layer < 0)
            {
                this.layer(layer);
                this.layerMask(layerMask);
            }
            if (isFlush) { FlushSystemShapes(); }
        }

        /// @i{Pushes the current transformation matrix onto the matrix stack,現在の行列（姿勢、位置、スケール）をスタックに積んで保存}
        public void pushMatrix()
		{
            matrixStack.Push(new UTransform(system.transform));
        }

        /// @i{Pops the current transformation matrix off the matrix stack,現在の行列（姿勢、位置、スケール）をスタックから降ろして反映}
		public void popMatrix()
		{
			if (matrixStack.Count > 0)
			{
				system.transform.Set(matrixStack.Pop());
			}
		}

        /// @i{pushStyle() and pushMatrix(),現在のスタイルと行列（姿勢、位置、スケール）をスタックに積んで保存}
        public void push() { pushStyle(); pushMatrix(); }
        /// @i{popStyle() and popMatrix(),現在のスタイルと行列（姿勢、位置、スケール）をスタックから降ろして反映}
        public void pop() { popMatrix(); popStyle(); }


        /// @i{Get the current local matrix,現在のローカル行列（姿勢、位置、スケール）を取得する}
        public UMatrix getMatrix() { return new UMatrix(system.transform.worldToLocalMatrix); }
        /// @i{Get the current world matrix,現在のワールド行列（姿勢、位置、スケール）を取得する}
        public Matrix4x4 getWorldMatrix() { return system.transform.localToWorldMatrix; }
        /// @i{Get the current local matrix,現在のローカル行列（姿勢、位置、スケール）を取得する}
        public Matrix4x4 getLocalMatrix() { return system.transform.worldToLocalMatrix; }

        /// @i{Overwrite the current local matrix,現在のローカル行列（姿勢、位置、スケール）を上書きする}
        public void applyMatrix(UMatrix matrix) { system.transform.setLocal(matrix); }
        /// @i{Overwrite the current world matrix,現在のワールド行列（姿勢、位置、スケール）を上書きする}
        public void applyWorldMatrix(Matrix4x4 matrix) { system.transform.setWorld(new UMatrix(matrix)); }
        /// @i{Overwrite the current local matrix,現在のローカル行列（姿勢、位置、スケール）を上書きする}
        public void applyLocalMatrix(Matrix4x4 matrix) { system.transform.setLocal(new UMatrix(matrix)); }

        //public void translateNoScale(float x, float y, float z = 0.0f) { translateNoScale(new Vector3(x, y, z)); }
        //public void translateNoScale(Vector3 pos) { system.transform.Translate(pos); }

        /// @i{Move position,位置を移動する}
        public void translate(float x, float y, float z = 0.0f) { translate(new Vector3(x, y, z)); }
        public void translate(Vector3 pos)
        {
            var s = system.transform.lossyScale;
            pos.x *= s.x * system.translateScale.x;
            pos.y *= s.y * system.translateScale.y;
            pos.z *= s.z * system.translateScale.z;
            system.transform.Translate(pos, Space.Self);
        }

        /// @i{Rotate orientation (Z),姿勢を回転させる(Z軸)}
        public void rotate(float angle) { rotateZ(angle); }
        /// @i{Rotate orientation (3 axis),姿勢を回転させる(3軸)}
        public void rotate(float xAngle, float yAngle, float zAngle) { system.transform.Rotate(new Vector3(rotVal(xAngle), rotVal(yAngle), rotVal(zAngle))); }
        public void rotate(Vector3 angles) { rotate(angles.x, angles.y, angles.z); }
        /// @i{Rotate orientation (any axis),姿勢を回転させる(指定の軸)}
        public void rotate(float angle, float x, float y, float z) { system.transform.RotateAround(Vector3.zero, new Vector3(x, y, z), rotVal(angle)); }
        /// @i{Rotate orientation (X),姿勢を回転させる(X軸)}
        public void rotateX(float angle) { system.transform.Rotate(rotVal(angle), 0.0f, 0.0f); }
        /// @i{Rotate orientation (Y),姿勢を回転させる(Y軸)}
        public void rotateY(float angle) { system.transform.Rotate(0.0f, rotVal(angle), 0.0f); }
        /// @i{Rotate orientation (Z),姿勢を回転させる(Z軸)}
        public void rotateZ(float angle) { system.transform.Rotate(0.0f, 0.0f, rotVal(angle)); }
        /// @i{Rotate orientation (Regardless of the current rotateMode\, rotate the 3 axis Euler angles in degrees),姿勢を回転させる(現在のrotateModeに関係なく、3軸オイラー角を度単位で回転)}
        /// @i{Scale the size,大きさをスケール倍する)}
        public void scale(float s) { scale(s, s, s); }
        public void scale(float x, float y, float z = 1.0f)
        {
            Vector3 s = system.transform.localScale;
            s.x *= x; s.y *= y; s.z *= z;
            system.transform.localScale = s;
        }
        public void scale(Vector3 s) { scale(s.x, s.y, s.z); }
        /// @i{Return the X coordinate of the current world position,現在ワールド位置のX座標を返す}
        public float modelX() { return system.transform.position.x; }
        /// @i{Return the Y coordinate of the current world position,現在ワールド位置のY座標を返す}
        public float modelY() { return system.transform.position.y; }
        /// @i{Return the Z coordinate of the current world position,現在ワールド位置のZ座標を返す}
        public float modelZ() { return system.transform.position.z; }

        /// @i{The createShape() function is used to define a new shape,新しいシェイプを作成する}
        public UShape createShape() { return new UShape(this); }
        /// @i{The createShape() function is used to define a new basic shape,基本形状のシェイプを作成する}
        /**
          @lang_en --------
          Create the basic shape specified by kind.

          @param kind Either POINT, LINE, RECT, ELLIPSE, BOX, SPHERE
          @param p ScaleVertex position of shape
          @sa UShape.ShapeKind 
          @end_lang

          @lang_jp --------
          kindで指定した基本形状のシェイプを作成する。

          @param kind UShape.ShapeKindのPOINT、LINE、RECT、ELLIPSE、BOX、SPHEREいずれかのシェイプ種別
          @param p シェイプの頂点位置
          @sa UShape.ShapeKind
          @end_lang
        */
        public UShape createShape(UShape.ShapeKind kind, params float[] p)
        {
            UShape shape = new UShape(this);
            switch (kind)
            {
			case UShape.ShapeKind.POINT:
				shape.createPoint(p[0], p[1], p[2]);
				break;
			case UShape.ShapeKind.LINE:
				shape.createLine(p[0], p[1], p[2], p[3], p[4], p[5]);
				break;
            case UShape.ShapeKind.RECT:
                shape.createRect(p[0], p[1], p[2], p[3]);
                break;
            case UShape.ShapeKind.ELLIPSE:
                shape.createEllipse(p[0], p[1], p[2], p[3]);
                break;
            case UShape.ShapeKind.BOX:
                shape.createBox(p[0], p[1], p[2]);
                break;
            case UShape.ShapeKind.SPHERE:
                shape.createSphere(p[0], p[1], p[2], (int)p[3], (int)p[4]);
                break;
	        }
            return shape;
        }
        /// @i{Create a point shape,点のシェイプを作成する}
		public UShape createPoint(float x, float y, float z) { return createShape(UShape.ShapeKind.POINT, x, y, z); }
        /// @i{Create a line shape,線のシェイプを作成する}
		public UShape createLine(float x1, float y1, float x2, float y2) { return createShape(UShape.ShapeKind.LINE, x1, y1, 0, x2, y2, 0); }
		public UShape createLine(float x1, float y1, float z1, float x2, float y2, float z2) { return createShape(UShape.ShapeKind.LINE, x1, y1, z1, x2, y2, z2); }
        /// @i{Create a rect shape,四角のシェイプを作成する}
		public UShape createRect(float x, float y, float w, float h) { return createShape(UShape.ShapeKind.RECT, x, y, w, h); }
        /// @i{Create a ellipse shape,円のシェイプを作成する}
        public UShape createEllipse(float x, float y, float w, float h) { return createShape(UShape.ShapeKind.ELLIPSE, x, y, w, h); }
        /// @i{Create a box shape,箱のシェイプを作成する}
        public UShape createBox(float w, float h, float d) { return createShape(UShape.ShapeKind.BOX, w, h, d); }
        /// @i{Create a sphere shape,球のシェイプを作成する}
        public UShape createSphere(float w, float h, float d, int ures = 30, int vres = 30) { return createShape(UShape.ShapeKind.SPHERE, w, h, d, ures, vres); }

        /// @i{Draw a point,点を描く}
        public void point(float x, float y, float z = 0.0f) { point(new Vector3(x, y, z)); }

        public void point(Vector3 pos)
		{
			#if true
			Vector3 v = system.transform.TransformPoint(pos);
			system.pointShape.vertex(v.x, v.y, v.z);
            if (system.pointShape.vertexCount >= 1024 * 60)
            {
                system.pointShape.endShape();
                drawShape(system.pointShape, Matrix4x4.identity);
                system.pointShape.beginShape(UShape.VertexType.POINTS);
            }
            #else
			draw(basicShapes.point, pos.x, pos.y, pos.z);
            #endif
        }
        /// @i{Draw a line,線を描く}
        public void line(float x1, float y1, float x2, float y2) { line(x1, y1, 0, x2, y2, 0); }
		public void line(float x1, float y1, float z1, float x2, float y2, float z2) { line(new Vector3(x1, y1, z1), new Vector3(x2, y2, z2));  }
        public void line(Vector3 v1, Vector3 v2)
        {
            #if true
            v1 = system.transform.TransformPoint(v1);
            v2 = system.transform.TransformPoint(v2);
            system.lineShape.vertex(v1.x, v1.y, v1.z);
			system.lineShape.vertex(v2.x, v2.y, v2.z);
            if(system.lineShape.vertexCount >= 1024 * 60)
            {
                system.lineShape.endShape();
                drawShape(system.lineShape, Matrix4x4.identity);
                system.lineShape.beginShape(UShape.VertexType.LINES);
            }
            #else
			UShape shape = new UShape(this);
			shape.createLine(v1.x, v1.y, v1.z, v2.x, v2.y, v2.z);
			draw(shape);
            #endif
        }
        /// @i{Draw a curve,曲線を描く}
        public void curve(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            curve(new Vector3(x1, y1, 0.0f), new Vector3(x2, y2, 0.0f), new Vector3(x3, y3, 0.0f), new Vector3(x4, y4, 0.0f));
        }
        public void curve(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3, float x4, float y4, float z4)
        {
            curve(new Vector3(x1, y1, z1), new Vector3(x2, y2, z2), new Vector3(x3, y3, z3), new Vector3(x4, y4, z4));
        }
        public void curve(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            v1 = system.transform.TransformPoint(v1);
            v2 = system.transform.TransformPoint(v2);
            v3 = system.transform.TransformPoint(v3);
            v4 = system.transform.TransformPoint(v4);
            system.curveShape.vertex(v1);
            system.curveShape.vertex(v2);
            system.curveShape.vertex(v3);
            system.curveShape.vertex(v4);
            if (system.curveShape.vertexCount >= 1024 * (60 / system.curveShape.curveDivision))
            {
                system.curveShape.endShape();
                drawShape(system.curveShape, Matrix4x4.identity);
                system.curveShape.beginShape(UShape.VertexType.CURVE_LINES);
            }
        }

        /// @i{Modifies the location from which rectangles are drawn by changing the way in which parameters given to rect() are intepreted. CORNER\, CENTER\, CORNER_P5,rect()で四角形を描くときの基準位置を設定する。CORNER、CENTER、CORNER_P5}
        public void rectMode(int mode) // CORNER or CENTER
        {
            Debug.Assert(mode == CORNER || mode == CENTER || mode == CORNER_P5);
            system.style.rectMode = mode;
        }

        /// @i{Draw a rect,四角を描く}
        public void rect(float x, float y, float w, float h)
        {
            Vector2 pos = GetModePos(system.style.rectMode, x, y, w, h);
            draw(basicShapes.rect, pos.x, pos.y, w, h);
        }

        /// @i{Modifies the location from which rectangles are drawn by changing the way in which parameters given to ellipse() are intepreted. CORNER\, CENTER\, CORNER_P5,ellipse()で円を描くときの基準位置を設定する。CORNER、CENTER、CORNER_P5}
        public void ellipseMode(int mode) // CORNER or CENTER
        {
            Debug.Assert(mode == CORNER || mode == CENTER || mode == CORNER_P5);
            system.style.ellipseMode = mode;
        }

        /// @i{Draw a ellipse,円を描く}
        public void ellipse(float x, float y, float w, float h)
        {
            Vector2 pos = GetModePos(system.style.ellipseMode, x + w * 0.5f, y + h * 0.5f, w, h);
            draw(basicShapes.ellipse, pos.x, pos.y, w, h);
        }

        /// @i{Draw a triangle,三角を描く}
        public void triangle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            beginShape(UShape.VertexType.TRIANGLES);
            vertex(x1, y1);
            vertex(x2, y2);
            vertex(x3, y3);
            endShape();
        }

        /// @i{Draw a box,箱を描く}
        public void box(float whd) { box(whd, whd, whd); }
        public void box(float w, float h, float d) { box(0.0f, 0.0f, 0.0f, w, h, d); }
        public void box(float x, float y, float z, float w, float h, float d) { draw(basicShapes.box, x, y, z, w, h, d); }
        public void box(Vector3 pos, Vector3 scale) { box(pos.x, pos.y, pos.z, scale.x, scale.y, scale.z); }
        public void box(Vector3 scale) { box(0.0f, 0.0f, 0.0f, scale.x, scale.y, scale.z); }
        /// @i{Draw a sphere,球を描く}
        public void sphere(float r) { sphere(r, r, r); }
        public void sphere(float w, float h, float d) { sphere(0.0f, 0.0f, 0.0f, w, h, d); }
        public void sphere(float x, float y, float z, float w, float h, float d)
        {
            if(isSphereRadius) { w *= 2.0f; h *= 2.0f; d *= 2.0f; }
            UShape shape = basicShapes.sphere;
            if (system.style.sphereDetailKey != UStyle.sphereDetailKeyDefault)
            {
                if (basicShapes.detailSpheres.ContainsKey(system.style.sphereDetailKey))
                {
                    shape = basicShapes.detailSpheres[system.style.sphereDetailKey];
                }
            }
            draw(shape, x, y, z, w, h, d);
        }
        public void sphere(Vector3 pos, Vector3 scale) { sphere(pos.x, pos.y, pos.z, scale.x, scale.y, scale.z); }
        public void sphere(Vector3 scale) { sphere(0.0f, 0.0f, 0.0f, scale.x, scale.y, scale.z); }
        /// @i{Set the number of divisions of the surface of the sphere when drawing with sphere(),sphere()で描くときの球の面の分割数を設定する}
        public void sphereDetail(int res) { sphereDetail(res, res); }
        public void sphereDetail(int ures, int vres)
        {
            int key = (ures << 16) | vres;
            if (!basicShapes.detailSpheres.ContainsKey(key))
            {
                basicShapes.detailSpheres[key] = createSphere(1.0f, 1.0f, 1.0f, ures, vres);
            }
            system.style.sphereDetailKey = key;
        }

        /// @i{Start designating the customized shape with beginShape() and draw it with endShape(),独自のシェイプをbeginShape()で作成開始し、endShape()で描く}
        /**
          @lang_en --------
          Between beginShape() and endShape(), you can specify a vertex using vertex() or curveVertex() and draw a customized shape.

          @param type Either POINTS, LINES, TRIANGLES, TRIANGLE_FAN, TRIANGLE_STRIP, QUADS, or QUAD_STRIP
          @sa UShape.VertexType
          @end_lang

          @lang_jp --------
          beginShape()からendShape()の間に、vertex()またはcurveVertex()を使って頂点を指定し、独自のシェイプを描ける。

          @param kind UShape.ShapeKindのPOINT、LINE、RECT、ELLIPSE、BOX、SPHEREいずれかのシェイプ種別
          @sa UShape.ShapeKind
          @end_lang
        */
        public void beginShape(UShape.VertexType type = UShape.VertexType.LINE_STRIP)
		{
			system.customShape = new UShape(this);
			system.customShape.beginShape(type);
		}
        /// @i{The endShape() function is the companion to beginShape() and may only be called after beginShape(). Close shape with CLOSE,endShape()はbeginShape()のあとに使い、その間で指定した頂点の図形を描く。CLOSEで図形を最初の頂点につなげて閉じる}
		public void endShape(UShape.CloseType closeType = UShape.CloseType.NONE)
		{
			if (system.customShape==null) { debuglogError("endShape: <Not beginShape>"); return; }
			system.customShape.endShape(closeType);
			draw(system.customShape);
			system.customShape = null;
		}
        public void endShape(int closeType) { endShape((UShape.CloseType)closeType); }
        public void endShapeClose() { endShape(UShape.CloseType.CLOSE); }

        /// @i{Add a vertex. Use between beginShape() and endShape(),頂点を追加する。beginShape()とendShape()の間で使うこと}
        public void vertex(float x, float y, float z = 0.0f) { vertex(x, y, z, 0.0f, 0.0f); }
		public void vertex(float x, float y, float u, float v) { vertex(x, y, 0.0f, u, v); }
		public void vertex(float x, float y, float z, float u, float v) { vertex(new Vector3(x, y, z), new Vector2(u, v)); }
        public void vertex(Vector3 pos) { vertex(pos, Vector2.zero); }
        public void vertex(Vector3 pos, Vector3 uv)
        {
            if (system.customShape == null) { debuglogError("vertex: <Not beginShape>"); return; }
            system.customShape.vertex(pos, uv);
        }

        /// @i{Add a vertex. Use between beginShape() and endShape(). Available only when CURVE_LINES\, CURVE_LINE_STRIP,曲線の頂点を追加する。beginShape()とendShape()の間で使うこと。CURVE_LINES、CURVE_LINE_STRIPのときのみ使用可能}
        public void curveVertex(float x, float y, float z = 0.0f) { curveVertex(new Vector3(x, y, z)); }
        public void curveVertex(Vector3 pos)
        {
            if (system.customShape == null) { debuglogError("curveVertex: <Not beginShape>"); return; }
            system.customShape.curveVertex(pos);
        }

        /// @i{Draw a shape,シェイプを描く}
		public void draw(UShape shape, float x, float y) { draw(shape, x, y, 0.0f); }
		public void draw(UShape shape, float x, float y, float w, float h) { draw(shape, x, y, 0.0f, w, h, 1.0f); }
        public void draw(UShape shape, float x, float y, float z)
        {
			pushMatrix();
			translate(x, y, z);
            drawShape(shape);
			popMatrix();
        }
        public void draw(UShape shape, float x, float y, float z, float w, float h, float d)
        {
            pushMatrix();
            translate(x, y, z);
            scale(w, h, d);
            drawShape(shape);
            popMatrix();
        }
        public void draw(UShape shape)
		{
			pushMatrix();
            drawShape(shape);
			popMatrix();
		}

		private void drawShape(UShape shape)
		{
			if (shape == null) return;

            applyDepth();

        #if UNITY_EDITOR
            Camera drawCam = null; // targetCamera; シーンビューに出なくなるのでひとまずnull
        #else
            Camera drawCam = targetCamera;
        #endif

            shape.draw(system.transform.localToWorldMatrix, drawCam);

            if (shape.isFillShape && shape.is2DShape) addDepthStep();
        }

        private void drawShape(UShape shape, Matrix4x4 matrix)
        {
            if (shape == null) return;

#if UNITY_EDITOR
            Camera drawCam = null; // targetCamera; シーンビューに出なくなるのでひとまずnull
#else
            Camera drawCam = targetCamera;
#endif
            shape.draw(matrix, drawCam);

            if (shape.isFillShape && shape.is2DShape) addDepthStep();
        }

        /*
        public void drawNow(UShape shape)
        {
            if (shape == null) return;
            shape.drawNow(system.transform.localToWorldMatrix, shape.nowColor);
        }
        */

        /// @i{Make image area,画像領域を作る}
        public UImage createImage(float width, float height, TextureFormat texFormat = TextureFormat.ARGB32)
        {
            return createImage((int)width, (int)height, texFormat);
        }
        public UImage createImage(int width, int height, TextureFormat texFormat = TextureFormat.ARGB32)
        {
            return createImage(system.imageBox, width, height, texFormat);
        }
        public UImage createImage(GameObject obj, int width, int height, TextureFormat texFormat = TextureFormat.ARGB32)
        {
            UImage img = obj.AddComponent<UImage>();
            img.create(width, height, texFormat);
            img.gameObject.SetActive(false);
            return img;
        }

        /// @i{Load image. Resources folder\, local file\, URL can be specified,画像を読み込む。リソースフォルダ内、ローカルファイル、URLを指定可能}
        public UImage loadImage(string path, int width = 0, int height = 0)
        {
            return loadImage(system.imageBox, path, width, height);
        }

        public UImage loadImage(GameObject obj, string path, int width = 0, int height = 0)
        {
            UImage img = obj.AddComponent<UImage>();
            img.width = width;
            img.height = height;
            img.load(this, path);
            return img;
        }

        /// @i{Remove image,画像を破棄する}
        public void removeImage(UImage img)
        {
            Debug.Assert(img);
            Destroy(img);
        }

        /// @i{Modifies the location from which images are drawn by changing the way in which parameters given to image() are intepreted. CORNER\, CENTER\, CORNER_P5,image()で画像を描くときの基準位置を設定する。CORNER、CENTER、CORNER_P5}
        public void imageMode(int mode) // CORNER or CENTER
        {
            Debug.Assert(mode == CORNER || mode == CENTER || mode == CORNER_P5);
            system.style.imageMode = mode;
        }

        /// @i{Draw a image,画像を描く}
        public void image(Texture tex, float x, float y) { assert(tex != null); image(tex, x, y, tex.width * 0.01f, tex.height * 0.01f); }
        public void image(UImage img, float x, float y) { assert(img != null); image(img, x, y, img.width * 0.01f, img.height * 0.01f); }
        public void image(UImage img, float x, float y, float w, float h) { assert(img != null); image(img.texture, x, y, w, h); }
        public void image(Texture tex, float x, float y, float w, float h)
        {
            //Debug.Assert(tex != null);
            Texture ptex = system.style.texture;
            system.style.texture = tex;
            Vector2 pos = GetModePos(system.style.imageMode, x, y, w, h);
            draw(basicShapes.rect, pos.x, pos.y, w, h);
            system.style.texture = ptex;
        }

        /// @i{Sets the current text size,テキストの大きさを設定する}
        public void textSize(float size) { system.style.textSize = size; }

        /// @i{Sets the current text size,テキストの縦横の基準位置を設定する}
        /**
          @lang_en --------
          @param alignX Either LEFT, CENTER, RIGHT
          @param alignY Either BASELINE, TOP, CENTER, BOTTOM
          @end_lang

          @lang_jp --------
          @param alignX 横の基準位置 LEFT, CENTER, RIGHT
          @param alignY 縦の基準位置 BASELINE, TOP, CENTER, BOTTOM
          @end_lang
        */
        public void textAlign(int alignX, int alignY = BASELINE)
        {
            Debug.Assert(alignX == LEFT || alignX == CENTER || alignX == RIGHT);
            Debug.Assert(alignY == TOP || alignY == CENTER || alignY == BOTTOM || alignY == BASELINE);
            system.style.textAlignX = alignX; // LEFT, CENTER, RIGHT
            system.style.textAlignY = alignY; // TOP, CENTER, BTOTTOM, BASELINE
        }

        /// @i{Load font. If it is not in the resource folder\, search for the same name font from the system font of the OS,フォントを読み込む。リソースフォルダにない場合、OSのシステムフォントから同名フォントを探す}
        public Font loadFont(string fontname)
        {
            string fontPath = getResourceName(fontname);
            Font font = Resources.Load(fontPath, typeof(Font)) as Font;
            if (font == null)
            {
                font = Font.CreateDynamicFontFromOSFont(fontname, (int)(system.style.textScale));
                if (font == null)
                {
                    debuglogWaring("loadFont <NotFound> " + fontPath);
                }
            }
            else
            {
                debuglog("loadFont " + fontPath);
            }
            return font;
        }

        /// @i{Return a list of system font names,OSのシステムフォント名一覧を取得する}
        public string[] fontList()
        {
            string[] fonts = Font.GetOSInstalledFontNames();
            string s = "";
            for (int i=0; i<fonts.Length; i++) { s += fonts[i]; }
            println(s);
            return fonts;
        }

        /// @i{Sets the current font that will be drawn with the text() function,テキスト描画に使うフォントを設定する}
        public void textFont(Font font) { system.style.font = font; }
        public void textFont(string fontname) { textFont(loadFont(fontname)); }

        /// @i{Create a text shape,テキストのシェイプを作成する}
        public GameObject createText(string str, float x, float y, float z = 0.0f)
        {
            GameObject obj = createObj(basicShapes.text);
            createText(obj, str, x, y, z);
            return obj;
        }

        private void createText(GameObject obj, string str, float x, float y, float z = 0.0f)
        {
            if (!basicShapes.text) return;

            TextMesh tm = obj.GetComponent<TextMesh>();
            if (tm)
            {
                Debug.Assert(system.style.textSize > 0);

                float textQuality = 100.0f * system.style.textScale;
                if (textQuality < 1.0f) textQuality = 1.0f;
                tm.color = system.style.fillColor;
                tm.characterSize = system.style.textSize * 10 / textQuality;
                tm.fontSize = (int)(textQuality);
                if (system.style.font != null)
                {
                    tm.font = system.style.font;
                    obj.GetComponent<Renderer>().material = system.style.font.material;
                }
                tm.text = str;
                tm.anchor = TextAnchor.MiddleCenter;
                if (system.style.textAlignX == LEFT)
                {
                    switch (system.style.textAlignY)
                    {
                        case TOP: tm.anchor = TextAnchor.UpperLeft; break;
                        case CENTER: tm.anchor = TextAnchor.MiddleLeft; break;
                        case BASELINE: tm.anchor = TextAnchor.MiddleLeft; break;
                        case BOTTOM: tm.anchor = TextAnchor.LowerLeft; break;
                    }
                }
                else if (system.style.textAlignX == RIGHT)
                {
                    switch (system.style.textAlignY)
                    {
                        case TOP: tm.anchor = TextAnchor.UpperRight; break;
                        case CENTER: tm.anchor = TextAnchor.MiddleRight; break;
                        case BASELINE: tm.anchor = TextAnchor.MiddleRight; break;
                        case BOTTOM: tm.anchor = TextAnchor.LowerRight; break;
                    }
                }
                else
                {
                    switch (system.style.textAlignY)
                    {
                        case TOP: tm.anchor = TextAnchor.UpperCenter; break;
                        case CENTER: tm.anchor = TextAnchor.MiddleCenter; break;
                        case BASELINE: tm.anchor = TextAnchor.MiddleCenter; break;
                        case BOTTOM: tm.anchor = TextAnchor.LowerCenter; break;
                    }
                }
                if (system.style.textAlignY == BASELINE)
                {
                    if (system.axis.y < 0) y -= system.style.textSize * 0.3f;
                    else y += system.style.textSize * 0.3f;
                }
            }

            Vector3 s = Vector3.one;
            if (system.axis.x < 0) { s.x = -1; }
            if (system.axis.y < 0) { s.y = -1; }

            SetTransformObj(obj, new Vector3(x, y, z), s);
        }

        /// @i{Draw a text,テキストを描く}
        public void text(string str, float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            GameObject obj = instantiateObj(basicShapes.text);
            if (obj) tempGameObjs.Add(obj);
            createText(obj, str, x, y, z);
        }

        /// @i{Do not use lights. unlit shader,ライトを使わないようにする（unlitシェーダー）}
        public void noLights() { system.style.isLighting = false; }

        /// @i{Use lights. normal shader. NOTE: Separately lights arrangement and setting are necessary,ライトを使うようにする（通常のシェーダー）※別途Lightの配置や設定は必要 }
        public void lights() { system.style.isLighting = true; }

        /// @i{Set ambient light (whole scene),環境光を設定する（シーン全体）}
        public void ambientLight(int r, int g, int b)
        {
            RenderSettings.ambientLight = color(r, g, b);
        }

        /// @i{Create a directional light,ディレクショナルライトを作る}
        public Light createDirectionalLight(int r, int g, int b, float nx, float ny, float nz)
        {
            return directionalLight(null, r, g, b, nx, ny, nz);
        }

        /// @i{Set the directional light. If lights is null\, add it to the scene newly,ディレクショナルライトを設定する。lightがnullの場合はシーンに新しく追加する}
        public Light directionalLight(Light light, int r, int g, int b, float nx, float ny, float nz)
        {
            if(!light)
            {
                GameObject obj = new GameObject("DirectionalLight");
                light = obj.AddComponent<Light>();
                obj.transform.SetParent(system.objBox.transform);
            }
            light.enabled = true;
            light.color = color(r, g, b);
            light.type = LightType.Directional;
            light.cullingMask = system.style.layerMask;
            light.gameObject.transform.localRotation = light.gameObject.transform.localRotation * Quaternion.LookRotation(new Vector3(nx, ny, nz));
            return light;
        }

        #endregion

        #region Processing Extra Members

        private void axisP5(float scale = 1.0f) { axis(scale, -scale, -scale); }
        private void axis(float x, float y, float z) { axis(new Vector3(x, y, z)); }
        private void axis(Vector3 axis)
        {
            bool isChangeY = (axis.y < 0 && system.axis.y > 0) || (axis.y > 0 && system.axis.y < 0);
            system.axis = axis;
            transform.localScale = axis;
            if(isChangeY) RecreateBasicFillShapes();
            UpdateTranslateScale();
        }
        /// @i{Return the axis and scale of the current coordinate system mode,現在の座標系モードの軸、スケール}
        public Vector2 axis2D { get { return new Vector2(system.axis.x, system.axis.y); } }
        public Vector3 axis3D { get { return system.axis; } }
        /// @i{Is it currently in Processing's coordinate system mode?,Processingの座標系モードか？}
        public bool isP5 { get { return system.axis.y < 0.0f; } }

        /// @i{Add shape to drawing list,描画リストにシェイプを追加}
        public void add(UShape shape) { shapes.Add(shape); }
        /// @i{Remove shape to drawing list,描画リストからシェイプを削除}
        public void remove(UShape shape) { shapes.Remove(shape); }

        /// @i{Set rotation mode of rotate() to radians,rotate()の回転モードをラジアン単位にする}
        public void rotateRadians() { isRotateRadians = true; }
        /// @i{Set rotation mode of rotate() to degrees,rotate()の回転モードを度単位にする}
        public void rotateDegrees() { isRotateRadians = false; }
        public void rotateDegrees(float xAngle, float yAngle, float zAngle) { system.transform.Rotate(new Vector3(xAngle, yAngle, zAngle)); }
        public void rotateDegrees(Vector3 eulerAngles) { system.transform.Rotate(eulerAngles); }
        /// @i{Interpret the value specified by sphere()\, where on is the radius and off is the diameter,sphere()で指定した値をonで半径、offで直径と解釈する}
        public void sphereRadius(bool on) { isSphereRadius = on; }

        /// @i{Sets the current font scale,テキストのスケールを設定する}
        public void textScale(float s) { system.style.textScale = s; }
        /// @i{Sets the current font quality,テキストの品質を設定する}
        public void textQuality(float level) { system.style.textQuality = level; }

        /// @i{The current transform in Unicessing,Unicessing内で座標変換した現在のTransform}
        public Transform uniTransfrom { get { return system.transform; } }
        /// @i{Returns the current transform in Unicessing,Unicessing内で座標変換した現在のTransformを返す}
        public Transform getTransform() { return system.transform; }
        /// @i{Returns world position,現在のワールド位置を返す（getWorldPos()の別名）}
        public Vector3 modelPos() { return getWorldPos(); }
        /// @i{Returns world position,ワールド座標系での位置を返す}
        public Vector3 getWorldPos() { return system.transform.position; }
        /// @i{Returns local position,ローカル座標系での位置を返す}
        public Vector3 getLocalPos() { return system.transform.localPosition; }
        /// @i{Returns Set world position,ワールド座標系で位置を設定する}
        public void setWorldPos(float x, float y, float z) { setWorldPos(new Vector3(x, y, z)); }
        public void setWorldPos(Vector3 pos) { system.transform.position = pos; }
        /// @i{Returns Set local position,ローカル座標系で位置を設定する}
        public void setLocalPos(float x, float y, float z) { setLocalPos(new Vector3(x, y, z)); }
        public void setLocalPos(Vector3 pos) { system.transform.localPosition = pos; }
        /// @i{Convert the position in the local coordinate system to the position in the world coordinate system,ローカル座標系での位置を、ワールド座標系での位置に変換する}
        public Vector3 localToWorldPos(float x, float y, float z = 0.0f) { return localToWorldPos(new Vector3(x, y, z)); }
        public Vector3 localToWorldPos(Vector3 localPos) { return system.transform.TransformPoint(localPos); }
        /// @i{Convert the position in the world coordinate system to the position in the local coordinate system,ワールド座標系での位置を、ローカル座標系での位置に変換する}
        public Vector3 wordlToLocalPos(float x, float y, float z = 0.0f) { return wordlToLocalPos(new Vector3(x, y, z)); }
        public Vector3 wordlToLocalPos(Vector3 worldPos) { return system.transform.InverseTransformPoint(worldPos); }
        /// @i{Convert the direction in the local coordinate system to the world coordinate system,ローカル座標系での方向をワールド座標系に変換する}
        public Vector3 localToWorldDir(Vector3 localDir) { return system.transform.TransformDirection(localDir); }
        /// @i{Convert the direction in the world coordinate system to the local coordinate system,ワールド座標系での方向をローカル座標系に変換する}
        public Vector3 worldToLocalDir(Vector3 woldDir) { return system.transform.InverseTransformDirection(woldDir); }
        /// @i{Returns Set world rotation,ワールド座標系で姿勢を設定する}
        public void setWorldRot(float xAngle, float yAngle, float zAngle) { setWorldRot(Quaternion.Euler(rotVal(xAngle), rotVal(yAngle), rotVal(zAngle))); }
        public void setWorldRot(Quaternion quat) { system.transform.localRotation = quat; }
        /// @i{Returns Set local rotation,ローカル座標系で姿勢を設定する}
        public void setLocalRot(float xAngle, float yAngle, float zAngle) { setLocalRot(Quaternion.Euler(rotVal(xAngle), rotVal(yAngle), rotVal(zAngle))); }
        public void setLocalRot(Quaternion quat) { system.transform.localRotation = quat; }
        /// @i{Returns SetLook at the camera,カメラの方を向かせる}
        public void lookAtCamera() { system.transform.LookAt(targetCamera.transform); system.transform.Rotate(0.0f, 180.0f, 0.0f); }
        /// @i{Returns SetLook at the camera,任意の点の方を向かせる}
        public void lookAt(float x, float y, float z) { lookAt(new Vector3(x, y, z)); }
        public void lookAt(Vector3 worldPosition) { lookAt(worldPosition, transform.up); }
        public void lookAt(Vector3 worldPosition, Vector3 worldUp) { system.transform.LookAt(worldPosition, worldUp); system.transform.Rotate(0.0f, 180.0f, 0.0f); }

        /// @i{Set specified layer and layer mask,指定のレイヤーとレイヤーマスクを設定}
        public void layerAndMask(int layerIndex) { layer(layerIndex); layerMask(1 << layerIndex); }
        public void layerAndMask(string layerName) { layer(layerName); layerMask(layerName); }
        /// @i{Set layer,レイヤーを設定}
        public void layer(int layerIndex)
        {
            if (system.style.layer != layerIndex) FlushSystemShapes();
            system.style.layer = layerIndex;
        }
        public void layer(string layerName) { layer(LayerMask.NameToLayer(layerName)); }
        /// @i{Begin setting layer. Restore with endLayer(),レイヤーを設定を開始。endLayer()で元に戻す}
        public void beginLayer(string layerName) { beginLayer(LayerMask.NameToLayer(layerName)); }
        public void beginLayer(int layerIndex)
        {
            layerStack.Push(system.style.layer);
            layerMaskStack.Push(system.style.layerMask);
            layer(layerIndex);
        }
        /// @i{Restore layer settings,レイヤーを設定を元に戻す}
        public void endLayer()
        {
            layerMask(layerMaskStack.Pop());
            layer(layerStack.Pop());
        }
        /// @i{Set layer mask,レイヤーマスクを設定}
        public void layerMask(int layerBits)
        {
            system.style.layerMask = layerBits;
            if (targetCamera) { targetCamera.cullingMask = system.style.layerMask; }
        }
        public void layerMask(string layerName) { layerMask(1 << LayerMask.NameToLayer(layerName)); }
        public void layerMaskEverything() { layerMask(-1); }
        /// @i{Set layer to add to layer mask,レイヤーマスクに追加するレイヤーを設定}
        public void addLayerMask(int layerIndex) { layer(system.style.layerMask | 1 << layerIndex); }
        public void addLayerMask(string layerName) { layer(system.style.layerMask | 1 << LayerMask.NameToLayer(layerName)); }
        /// @i{Set layer to remove to layer mask,レイヤーマスクから削除するレイヤーを設定}
        public void removeLayerMask(int layerIndex) { layer(system.style.layerMask & ~(1 << layerIndex)); }
        public void removeLayerMask(string layerName) { layer(system.style.layerMask & ~(1 << LayerMask.NameToLayer(layerName))); }

        /// @i{Enable scene lights,シーンのライトをEnableにする}
        public void sceneLights()
        {
            Light[] lights = GetComponentsInChildren<Light>();
            foreach (Light light in lights)
            {
                if (light.gameObject.layer == system.style.layer) { light.enabled = true; }
            }
            if (lights == null) createDirectionalLight(128, 128, 128, 0, 0, -1);
        }

        /// @i{Disable scene lights,シーンのライトをDisableにする}
        public void noSceneLights()
        {
            Light[] lights = GetComponentsInChildren<Light>();
            foreach (Light light in lights)
            {
                if (light.gameObject.layer == system.style.layer) { light.enabled = false; }
            }
        }

        /// @i{Restore DepthStep to default value,DepthStepをデフォルト値に戻す}
        public void depthStep() { depthStep(DefaultDepthStep); }
        /// @i{Set DepthStep,DepthStepを設定する}
        public void depthStep(float step) { system.style.depthStep = step; addDepthStep(); }
        /// @i{Set DepthStep to 0 so as not to automatically change the overlap depth of drawing,DepthStepを0にして、描画の重ねあわせの深さを自動的に変えないようにする}
        public void noDepthStep() { system.style.depthStep = 0.0f; }

        private void addDepthStep() { system.style.depth += system.style.depthStep; }
        private void applyDepth()
        {
            if (targetCamera != null)
            {
                Vector3 dir = targetCamera.transform.forward;
                float d = -system.style.depth * system.axis.z;
                Vector3 depth = dir * d;
                system.transform.Translate(depth);
            }
        }

        /// @i{Load prefab from Resources folder,プレハブをリソースフォルダから読み込む}
        public GameObject loadPrefab(string path)
        {
            GameObject prefabObj = Resources.Load(path) as GameObject;
            if (!prefabObj)
            {
                debuglogWaring("loadPrefab <NotFound> " + path);
                return null;
            }
            return prefabObj;
        }

        /// @i{Create (Instantiate) GameObject from prefab and deactivate once,プレハブからGameObjectを作成（Instantiate）し、いったん非アクティブにする}
        public GameObject createObj(GameObject prefabObj)
        {
            GameObject obj = instantiateObj(prefabObj);
            if (!obj)
            {
                debuglogError("createObj <Failed> " + prefabObj);
                return null;
            }
            obj.SetActive(false);
            if (obj) keepGameObjs.Add(obj);
            return obj;
        }

        /// @i{Activate and display the GameObject that was being createdObj(),createObj()していたGameObjectをアクティブにして表示する}
        public void dispObj(GameObject createdObj) { dispObj(createdObj, Vector3.zero, Vector3.one); }
        public void dispObj(GameObject createdObj, float x, float y, float z = 0.0f, float sx = 1.0f, float sy = 1.0f, float sz = 1.0f)
        {
            dispObj(createdObj, new Vector3(x, y, z), new Vector3(sx, sy, sz));
        }
        public void dispObj(GameObject createdObj, Vector3 pos, Vector3 scale)
        {
            Debug.Assert(createdObj);
            if (!createdObj.activeSelf /*&& keepGameObjs.Contains(createdObj)*/) // createObjectで作成したオブジェクトなら
            {
                createdObj.SetActive(true);
            }
            SetTransformObj(createdObj, pos, scale);
        }

        /// @i{Create and draw an object temporarily for this frame from prefab,プレハブからこのフレームだけ一時的にオブジェクトを作成して描く}
        public GameObject entryObj(GameObject prefabObj) { return entryObj(prefabObj, Vector3.zero, Vector3.one); }
        public GameObject entryObj(GameObject prefabObj, float x, float y, float z = 0.0f, float sx = 1.0f, float sy = 1.0f, float sz = 1.0f)
        {
            return entryObj(prefabObj, new Vector3(x, y, z), new Vector3(sx, sy, sz));
        }
        public GameObject entryObj(GameObject prefabObj, Vector3 pos, Vector3 scale)
        {
            Debug.Assert(prefabObj);

            GameObject obj = instantiateObj(prefabObj);
            if (obj) tempGameObjs.Add(obj);
            SetTransformObj(obj, pos, scale);
            return obj;
        }

        /// @i{Draw GameObject. Call dipsObj() if the object is inactive\, call entryObj() if it is active,GameObjectを描く。渡したオブジェクトが非アクティブならdipsObj()を、アクティブならentryObj()を呼ぶ}
        public GameObject draw(GameObject obj) { return draw(obj, Vector3.zero, Vector3.one); }
        public GameObject draw(GameObject obj, float x, float y, float z = 0.0f, float sx = 1.0f, float sy = 1.0f, float sz = 1.0f)
        {
            return draw(obj, new Vector3(x, y, z), new Vector3(sx, sy, sz));
        }
        public GameObject draw(GameObject obj, Vector3 pos, Vector3 scale)
        {
            Debug.Assert(obj);
            GameObject dispObj = null;
            if (!obj.activeSelf /*&& keepGameObjs.Contains(obj)*/) // createObjectで作成したオブジェクトなら
            {
                dispObj = obj;
                dispObj.SetActive(true);
            }
            else
            {
                dispObj = instantiateObj(obj);
                if (dispObj) tempGameObjs.Add(dispObj);
            }
            SetTransformObj(dispObj, pos, scale);
            return obj;
        }

        private GameObject instantiateObj(GameObject prefabObj)
        {
            if (!prefabObj)
            {
                debuglogWaring("prefab <Null Resource>");
                return null;
            }
            GameObject obj = null;
            if (!obj)
            {
                obj = Instantiate(prefabObj) as GameObject;
                if (!obj)
                {
                    debuglogWaring("prefab <Instantiate Failed>");
                    return null;
                }
                else
                {
                    obj.transform.SetParent(system.objBox.transform);
                }
            }
            return obj;
        }

        private void SetTransformObj(GameObject obj, Vector3 pos, Vector3 scale)
        {
            obj.transform.position = system.transform.TransformPoint(pos);
            obj.transform.localScale = Vector3.Scale(system.transform.lossyScale, scale);
            obj.transform.rotation = system.transform.rotation;
            obj.gameObject.layer = system.style.layer;
        }

        /// @i{Destroy GameObject,GameObjectを破棄する}
        public void destroyObj(GameObject obj)
        {
            if (keepGameObjs.Contains(obj))
            {
                keepGameObjs.Remove(obj);
            }
            else if (tempGameObjs.Contains(obj))
            {
                tempGameObjs.Remove(obj);
            }
            Destroy(obj);
        }

        /// @i{Draw a mesh,メッシュを描く}
        public void mesh(Mesh mesh, float x, float y, float z = 0.0f, float sx = 1.0f, float sy = 1.0f, float sz = 1.0f, bool enableMaterial = true)
        {
            this.mesh(mesh, new Vector3(x, y, z), new Vector3(sx, sy, sz), enableMaterial);
        }
        public void mesh(Mesh mesh, Vector3 pos, Vector3 scale, bool enableMaterial = true)
        {
            this.mesh(mesh, null, pos, scale, enableMaterial);
        }
        private void mesh(Mesh mesh, Mesh wireMesh, Vector3 pos, Vector3 scale, bool enableMaterial = true)
        {
            pushMatrix();
            translate(pos);
            system.transform.localScale = Vector3.Scale(system.transform.localScale, scale);

            Material material;
            if (system.style.isLighting) material = UMaterials.getFillMaterial(system.style.blendMode);
            else material = UMaterials.getFillUnlitMaterial(system.style.blendMode);

            MaterialPropertyBlock materialPB = null;
            if (enableMaterial)
            {
                materialPB = new MaterialPropertyBlock();
                materialPB.SetColor("_Color", system.style.fillColor);
                if (system.style.texture != null)
                {
                    materialPB.SetTexture("_MainTex", system.style.texture);
                }
            }

            Camera drawCam = null; // targetCamera; シーンビューに出なくなるのでひとまずnull
            Matrix4x4 matrix = system.transform.localToWorldMatrix;
            if (system.style.isFill)
            {
                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    Graphics.DrawMesh(mesh, matrix, material, system.style.layer, drawCam, i, materialPB);
                }
            }

            if (system.style.isStroke && wireMesh != null)
            {
                material = UMaterials.getFillUnlitMaterial(system.style.blendMode);
                if (enableMaterial)
                {
                    materialPB.SetColor("_Color", system.style.strokeColor);
                }
                for (int i = 0; i < wireMesh.subMeshCount; i++)
                {
                    Graphics.DrawMesh(wireMesh, matrix, material, system.style.layer, drawCam, i, materialPB);
                }
            }
            popMatrix();
        }

        /// @i{Draw a mesh held by GameObject,GameObjectが持つメッシュを描く}
        public void mesh(GameObject inMeshObj, float x, float y, float z = 0.0f, float sx = 1.0f, float sy = 1.0f, float sz = 1.0f, bool enableMaterial = true)
        {
            this.mesh(inMeshObj, new Vector3(x, y, z), new Vector3(sx, sy, sz), enableMaterial);
        }
        public void mesh(GameObject inMeshObj, Vector3 pos, Vector3 scale, bool enableMaterial = true)
        {
            Debug.Assert(inMeshObj);
            if (inMeshObj == null) { return; }

            MeshFilter meshFilter = inMeshObj.GetComponent<MeshFilter>();
            if (meshFilter)
            {
                Mesh m = meshFilter.sharedMesh;
                Mesh wm = null;
                if (system.style.isStroke)
                {
                    UWireframe wire = inMeshObj.GetComponent<UWireframe>();
                    if (!wire) { wire = inMeshObj.AddComponent<UWireframe>(); }
                    if (!wire.mesh) { wire.RemakeMesh(m); }
                    wm = wire.mesh;
                }
                if (m) { mesh(m, wm, pos, scale, enableMaterial); }
            }
        }

        /// @i{Draw a connected shape in the system,システムでまとめているシェイプを描き出す}
        public void flushSystemShapes() { FlushSystemShapes(); }

        #endregion

        #region USubGraphics

        private List<USubGraphics> subGraphics = new List<USubGraphics>();

        /// @i{Add USubGraphics to the list. NOTE: USubGraphics automatically add() at the time of Start()\, so you do not need to call it yourself,USubGraphicsをリストに追加する。※USubGraphicsはStart()時に自動的にこの処理を行うので、独自に呼ぶ必要はない}
        public void add(USubGraphics sg) { if (sg != null && !subGraphics.Contains(sg)) subGraphics.Add(sg); }
        /// @i{Remove USubGraphics from the list. NOTE: USubGraphics automatically remove() on OnDestroy()\, so you do not need to call it yourself,USubGraphicsをリストから削除する。※USubGraphicsはOnDestroy()時に自動的にこの処理を行うので、独自に呼ぶ必要はない}
        public void remove(USubGraphics sg) { subGraphics.Remove(sg); }

        /// @i{Create USubGraphics that can be drawn by sharing UGprahics,UGprahicsを共有して描画できるUSubGraphicsを作成する}
        public USubGraphics createSubGraphics(System.Action<UGraphics> customDraw = null)
        {
            return createSubGraphics<USubGraphics>(name, null, null, customDraw);
        }
        public USubGraphics createSubGraphics(string name = "SubGraphics", System.Action<GameObject> preInit = null, System.Action<USubGraphics> postInit = null, System.Action<UGraphics> customDraw = null)
        {
            return createSubGraphics<USubGraphics>(name, preInit, postInit, customDraw);
        }
        /// @i{Create USubGraphics that can be drawn by sharing UGprahics,UGprahicsを共有して描画できるUSubGraphicsを作成する}
        /**
          @lang_en --------
          @tparam T Subclass of USubGraphics or USubGraphics
          @param name Name of GameObject
          @param preInit Process to be done when creating GameObject
          @param postInit Process to be done after USubGraphics component creation
          @param customDraw Process to be done at drawing
          @end_lang

          @lang_jp --------
          @tparam T USubGraphicsまたはその派生クラス
          @param name GameObjectの名前
          @param preInit GameObject生成時に行いたい処理
          @param postInit USubGraphicsコンポーネント生成後に行いたい処理
          @param customDraw USubGraphics描画時に行いたい処理
          @end_lang

          @code
            // Unicessing/Scripts/Samples/UnicessingMenu.cs addButton()
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
          @endcode
          @sa USubGraphics
        */
        public T createSubGraphics<T>(string name = "SubGraphics", System.Action<GameObject> preInit = null, System.Action<T> postInit = null, System.Action<UGraphics> customDraw = null) where T : USubGraphics
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(transform);
            //obj.transform.SetParent(system.objBox.transform);
            if (preInit != null) preInit(obj);
            T subGraphics = obj.AddComponent<T>();
            subGraphics.g = this;
            if (postInit != null) postInit(subGraphics);
            if (customDraw != null) subGraphics.customDraw = customDraw;
            return subGraphics;
        }
        #endregion
    }

}