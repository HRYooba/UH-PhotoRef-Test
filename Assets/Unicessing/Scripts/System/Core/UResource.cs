using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

namespace Unicessing
{
    public class UResource : UInput
    {
        public string getResourceName(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (directory.Length > 0) { directory += "/"; }
            return directory + Path.GetFileNameWithoutExtension(path);
        }

        private string tryLoadTextFromResources(string path)
        {
            string resourcePath = getResourceName(path);
            TextAsset textAsset = Resources.Load(resourcePath, typeof(TextAsset)) as TextAsset;
            if (textAsset != null) { debuglog("loadTextFromResources " + resourcePath); }
            return (textAsset != null) ? textAsset.text : null;
        }

        public string loadTextFromResources(string path)
        {
            string text = tryLoadTextFromResources(path);
            if (text == null) { debuglogWaring("loadTextFromResources <Failed> " + path); }
            return text;
        }

        public delegate void OnLoadCompleteStringArray(string[] text);

        public void loadTextFromURL(string url, OnLoadCompleteStringArray onComplete)
        {
            debuglog("loadTextFromURL loading... " + url);
            StartCoroutine(
                loadTextFromURL_corutine(url,
                    text =>
                    {
                        debuglog("loadTextFromURL load complete " + url);
                        string[] stringArray = text.Replace("\r\n", "\n").Split('\n');
                        if (onComplete != null) { onComplete(stringArray); }
                    }
                )
            );
        }

        public void loadTextFromURL(string url, Action<string> onComplete)
        {
            debuglog("loadTextFromURL loading... " + url);
#if false
		    WWW web = new WWW(url);
		    while(!web.isDone) {
			    if(Time.realtimeSinceStartup > time_start + 10.0f) {
				    debuglogError("loadTextFromURL: <TimeOut> " + url);
			    }
		    }
		    debuglog("loadTextFromURL load complete " + url);
		    if(onComplete!=null) { onComplete(web.text); }
#else
            StartCoroutine(
                loadTextFromURL_corutine(url,
                    text =>
                    {
                        debuglog("loadTextFromURL load complete " + url);
                        if (onComplete != null) { onComplete(text); }
                    }
                )
            );
#endif
        }

        public IEnumerator loadTextFromURL_corutine(string url, Action<string> onCompleate)
        {
            WWW web = new WWW(url);
            yield return web;
            onCompleate(web.text);
        }

        public string loadTextFromLocalFile(string path)
        {
            string result = null;
#if UNITY_WEBPLAYER
		    debuglogWaring("loadTextFromLocalFile: <NotWorking on WebPlayer>");
#else
            if (!File.Exists(path))
            {
                debuglogWaring("loadTextFromLocalFile <Not Found> " + path);
                return null;
            }
            debuglog("loadTextFromLocalFile " + path);
            FileInfo fi = new FileInfo(path);
            using (StreamReader sr = new StreamReader(fi.OpenRead()))
            {
                result = sr.ReadToEnd();
            }
#endif
            return result;
        }

        public string loadStringText(string filename, Action<string> onComplete = null)
        {
            if (filename.StartsWith("http://") || filename.StartsWith("file://"))
            {
                loadTextFromURL(filename, onComplete);
            }
            else
            {
                string t = tryLoadTextFromResources(filename);
                if (t == null)
                {
                    t = loadTextFromLocalFile(filename);
                }
                if (t != null)
                {
                    if (onComplete != null) { onComplete(t); }
                    return t;
                }
            }
            return null;
        }

        public void saveTextToLocalFile(string path, string[] data)
        {
#if UNITY_WEBPLAYER
		    debuglogWaring("saveTextToLocalFile: <NotWorking on WebPlayer>");
#else
            debuglog("saveTextToLocalFile " + path);
            FileInfo fi = new FileInfo(path);
            using (StreamWriter sw = fi.CreateText())
            {
                for (int i = 0; i < data.Length; i++)
                {
                    sw.WriteLine(data[i]);
                }
                sw.Flush();
            }
#endif
        }

        public void saveStringText(string path, string data)
        {
#if UNITY_WEBPLAYER
		    debuglogWaring("saveStringText: <NotWorking on WebPlayer>");
#else
            debuglog("saveStringText " + path);
            FileInfo fi = new FileInfo(path);
            using (StreamWriter sw = fi.CreateText())
            {
                sw.Write(data);
                sw.Flush();
            }
#endif
        }

        public void loadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void addScene(string name)
        {
            SceneManager.LoadScene(name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
    }
}
