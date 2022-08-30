using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditor : EditorWindow
{
    private int gridSize = 10;
    private int minGridSize = 5;
    private int maxGridSize = 15;

    private int tempGridSize;
    private int currentCellIndex = 0;
    private int tempTextureIndex = 0;

    private List<bool> levelDataList;
    private List<Texture2D> cellTextureList;

    private Texture2D filledTexture;
    private Texture2D crossedTexture;
    private GUIStyle cellButtonStyle;

    private void OnEnable()
    {
        Debug.Log("LEVEL EDITOR IS ONLINE");

        currentCellIndex = 0;
        tempTextureIndex = 0;
        tempGridSize = 0;

        levelDataList = new List<bool>();
        cellTextureList = new List<Texture2D>();

        filledTexture = EditorGUIUtility.Load("Assets/Image/Cell Images/kare-500.png") as Texture2D;
        crossedTexture = EditorGUIUtility.Load("Assets/Image/Cell Images/500-500-x.png") as Texture2D;

        cellTextureList.Add(filledTexture);
        cellTextureList.Add(crossedTexture);

        for(int i = 0; i < maxGridSize * maxGridSize; i++)
        {
            levelDataList.Add(false);
        }

        cellButtonStyle = new GUIStyle()
        {
            onNormal = new GUIStyleState()
            {
                background = cellTextureList[tempTextureIndex]
            }
        };
    }

    [MenuItem("Window/Windows/Level Editor")]
    public static void OpenWindow()
    {
        GetWindow<LevelEditor>("Level Editor");
        //GetWindow<LevelEditor>("Level Editor").ShowUtility();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void DrawGrid()
    {
        GUILayout.Space(40f);

        for (int i = 0; i < gridSize; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            for (int j = 0; j < gridSize; j++)
            {
                currentCellIndex = (i * gridSize) + j;

                if (!levelDataList[currentCellIndex])
                {
                    tempTextureIndex = 0;
                }

                else
                {
                    tempTextureIndex = 1;
                }

                if (GUILayout.Button(cellTextureList[tempTextureIndex], GUILayout.Height(25f), GUILayout.Width(25f)))
                {
                    bool tempBool = levelDataList[currentCellIndex];
                    levelDataList[currentCellIndex] = !tempBool;

                    for(int k = 0; k < gridSize * gridSize; k++)
                    {
                        Debug.Log(k / gridSize + ", " + k % gridSize + " - " + levelDataList[k]);
                    }
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

    private void DrawGridSizeField()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Size:", GUILayout.Height(20f), GUILayout.Width(60f));
        tempGridSize = EditorGUI.IntField(new Rect(65f, 3f, 60f, 20f), tempGridSize);
        GUILayout.EndHorizontal();    
    }

    private void CheckGridSize()
    {
        if (tempGridSize != gridSize)
        {
            if (tempGridSize > maxGridSize)
            {
                tempGridSize = maxGridSize;
            }

            else if (tempGridSize < minGridSize)
            {
                tempGridSize = minGridSize;
            }

            gridSize = tempGridSize;  
        }
    }

    private void SaveLevelData()
    {
        string path = Application.dataPath + "/Level Data/level_0.txt";
        string result = "";
        
        if(!File.Exists(path))
        {
            for (int i = 0; i < gridSize * gridSize; i++)
            {
                if(!levelDataList[i])
                {
                    result += "0";
                }

                else
                {
                    result += "1";
                }

                if (i != gridSize * gridSize - 1)
                {
                    result += ",";
                }
            }

            File.WriteAllText(path, result);
        }
    }

    private void DrawBelowButtons()
    {
        GUILayout.Space(40f);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("SAVE", GUILayout.Height(20f), GUILayout.Width(50f)))
        {
            SaveLevelData();
        }

        if (GUILayout.Button("CLEAR", GUILayout.Height(20f), GUILayout.Width(50f)))
        {
            for (int i = 0; i < maxGridSize * maxGridSize; i++)
            {
                levelDataList[i] = false;
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }

    private void OnGUI()
    {
        DrawGridSizeField();
        CheckGridSize();
        DrawGrid();
        DrawBelowButtons();
    }
}
