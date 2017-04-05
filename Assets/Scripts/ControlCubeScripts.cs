using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCubeScripts : MonoBehaviour
{

    public UH uh;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = new Quaternion(-uh.UHQuaternion[1], -uh.UHQuaternion[3], -uh.UHQuaternion[2], uh.UHQuaternion[0]);
		float size = 4.0f - (uh.UHPR[4]) / 100.0f;
		transform.localScale = new Vector3(size * 2, size, size * 3);
    }
}
