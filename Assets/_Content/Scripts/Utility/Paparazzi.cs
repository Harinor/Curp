using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class Paparazzi : MonoBehaviour
{
    public static Paparazzi instance;

    [SerializeField] Camera theCamera;

    bool isStalking;

    #region --- UNITY CALLBACKS ---
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }
    #endregion

    #region --- METHODS ---
    private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (isStalking)
        {
            ProcessTakingScreenshot();
        }
    }

    private void ProcessTakingScreenshot()
    {
        MasterManager.cout("ProcessTakingScreenshot()");
        isStalking = false;
        RenderTexture renderTexture = theCamera.targetTexture;

        Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height);
        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
        renderResult.ReadPixels(rect, 0, 0);

        string filename = $"Curp_{DateTime.Now:yyyyMMddHHmmss}.png";
        string path = Path.Combine(Application.dataPath/*, "screenshots"*/, filename);

        if (Application.platform == RuntimePlatform.Android)
        {
            //SaveImageToGallery(renderResult, filename, $"A file called {filename}.");
            //MasterManager.cout("Android: ProcessTakingScreenshot()");
            NativeToolkit.SaveImage(renderResult, filename+"img", "png");
            //NativeToolkit.SaveScreenshot(filename+"scr", "Curp", "jpg", rect);
        }
        else
        {
            string screenshotsPath = Path.Combine(Application.dataPath, "screenshots");
            if (!Directory.Exists(screenshotsPath))
            {
                Directory.CreateDirectory(screenshotsPath);
            }

            byte[] byteArray = renderResult.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(screenshotsPath, filename), byteArray);
        }

        RenderTexture.ReleaseTemporary(renderTexture);
        theCamera.targetTexture = null;
    }

    public static void TakeTheShot()
    {
        instance.TakeScreenshot();
    }

    private void TakeScreenshot()
    {
        theCamera.targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.width, 16);
        isStalking = true;
    }

    #endregion

    #region --- ANDROID ---
    private static AndroidJavaObject _activity;
    public static AndroidJavaObject Activity
    {
        get
        {
            if (_activity == null)
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _activity;
        }
    }

    private const string MediaStoreImagesMediaClass = "android.provider.MediaStore$Images$Media";
    public static string SaveImageToGallery(Texture2D texture2D, string title, string description)
    {
        using (var mediaClass = new AndroidJavaClass(MediaStoreImagesMediaClass))
        {
            using (var cr = Activity.Call<AndroidJavaObject>("getContentResolver"))
            {
                var image = Texture2DToAndroidBitmap(texture2D);
                var imageUrl = mediaClass.CallStatic<string>("insertImage", cr, image, title, description);
                return imageUrl;
            }
        }
    }

    public static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D texture2D)
    {
        byte[] encoded = texture2D.EncodeToPNG();
        using (var bf = new AndroidJavaClass("android.graphics.BitmapFactory"))
        {
            return bf.CallStatic<AndroidJavaObject>("decodeByteArray", encoded, 0, encoded.Length);
        }
    }
#endregion
}
