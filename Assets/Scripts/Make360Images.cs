using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

public class Make360Images : MonoBehaviour
{
    public Transform targetObject;
    public RenderTexture rt;
    public string renderFolderPath = "Assets\\Renders";

    private Camera mainCamera;
    private int renderIndex = 0;


    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(PerformRenders());
    }

    private IEnumerator PerformRenders()
    {
        for (int i = 0; i < 36; i++)
        {
            // Move camera around the target object
            float angle = i * 10f;
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            Vector3 cameraPosition = targetObject.position + offset * 2f;
            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.LookAt(targetObject);

            // Capture and save render
            string renderFilePath = GetRenderFilePath(renderFolderPath, renderIndex);
            yield return new WaitForEndOfFrame();
            //SaveRenderTexture(rt, renderFilePath);
            TakeScreenshot(i);

            // Rotate the target object by 10 degrees

            renderIndex++;
        }
    }

    private string GetRenderFilePath(string folderPath, int index)
    {
        string fileName = "Render_" + index.ToString("D3") + ".png";
        string filePath = Path.Combine(folderPath, fileName);
        return filePath;
    }

    static void SaveRenderTexture(RenderTexture rt, string path)
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;
        var bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        Debug.Log($"Saved texture: {rt.width}x{rt.height} - " + path);
    }
    [MenuItem("Assets/Take Screenshot", true)]
    public static bool TakeScreenshotValidation() =>
        Selection.activeGameObject && Selection.activeGameObject.GetComponent<Camera>();
    [MenuItem("Assets/Take Screenshot")]
    public static void TakeScreenshot(int i)
    {
        var camera = Selection.activeGameObject.GetComponent<Camera>();
        var prev = camera.targetTexture;
        var rt = new RenderTexture(Screen.width, Screen.height, 16);
        camera.targetTexture = rt;
        camera.Render();
        SaveRenderTexture(rt, "Assets\\Renders\\" + i.ToString() + "screenshot.png");
        camera.targetTexture = prev;
        Object.DestroyImmediate(rt);
    }

}
