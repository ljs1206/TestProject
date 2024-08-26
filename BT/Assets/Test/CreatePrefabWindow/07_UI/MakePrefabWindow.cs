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
    [SerializeField]
    private PrefabTableSO _prefabTable;

    private readonly string _prefabVisual = "prefab-visual";
    private readonly string _prefabLabel = "prefab-label";

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
        
        // foreach(var item in _prefabTable.prefabList){
        //     VisualElement visualElement = root.Q<Button>(item.name);

        //     if(visualElement != null)
        //         prefabView.Remove(visualElement);
        // }

        foreach(var item in _prefabTable.prefabList){
            Debug.Log(item.name);
            VisualElement element = new VisualElement();
            element.AddToClassList(_prefabVisual);
            element.name = item.name;

            Label label = new Label();
            label.AddToClassList(_prefabLabel);
            label.text = item.name;
            
            element.Add(label);
            prefabView.Add(element);
        }

        btn.clicked += HandleBtnCllikEvent;
    }

    private void HandleBtnCllikEvent()
    {
        Guid id = Guid.NewGuid();
        GameObject obj = new GameObject();
        _prefabTable.prefabList.Add(PrefabUtility.SaveAsPrefabAsset(obj, $"{_prefabSavePath}{id}.prefab", out bool isSuccess));
        DestroyImmediate(obj);

        if(isSuccess){
            Debug.Log($"Success Create Prefab \n Name : {id} \nPath : {_prefabSavePath}{id}");
        }
        else{
            Debug.Log("Failure Create Prefab");
        }
    }
}
