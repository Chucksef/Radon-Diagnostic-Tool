using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FanDataEditor : EditorWindow
{
    public FanOptions fanOptions;
    private string fanDataProjectFilePath = "/StreamingAssets/data.json";

    [MenuItem ("Window/Game Data Editor")]
    static void Init()
    {
        FanDataEditor window = (FanDataEditor)EditorWindow.GetWindow(typeof(FanDataEditor));
        window.Show();
    }

    private void OnGUI()
    {
        if(fanOptions != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("fanOptions");

            EditorGUILayout.PropertyField(serializedProperty, true);

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save Data"))
            {
                SaveGameData();
            }
        }
        if (GUILayout.Button("Load Data"))
        {
            LoadGameData();
        }
    }

    private void LoadGameData()
    {
        string filePath = Application.dataPath + fanDataProjectFilePath;

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            fanOptions = JsonUtility.FromJson<FanOptions>(dataAsJson);
        }
        else
        {
            fanOptions = new FanOptions();
        }
    }

    private void SaveGameData()
    {
        string dataAsJson = JsonUtility.ToJson(fanOptions);
        string filePath = Application.dataPath + fanDataProjectFilePath;
        File.WriteAllText(filePath, dataAsJson);
    }
}
