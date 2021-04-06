﻿using UnityEngine;
using System.Collections;

public class HiResScreenShots : MonoBehaviour
{
    public int resWidth = 1920; // 2550
    public int resHeight = 1080; // 3300

    public Camera cameraTarget;

    private bool takeHiResShot = false;

    void Start()
    {
        if (cameraTarget == null)
        {
            cameraTarget = GetComponentInChildren<Camera>();
        }
    }

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/Screenshots/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown("o");
        if (takeHiResShot)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            cameraTarget.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            cameraTarget.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            cameraTarget.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
    }
}