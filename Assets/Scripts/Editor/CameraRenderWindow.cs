using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CameraRenderWindow : EditorWindow
{
    private int imageWidth = 1920;
    private int imageHeight = 1080;
    private string fileName = "EditorRender.png";

    [MenuItem("Tools/Camera Render Window")]
    public static void ShowWindow()
    {
        GetWindow<CameraRenderWindow>("Camera Render");
    }

    private void OnGUI()
    {
        GUILayout.Label("Настройки рендера", EditorStyles.boldLabel);

        imageWidth = EditorGUILayout.IntField("Ширина (Width)", imageWidth);
        imageHeight = EditorGUILayout.IntField("Высота (Height)", imageHeight);
        fileName = EditorGUILayout.TextField("Имя файла", fileName);

        GUILayout.Space(15);

        Camera selectedCamera = GetSelectedCamera();

        if (selectedCamera == null)
        {
            EditorGUILayout.HelpBox("Пожалуйста, выберите камеру в окне Hierarchy.", MessageType.Warning);
            GUI.enabled = false; 
        }
        else
        {
            EditorGUILayout.HelpBox($"Выбрана камера: {selectedCamera.name}", MessageType.Info);
            GUI.enabled = true; 
        }

        if (GUILayout.Button("Отрендерить и сохранить PNG", GUILayout.Height(40)))
        {
            CaptureCameraToPNG(selectedCamera);
        }

        GUI.enabled = true; 
    }

    private Camera GetSelectedCamera()
    {
        if (Selection.activeGameObject != null)
        {
            return Selection.activeGameObject.GetComponent<Camera>();
        }
        return null;
    }

    private void CaptureCameraToPNG(Camera cam)
    {
        RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = cam.targetTexture;
        cam.targetTexture = renderTexture;

        cam.Render();

        RenderTexture.active = renderTexture;
        Texture2D screenShot = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
        screenShot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenShot.Apply();

        cam.targetTexture = previousTarget;
        RenderTexture.active = null;
        DestroyImmediate(renderTexture); 

        byte[] bytes = screenShot.EncodeToPNG();
        DestroyImmediate(screenShot);

        string folderPath = Path.Combine(Application.dataPath, "SavedRenders");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (!fileName.ToLower().EndsWith(".png"))
        {
            fileName += ".png";
        }

        string fullPath = Path.Combine(folderPath, fileName);
        File.WriteAllBytes(fullPath, bytes);

        AssetDatabase.Refresh();

        Debug.Log($"[CameraRender] Изображение сохранено: {fullPath}");
        EditorUtility.DisplayDialog("Успех!", $"Изображение сохранено в Assets/SavedRenders/{fileName}", "OK");
    }
}
