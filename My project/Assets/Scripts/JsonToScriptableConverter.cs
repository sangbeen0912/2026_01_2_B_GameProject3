#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Data;

public enum ConversionType
{
    Items,

}

public class JsonToScriptableConverter : EditorWindow
{
    private string jsonFilePath = "";                                   // JSON 파일 경로 문자열 값
    private string outputFolder = "Assets/scriptObjects";               // 출력 SO 파일 경로 값
    private bool createDatabase = true;                                 // 데이터 베이스 활용 여부 체크 값
    private ConversionType conversionType = ConversionType.Items;




    [MenuItem("Tools/JSON to Scriptable Object")]
    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("JSON to Scriptable Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON to Scriptable object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json"); ;
        }

        EditorGUILayout.LabelField("Selected File : ", jsonFilePath);
        EditorGUILayout.Space();

        conversionType = (ConversionType)EditorGUILayout.EnumPopup("Conversion Type",conversionType);
        if(conversionType == ConversionType.Items && outputFolder == "Assets/ScriptableObjects")
        {
            outputFolder = "Assets/ScriptableObjects/Items";
        }  
        else if(conversionType == ConversionType.Dialogs && outputFolder == "Assets/ScriptableObjects")
        {
            outputFolder = "Assets/ScriptableObjects/DiaLogs";
        }




        outputFolder = EditorGUILayout.TextField("Output Foloder : ", outputFolder);
        createDatabase = EditorGUILayout.Toggle("Create Databse Asset", createDatabase);
        EditorGUILayout.Space();

        if(GUILayout.Button("Convert to Scriptable Object"))
        {
            if(string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Pease Selct a JSON file first", "OK");
                return;
            }
            ConvertJsonToScriptableObjects();
        }
    }


    private void ConvertJsonToScriptableObjects()       // JSON 파일을 ScriptableObject 파일로 변환 시켜주는 함수
    {
        // 폴더 생성
        if(!Directory.Exists(outputFolder))             // 폴더 위치를 확인하고 업으면 생성한다.
        {
            Directory.CreateDirectory(outputFolder);
        }

        // JSON 파일 읽기
        string jsonText = File.ReadAllText(jsonFilePath);       // JSON 파일을 읽는다.

        try
        {
            List<ItemData> itemDataList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

            List<ItemSO> createdItems = new List<ItemSO>();     // ItemSO 리스트 생성

            // 각 아이템을 테이터 스크립터블 오브젝트로 변환
            foreach(ItemData itemData in itemDataList)
            {
                ItemSO itemSO = ScriptableObject.CreateInstance<ItemSO>();      //ItemSO 파일을 생성

                // 데이터 복사
                itemSO.id = itemData.id;
                itemSO.itemName = itemData.itemName;
                itemSO.nameEng = itemData.nameEng;
                itemSO.description = itemData.description;

                // 열거형 변환
                if(System.Enum.TryParse(itemData.itemTypeString, out ItemType parsedType))
                {
                    itemSO.itemType = parsedType;  
                }
                else
                {
                    Debug.LogWarning($"아이템 {itemData.itemName}의 유효하지 않은 타입 : {itemData.itemTypeString}");
                }

                itemSO.price = itemData.price;
                itemSO.power = itemData.power;
                itemSO.level = itemData.level;
                itemSO.isStackable = itemData.isStackable;


                //아이콘 로드 (경로가 있는 경우)        //아이콘 경로가 있는지 확인한다.
                if(!string.IsNullOrEmpty(itemData.iconPath))
                {
                    itemSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{itemData.iconPath}.png");

                    if(itemSO.icon == null)
                    {
                        Debug.LogWarning($"아이템 {itemData.nameEng} 의 아이콘을 찾을 수 없습니다. : {itemData.iconPath}");
                    }
                }

                // 스크립터블 오브젝트 저장 - ID를 4가지 숫자로 포맷팅
                string assetPath = $"{outputFolder}/Item_{itemData.id.ToString("D4")}_{itemData.nameEng}.asset";
                AssetDatabase.CreateAsset(itemSO, assetPath );


                // 에셋 이름 지정
                itemSO.name = $"Item_{itemData.id.ToString("D4")} + {itemData.nameEng}";
                createdItems.Add(itemSO);

                EditorUtility.SetDirty(itemSO);
            }

            // 데이터베이스
            if (createDatabase && createdItems.Count > 0)       // 생성
            {
                ItemDataBaseSO dataBase = ScriptableObject.CreateInstance<ItemDataBaseSO>();
                dataBase.items = createdItems;

                AssetDatabase.CreateAsset(dataBase, $"{outputFolder}/ItemDatabase.asset");
                EditorUtility.SetDirty(dataBase);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdItems.Count} scriptable objects!", "OK"); ;
        }

           
        catch(System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 : {e}");
        }
    }

    private void ConvertJsonToDialogScriptableObjects()
    {
      if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        String jsonText = File.ReadAllText(jsonFilePath);
        try
        {
            

            List<DialogRowData> rowDataList = JsonConvert.DeserializeObject<List<DialogRowData>>(JsonText);
             
            Dictionary<int,DialogS0> dialogMap = new Dictionary<int,DialogS0>();
            List<DialogS0> creataDialogs = new List<DialogS0>();
            
            foreach(var rowData in rowDataList)
            {
                DialogS0 dialogS0 = ScriptableObject.CreateInstance<DialogS0>();

                dialogS0.id = rowData.id.Value;
                dialogS0.charachrtName = rowData.charachrtName;
                dialogS0.text = rowData.text;
                dialogS0.nextId = rowData.nextId.HasValue ? rowData.nextId.Value : -1 ;
                dialogS0.portraitPath = rowData.portraitPath;
                dialogS0.choices = new List<DialogChoicesS0>();

                if(!string.IsNullOrEmpty(rowData.protraitPath))
                {
                    dialogS0.portrait = Resources.Load<Sprite>(rowData.protraitPath);
                   
                    if (dialogS0.portrait == null)
                    {
                        Debug.LogWarning($"대화 {rowData.id}의 초상화를 찾을 수 없습니다.");

                    }
                }
                dialogMap[dialogS0.id] = dialogS0;
                createDialogs.Add(dialogS0);

               foreach(var rowData in rowDataList)
                {
                    if(!rowData.id.HasValue &&!string.IsNullOrEmpty(rowData.choiceText) && rowData.choiceNextId,HasValue)
                    {
                        int parentid = -1;
                        int currentIndex = rowDataList.IndexOf(rowData);
                        for (int i = currentIndex -1 ; i >= 0 ; i--)
                        {
                            if (rowDataList[i].id.HasValue)
                            {
                                parentid = rowDataList[].id.Value;
                                break;
                            }
                        }
                    }
                }

            }

            foreach(var dialog in creataDialogs)
            {
                string assetPath = $"{outputFolder}/Dialog {dialog.id.ToString("D4")}.asset";
                dialog.name = $"Dialog_{dialog.id.ToString("D4")}";

                EditorUtility.SetDirty(dialog);

            }
            if(createDatabase && createDialogs.Count >0)
            {
                DialogDatabasS0 database = ScriptableObject.CreateInstance<DialogDatabasS0>();
                database.dialogs = creataDialogs;

                AssetDatabase.CreateAsset(database, $"{outputFolder}/dialogDatabase.assets");
                EditorUtility.SetDirty(database);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success",$"created{createDialogs.Count} dialog scriptable dbjects!", "OK");
 

        }
        catch(System.Exception e)
        {
            EditorUtility.DisplayDialog("Error",$"Faild to convert JSON:{ e.Message}","OK");
            Debug.LogError($"JSON 변환 오류:{e}");
        }

    }
}

#endif