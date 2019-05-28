using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ScrollList : MonoBehaviour {

    public List<FanModel> fanModelList;
    public SimpleObjectPool rowObjectPool;

    private FanModel[] allFanModels;
    private string fanDataFileName = "data.json";

    public Color evenRowColor = new Color(1,1,1);
    public Color oddRowColor = new Color(0,0,0);

    // Use this for initialization
    void Start () {
        LoadFanData();
        RefreshDisplay();
	}

    public void RefreshDisplay()
    {
        AddRows();
    }

    private void LoadFanData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fanDataFileName);
        string readData;

        if (Application.platform == RuntimePlatform.Android)
        {
            WWW reader = new WWW(filePath);
            while (!reader.isDone) { }
            readData = reader.text;

            if(readData != "")
            {
                FanOptions loadedData = JsonUtility.FromJson<FanOptions>(readData);
                allFanModels = loadedData.allFanModels;

                fanModelList.Clear();
                for (int i = 0; i < allFanModels.Length; i++)
                {
                    fanModelList.Add(allFanModels[i]);
                }
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                readData = File.ReadAllText(filePath);
                FanOptions loadedData = JsonUtility.FromJson<FanOptions>(readData);
                allFanModels = loadedData.allFanModels;

                fanModelList.Clear();
                for (int i = 0; i < allFanModels.Length; i++)
                {
                    fanModelList.Add(allFanModels[i]);
                }
            } else
            {
                Debug.LogError("No Database File Found!");
            }
        }
    }

    private void AddRows()
    {
        for (int i = 0; i < fanModelList.Count; i++)
        {
            FanModel currentFanModel = fanModelList[i];
            GameObject newRow = rowObjectPool.GetObject();
            newRow.transform.SetParent(gameObject.transform, false);
            Image img = newRow.GetComponent<Image>();
            if (i % 2 == 0)
            {
                img.color = evenRowColor;
            }
            else
            {
                img.color = oddRowColor;
            }

            RowData rowData = newRow.GetComponent<RowData>();
            rowData.Setup(currentFanModel);
        }
    }
}
