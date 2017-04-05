using UnityEngine;
using System.Collections;

namespace Unicessing
{
    public class UInput : UCamera
    {
        private bool isReadyInput = false;
        protected void InitInput()
        {
            isReadyInput = true;
        }

        #region Processing Events
        protected virtual void OnMousePressed() { }
        protected virtual void OnMouseReleased() { }
        protected virtual void OnMouseMoved() { }
        protected virtual void OnMouseDragged() { }
        protected virtual void OnKeyPressed() { }
        protected virtual void OnKeyTyped() { }
        #endregion

        public bool mousePressed { get { return Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2); } }
        public int mouseButton
        {
            get
            {
                if (Input.GetMouseButton(0)) return LEFT;
                else if (Input.GetMouseButton(1)) return RIGHT;
                else if (Input.GetMouseButton(2)) return MIDDLE;
                else return NONE;
            }
        }
        public Transform mousePlane;
        public Vector3 mouse3D { get { return _mouse3D; } }
        private Vector3 _mouse3D = new Vector3();
        public Vector3 pmouse3D { get { return _pmouse3D; } }
        private Vector3 _pmouse3D = new Vector3();

        public int cursorX { get { return (int)Input.mousePosition.x; } }
        public int cursorY { get { return (int)Input.mousePosition.y; } }
        public float mouseX { get { return _mouse3D.x; } }
        public float mouseY { get { return _mouse3D.y; } }
        public float mouseZ { get { return _mouse3D.z; } }
        public float pmouseX { get { return _pmouse3D.x; } }
        public float pmouseY { get { return _pmouse3D.y; } }
        public float pmouseZ { get { return _pmouse3D.z; } }
        public bool keyPressed { get { return Input.anyKey; } }

        public float moveMouseX { get { return mouseX - pmouseX; } }
        public float moveMouseY { get { return mouseY - pmouseY; } }
        public float dragMouseX { get { return endDragMouseX - beginDragMouseX; } }
        public float dragMouseY { get { return endDragMouseY - beginDragMouseY; } }
        public float beginDragMouseX { get; private set; }
        public float beginDragMouseY { get; private set; }
        public float endDragMouseX { get; private set; }
        public float endDragMouseY { get; private set; }
        public bool mouseReleased { get { return mouseButtonUp != NONE; } }
        public int mouseButtonUp
        {
            get
            {
                if (!isReadyInput) { return NONE; }
                else if (Input.GetMouseButtonUp(0)) return LEFT;
                else if (Input.GetMouseButtonUp(1)) return RIGHT;
                else if (Input.GetMouseButtonUp(2)) return MIDDLE;
                else return NONE;
            }
        }
        public int mouseButtonDown
        {
            get
            {
                if (!isReadyInput) { return NONE; }
                else if (Input.GetMouseButtonDown(0)) return LEFT;
                else if (Input.GetMouseButtonDown(1)) return RIGHT;
                else if (Input.GetMouseButtonDown(2)) return MIDDLE;
                else return NONE;
            }
        }

        public bool isKey(KeyCode keyCode) { return Input.GetKey(keyCode); }
        public bool isKeyDown(KeyCode keyCode) { return isReadyInput && Input.GetKeyDown(keyCode); }
        public bool isKeyUp(KeyCode keyCode) { return isReadyInput && Input.GetKeyUp(keyCode); }
        public bool isKey(string keyName) { return Input.GetKey(keyName); }
        public bool isKeyDown(string keyName) { return isReadyInput && Input.GetKeyDown(keyName); }
        public bool isKeyUp(string keyName) { return isReadyInput && Input.GetKeyUp(keyName); }

        public float inputAxis(string name) { return Input.GetAxis(name); }
        public float inputX { get { return Input.GetAxis("Horizontal"); } }
        public float inputY { get { return Input.GetAxis("Vertical"); } }

        public bool touchPressed { get { return mousePressed; } }
        public bool touchReleased { get { return mouseReleased; } }
        public bool touchDown { get { return mouseButtonDown != NONE; } }
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        public int touchCount { get { return mousePressed ? 1 : 0; } }
        public Vector2 touch(int index) { return new Vector2(mouseX, mouseY); }
#else
	public int touchCount { get { return Input.touchCount; } }
	public Vector2 touch(int index) {
		Touch t = Input.GetTouch(index);
		return new Vector2(pixelToScreenX(t.position.x), pixelToScreenY(t.position.y));
	}
#endif
        public string B_FIRE1 { get { return "Fire1"; } }
        public string B_FIRE2 { get { return "Fire2"; } }
        public string B_FIRE3 { get { return "Fire3"; } }
        public string B_JUMP { get { return "Jump"; } }
        public bool isButton(string name) { return Input.GetButton(name); }
        public bool isButtonDown(string name) { return Input.GetButtonDown(name); }
        public bool isButtonUp(string name) { return Input.GetButtonDown(name); }

        public RaycastHit raycastScreen(float distance = INFINITY, int layerMask = -1)
        {
            if(isVR && targetCamera)
            {
                return raycast(targetCamera.transform.position, targetCamera.transform.forward, distance, layerMask);
            }
            return raycastScreen(cursorX, cursorY, distance, layerMask);
        }

        public RaycastHit raycastScreen(float x, float y, float distance = INFINITY, int layerMask = -1)
        {
            if (!targetCamera) return new RaycastHit();
            Ray ray = targetCamera.ScreenPointToRay(new Vector3(x, y, Input.mousePosition.z));
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, distance, layerMask);
            return hitInfo;
        }

        public void updateMouse3D()
        {
            if (!targetCamera) return;

            Transform planeTrans = mousePlane ? mousePlane : transform;
            Vector3 planePos = planeTrans.position;
            Vector3 planeNormal = planeTrans.forward;

            _pmouse3D.Set(_mouse3D.x, _mouse3D.y, _mouse3D.z);

            Vector3 hitPos = planeMousePos(planePos, planeNormal);
            hitPos = transform.InverseTransformPoint(hitPos);

            hitPos = OnUpdateMouse3D(hitPos);

            _mouse3D.Set(hitPos.x, hitPos.y, hitPos.z);
        }

        protected virtual Vector3 OnUpdateMouse3D(Vector3 pos) { return pos; }

        public Vector3 planeMousePos(float x, float y, float z) { return planeMousePos(new Vector3(x, y, z)); }
        public Vector3 planeMousePos(Vector3 planePos)
        {
            Vector3 planeNormal = transform.forward;
            if (mousePlane)
            {
                planeNormal = mousePlane.forward;
            }
            return planeMousePos(planePos, planeNormal);
        }

        public Vector3 planeMousePos(Vector3 planePos, Vector3 planeNormal)
        {
            if (!targetCamera) return Vector3.zero;

            Vector3 hitPos = Vector3.zero;
            Ray ray;
            if (isVR)
            {
                ray = new Ray(targetCamera.transform.position, targetCamera.transform.forward);
            }
            else
            {
                ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            }

            if (!RaycastPlane(out hitPos, ray, planeNormal, planePos))
            {
                hitPos.Set(0, 0, INFINITY);
            }
            return hitPos;
        }

        public Vector3 planeHitPos(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planePos, Vector3 planeNormal)
        {
            Vector3 hitPos = Vector3.zero;
            Ray ray = new Ray(rayOrigin, rayDirection);
            if (!RaycastPlane(out hitPos, ray, planeNormal, planePos))
            {
                hitPos.Set(0, 0, INFINITY);
            }
            return hitPos;
        }

        private bool RaycastPlane(out Vector3 intersection, Ray ray, Vector3 planeNormal, Vector3 planePoint)
        {
            float dotDenomi = Vector3.Dot(ray.direction, planeNormal);
            if (dotDenomi != 0.0f)
            {
                float dotNum = Vector3.Dot((planePoint - ray.origin), planeNormal);
                float length = dotNum / dotDenomi;
                intersection = ray.origin + ray.direction * length;
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        protected virtual void UpdateInput()
        {
            updateMouse3D();
            if (mouseButtonDown != NONE)
            {
                endDragMouseX = beginDragMouseX = mouseX;
                endDragMouseY = beginDragMouseY = mouseY;
                OnMousePressed();
            }
            if (mouseReleased) { OnMouseReleased(); }
            if (abs(mouseX - pmouseX) > 0.001f || abs(mouseY - pmouseY) > 0.001f)
            {
                OnMouseMoved();
                if (mousePressed)
                {
                    endDragMouseX = mouseX;
                    endDragMouseY = mouseY;
                    OnMouseDragged();
                }
            }
            if (keyPressed)
            {
                OnKeyPressed();
                if (Input.anyKeyDown) { OnKeyTyped(); }
            }
        }
    }

}
