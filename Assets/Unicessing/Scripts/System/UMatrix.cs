using UnityEngine;
using System.Collections;

namespace Unicessing
{
    static class UTransformExtensions
    {
        public static void Set(this Transform transform, UTransform utrans)
        {
            transform.localPosition = utrans.localPosition;
            transform.localRotation = utrans.localRotation;
            transform.localScale = utrans.localScale;
        }
    }

    static class UMatrixExtensions
    {
        public static void set(this Transform transform, UMatrix matrix)
        {
            setWorld(transform, matrix);
        }

        public static void setLocal(this Transform transform, UMatrix matrix)
        {
            transform.localScale = matrix.getScale();
            transform.localRotation = matrix.getRotation();
            transform.localPosition = matrix.getPosition();
        }

        public static void setWorld(this Transform transform, UMatrix matrix)
        {
            transform.localScale = matrix.getScale();
            transform.rotation = matrix.getRotation();
            transform.position = matrix.getPosition();
        }
    }

    public class UTransform
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;

        public UTransform(Transform source)
        {
            this.localPosition = source.localPosition;
            this.localRotation = source.localRotation;
            this.localScale = source.localScale;
        }

        public Matrix4x4 getMatrix() { return Matrix4x4.TRS(localPosition, localRotation, localScale); }
    }

    public class UMatrix
    {
        protected Matrix4x4 m;

        public UMatrix() { this.m = new Matrix4x4(); }
        public UMatrix(Transform source) { this.m = source.localToWorldMatrix; }
        public UMatrix(Matrix4x4 source) { this.m = source; }
        public UMatrix(UMatrix source) { this.m = source.m; }
        public override string ToString() { return m.ToString(); }

        #region Processing Members
        public float m00 { get { return m.m00; } set { m.m00 = value; } }
        public float m01 { get { return m.m01; } set { m.m01 = value; } }
        public float m02 { get { return m.m02; } set { m.m02 = value; } }
        public float m03 { get { return m.m03; } set { m.m03 = value; } }
        public float m10 { get { return m.m10; } set { m.m10 = value; } }
        public float m11 { get { return m.m11; } set { m.m11 = value; } }
        public float m12 { get { return m.m12; } set { m.m12 = value; } }
        public float m13 { get { return m.m13; } set { m.m13 = value; } }
        public float m20 { get { return m.m20; } set { m.m20 = value; } }
        public float m21 { get { return m.m21; } set { m.m21 = value; } }
        public float m22 { get { return m.m22; } set { m.m22 = value; } }
        public float m23 { get { return m.m23; } set { m.m23 = value; } }
        public float m30 { get { return m.m30; } set { m.m30 = value; } }
        public float m31 { get { return m.m31; } set { m.m31 = value; } }
        public float m32 { get { return m.m32; } set { m.m32 = value; } }
        public float m33 { get { return m.m33; } set { m.m33 = value; } }

        public UMatrix get() { return new UMatrix(this); }
        public void set(Matrix4x4 source) { m = source; }
        public void set(UMatrix source) { m = source.m; }
        public void applay(UMatrix source) { m *= source.m; }
        public void preApplay(UMatrix left) { set(left.m * m); }
        public void reset() { set(Matrix4x4.identity); }

        public bool invert()
        {
            var im = m.inverse;
            m = im;
            return true;
        }
        public void transpose() { m = m.transpose; }

        public Vector3 mult(Vector3 source, Vector3 target)
        {
            target = m.MultiplyPoint(source);
            return target;
        }

        public void scale(Vector3 v) { scale(v.x, v.y, v.z); }
        public void scale(float x, float y, float z = 1.0f)
        {
            m.m00 *= x; m.m01 *= x; m.m02 *= x;
            m.m10 *= y; m.m11 *= y; m.m12 *= y;
            m.m20 *= z; m.m21 *= z; m.m22 *= z;
        }
        #endregion

        #region Processing Extra Members
        public Vector3 mult(Vector3 source) { return m.MultiplyPoint(source); }

        public Quaternion getRotation()
        {
            //return Quaternion.LookRotation(m.GetColumn(2).normalized, m.GetColumn(1).normalized);

            var qw = Mathf.Sqrt(1f + m.m00 + m.m11 + m.m22) / 2;
            var w = 4 * qw;
            var qx = (m.m21 - m.m12) / w;
            var qy = (m.m02 - m.m20) / w;
            var qz = (m.m10 - m.m01) / w;
            return new Quaternion(qx, qy, qz, qw);
        }

        public Vector3 getPosition()
        {
            var x = m.m03;
            var y = m.m13;
            var z = m.m23;
            return new Vector3(x, y, z);
        }

        public Vector3 getScale()
        {
            return new Vector3(
                Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02),
                Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12),
                Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22) );
        }

        public void set3x3(UMatrix pm)
        {
            m.m00 = pm.m00; m.m01 = pm.m01; m.m02 = pm.m02;
            m.m10 = pm.m10; m.m11 = pm.m11; m.m12 = pm.m12;
            m.m20 = pm.m20; m.m21 = pm.m21; m.m22 = pm.m22;
        }
        #endregion
    }
}