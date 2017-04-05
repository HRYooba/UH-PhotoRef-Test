#define DEBUG_uProcessing

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Unicessing
{
    public class UMath : UConstants
    {
        private long startTicks;

        protected void InitMath()
        {
            startTicks = DateTime.Now.Ticks;
        }

        #region Processing Members
        public int frameCount { get { return Time.frameCount; } }
        public void frameRate(int fps) { Application.targetFrameRate = fps; }
        public int day() { return DateTime.Now.Day; }
        public int hour() { return DateTime.Now.Hour; }
        public int millis() { return (int)((DateTime.Now.Ticks - startTicks) / 10000); }
        public int minute() { return DateTime.Now.Minute; }
        public int month() { return DateTime.Now.Month; }
        public int second() { return DateTime.Now.Second; }
        public int year() { return DateTime.Now.Year; }
        public float PI { get { return Mathf.PI; } }
        public float HALF_PI { get { return Mathf.PI * 0.5f; } }
        public float QUATER_PI { get { return Mathf.PI * 0.25f; } }
        public float TWO_PI { get { return Mathf.PI * 2.0f; } }
        public float DEG_TO_RAD { get { return Mathf.Deg2Rad; } }
        public float RAD_TO_DEG { get { return Mathf.Rad2Deg; } }
        public float abs(float f) { return Mathf.Abs(f); }
        public int ceil(float f) { return (int)Mathf.Ceil(f); }
        public float constrain(float amt, float low, float high) { return Mathf.Clamp(amt, low, high); }
        public float dist(float x1, float y1, float x2, float y2)
        {
            return mag(x1 - x2, y1 - y2);
        }
        public float dist(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return mag(x1 - x2, y1 - y2, z1 - z2);
        }
        public float exp(float f) { return Mathf.Exp(f); }
        public int floor(float f) { return (int)Mathf.Floor(f); }
        public float lerp(float start, float stop, float amt) { return Mathf.Lerp(start, stop, amt); }
        public float log(float f) { return Mathf.Log(f); }
        public static float mag(float x, float y) { return Mathf.Sqrt(x * x + y * y); }
        public static float mag(float x, float y, float z) { return Mathf.Sqrt(x * x + y * y + z * z); }
        public static float map(float value, float start1, float stop1, float start2, float stop2)
        {
            return start2 + norm(value, start1, stop1) * (stop2 - start2);
        }
        public float min(float a, float b) { return Mathf.Min(a, b); }
        public float min(float a, float b, float c) { return Mathf.Min(Mathf.Min(a, b), c); }
        public float min(params float[] values) { return Mathf.Min(values); }
        public float max(float a, float b) { return Mathf.Max(a, b); }
        public float max(float a, float b, float c) { return Mathf.Max(Mathf.Max(a, b), c); }
        public float max(params float[] values) { return Mathf.Max(values); }
        public int min(int a, int b) { return Math.Min(a, b); }
        public int min(int a, int b, int c) { return Math.Min(Mathf.Min(a, b), c); }
        public int max(int a, int b) { return Math.Max(a, b); }
        public int max(int a, int b, int c) { return Math.Max(Math.Max(a, b), c); }
        public static float norm(float value, float start, float stop) { return value / (stop - start); }
        public static float pow(float n, float e) { return Mathf.Pow(n, e); }
        public static int round(float f) { return (int)Mathf.Round(f); }
        public float sq(float f) { return f * f; }
        public float sqrt(float f) { return Mathf.Sqrt(f); }
        public float acos(float f) { return Mathf.Acos(f); }
        public float asin(float f) { return Mathf.Asin(f); }
        public float atan(float f) { return Mathf.Atan(f); }
        public float atan2(float y, float x) { return Mathf.Atan2(y, x); }
        public float cos(float f) { return Mathf.Cos(f); }
        public float sin(float f) { return Mathf.Sin(f); }
        public float tan(float f) { return Mathf.Tan(f); }
        public float degrees(float rad) { return Mathf.Rad2Deg * rad; }
        public float radians(float deg) { return Mathf.Deg2Rad * deg; }
        public float noise(float x) { return Mathf.PerlinNoise(x, 0); }
        public float noise(float x, float y) { return Mathf.PerlinNoise(x, y); }
        public float random(float low, float high) { return UnityEngine.Random.Range(low, high); }
        public float random(float high) { return UnityEngine.Random.Range(0.0f, high); }
        #if UNITY_5_4_OR_NEWER
        public void randomSeed(int seed) { UnityEngine.Random.InitState(seed); }
        #else
        public void randomSeed(int seed) { UnityEngine.Random.seed = seed; }
        #endif
        public int random(int high) { return UnityEngine.Random.Range(0, high); }
        public int random(int low, int high) { return UnityEngine.Random.Range(low, high); }
        private string ns(char c, int count) { return new String(c, count); }
        public string nf(int num, int digits) { return num.ToString("d" + digits); }
        public string nf(float num, int left, int right) { return num.ToString(ns('0', left) + "." + ns('0', right)); }
        public string nfs(int num, int digits) { return num.ToString(" d" + digits + ";" + "-d" + digits); }
        public string nfs(float num, int left, int right) { string s = ns('0', left) + "." + ns('0', right); return num.ToString(" " + s + ";-" + s); }
        public string nfp(int num, int digits) { return num.ToString("+d" + digits + ";" + "-d" + digits); }
        public string nfp(float num, int left, int right) { string s = ns('0', left) + "." + ns('0', right); return num.ToString("+" + s + ";-" + s); }
        public string nfc(int num) { return num.ToString("#,0"); }
        public string nfc(int num, int right) { return num.ToString("0,." + right); }
        public string[] split(string value, char delim) { return value.Split(delim); }
        public string[] splitTokens(string value, string delim = "¥t¥n¥r¥f ") { return value.Split(delim.ToCharArray(), StringSplitOptions.None); }
        public string trim(string value) { return value.Trim(); }
        public string join(string[] stringArray, string separator) { return string.Join(separator, stringArray); }
        #endregion

        #region Processing Extra Members
        public float frameSec { get { return Time.timeSinceLevelLoad; } }
        public float frameCount60 { get { return Time.timeSinceLevelLoad * 60.0f; } }
        public float modulo(float a, float b) { return a - (int)Mathf.Floor(a / b) * b; }
        public Vector3 randomVec2() { return UnityEngine.Random.insideUnitCircle; }
        public Vector3 randomVec3() { return UnityEngine.Random.insideUnitSphere; }
        public Vector3 randomUnitVec3() { return UnityEngine.Random.onUnitSphere; }
        public Vector3 randomScaleVec3(float r) { return UnityEngine.Random.onUnitSphere * r; }

        public Vector3 curvePos(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float t)
        {
            Vector3 pos = new Vector3();
            pos.x = catmullRom(v0.x, v1.x, v2.x, v3.x, t);
            pos.y = catmullRom(v0.y, v1.y, v2.y, v3.y, t);
            pos.z = catmullRom(v0.z, v1.z, v2.z, v3.z, t);
            return pos;
        }

        private float catmullRom(float p0, float p1, float p2, float p3, float t)
        {
            float v0 = (p2 - p0) * 0.5f;
            float v1 = (p3 - p1) * 0.5f;
            float t2 = t * t;
            float t3 = t2 * t;
            return ((p1 - p2) * 2.0f + v0 + v1) * t3 + ((p2 - p1) * 3.0f - 2.0f * v0 - v1) * t2 + v0 * t + p1;
        }
        #endregion
    }
}