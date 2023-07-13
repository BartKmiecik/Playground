using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Make360Images : MonoBehaviour
{
    public Transform targetObject;
    public RenderTexture rt;
    public string renderFolderPath = "Assets\\Renders";
    public Transform car;

    private Camera mainCamera;
    private int renderIndex = 0;
    private static int counter = 0;


    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(PerformRenders());
    }

    private IEnumerator PerformRenders()
    {
        for(int j = 0; j< 3; j++)
        {
            for (int i = 1; i < 37; i++)
            {
                // Move camera around the target object
                float angle = i * 10f;
                Vector3 offset = new Vector3(car.eulerAngles.x, angle, 0f);
                Vector3 cameraPosition = targetObject.position + offset * 8f;
                //mainCamera.transform.localPosition = cameraPosition;
                //mainCamera.transform.LookAt(targetObject);
                car.eulerAngles = offset;

                // Capture and save render
                string renderFilePath = GetRenderFilePath(renderFolderPath, renderIndex);
                yield return new WaitForEndOfFrame();
                //SaveRenderTexture(rt, renderFilePath);
                TakeScreenshot();

                // Rotate the target object by 10 degrees

                renderIndex++;
            }
            Vector3 rottt = new Vector3(car.eulerAngles.x + 15, car.eulerAngles.y, car.eulerAngles.z);
            car.eulerAngles = rottt;
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
        //Debug.Log($"Saved texture: {rt.width}x{rt.height} - " + path);
    }
    [MenuItem("Assets/Take Screenshot", true)]
    public static bool TakeScreenshotValidation() =>
        Selection.activeGameObject && Selection.activeGameObject.GetComponent<Camera>();
    [MenuItem("Assets/Take Screenshot")]
    public static void TakeScreenshot()
    {
        ++counter;
        var camera = Selection.activeGameObject.GetComponent<Camera>();
        var prev = camera.targetTexture;
        var rt = new RenderTexture(Screen.width, Screen.height, 16);
        camera.targetTexture = rt;
        camera.Render();
        SaveRenderTexture(rt, "Assets\\Renders\\" + counter.ToString() + "screenshot.png");
        camera.targetTexture = prev;
        Object.DestroyImmediate(rt);
    }

}
