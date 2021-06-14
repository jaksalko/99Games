using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureCamera : MonoBehaviour
{
    public Camera renderTextureCamera;
    // Start is called before the first frame update
    void Start()
    {
        renderTextureCamera.clearFlags = CameraClearFlags.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
