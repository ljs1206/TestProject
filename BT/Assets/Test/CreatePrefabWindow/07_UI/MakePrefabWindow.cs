using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MakePrefabWindow : EditorWindow
{
    private readonly string _prefabSavePath = "Assets/Test/CreatePrefabWindow/03_Prefab/";

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField]
    private StyleSheet _styleSheet;

    [MenuItem("Editor/MakePrefabWindow")]
    public static void ShowExample()
    {
        MakePrefabWindow wnd = GetWindow<MakePrefabWindow>();
        wnd.titleContent = new GUIContent("MakePrefabWindow");
        wnd.minSize = new Vector2(800, 600);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        labelFromUXML.style.flexGrow = 1;
        root.Add(labelFromUXML);

        Button btn = root.Q<Button>("MakeBtn");
        VisualElement prefabView = root.Q<VisualElement>("PrefabView");

        var prefabs = AssetDatabase.LoadAllAssetsAtPath("Assets/Test/CreatePrefabWindow/03_Prefab");
        Debug.Log(prefabs.Length);
        
        foreach(var item in prefabs){
            Debug.Log(item.name);
            VisualElement element = new VisualElement();
            element.styleSheets.Add(_styleSheet);
            prefabView.Add(element);
        }

        btn.clicked += HandleBtnCllikEvent;
    }

    private void HandleBtnCllikEvent()
    {
        Guid id = Guid.NewGuid();
        GameObject obj = new GameObject();
        PrefabUtility.SaveAsPrefabAsset(obj, $"{_prefabSavePath}{id}.prefab", out bool isSuccess);
        DestroyImmediate(obj);

        if(isSuccess){
            Debug.Log($"Success Create Prefab \n Name : {id} \nPath : {_prefabSavePath}{id}");
        }
        else{
            Debug.Log("Failure Create Prefab");
        }
    }
}
