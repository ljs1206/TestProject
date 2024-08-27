using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private readonly string _prefabVisualSelect = "prefab-visual-select";
    private readonly string _prefabFilePath = "Assets/Test/CreatePrefabWindow/03_Prefab";

    [MenuItem("Editor/MakePrefabWindow")]
    public static void ShowExample()
    {
        MakePrefabWindow wnd = GetWindow<MakePrefabWindow>();
        wnd.titleContent = new GUIContent("MakePrefabWindow");
        wnd.minSize = new Vector2(800, 600);
    }

    private VisualElement _root;
    private VisualElement _prefabView;
    
    [HideInInspector] public VisualElement _selected;

    public void CreateGUI()
    {
        _root = rootVisualElement;
        
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        labelFromUXML.style.flexGrow = 1;
        _root.Add(labelFromUXML);

        Button makeBtn = _root.Q<Button>("MakeBtn");
        Button deleteBtn = _root.Q<Button>("DeleteBtn");
        _prefabView = _root.Q<VisualElement>("PrefabView");

        if(_prefabTable.prefabList.Count > 0){
            foreach(var item in _prefabTable.prefabList){
                ViewItem(item);
            }
        }
        
        // foreach(var item in _prefabTable.prefabList){
        //     VisualElement visualElement = root.Q<Button>(item.name);

        //     if(visualElement != null)
        //         _prefabView.Remove(visualElement);
        // }

        makeBtn.clicked += HandleMakeBtnCllikEvent;
        deleteBtn.clicked += HandleDeleteBtnClickEvent;
    }


    private void ViewItem(GameObject obj){
        VisualElement element = new VisualElement();
        element.AddToClassList(_prefabVisual);
        element.name = obj.name;

        element.RegisterCallback<PointerDownEvent>(ElementPointerDownEvent);

        Label label = new Label();
        label.AddToClassList(_prefabLabel);
        label.text = obj.name;

        element.Add(label);
        _prefabView.Add(element);
    }

    private void ElementPointerDownEvent(PointerDownEvent evt)
    {
        foreach(var item in _prefabTable.prefabList){
            VisualElement element = _root.Q<VisualElement>(item.name);
            List<string> classNames = element.GetClasses().ToList();

            foreach(string str in classNames){
                if(str == _prefabVisualSelect){
                    element.AddToClassList(_prefabVisual);
                    element.RemoveFromClassList(_prefabVisualSelect);
                }
            }

            if(evt.currentTarget.GetHashCode() == element.GetHashCode()){
                _selected = element;
                element.RemoveFromClassList(_prefabVisual);
                element.AddToClassList(_prefabVisualSelect);
            }
        }
    }

    private void HandleMakeBtnCllikEvent()
    {
        Guid id = Guid.NewGuid();
        GameObject obj = new GameObject();
        _prefabTable.prefabList.Add(PrefabUtility.SaveAsPrefabAsset(obj, $"{_prefabSavePath}{id}.prefab", out bool isSuccess));
        obj.name = id.ToString();

        if(isSuccess){
            ViewItem(obj);
            Debug.Log($"Success Create Prefab \n Name : {id} \nPath : {_prefabSavePath}{id}");
        }
        else{
            Debug.Log("Failure Create Prefab");
        }
        DestroyImmediate(obj);
    }

    private void HandleDeleteBtnClickEvent()
    {
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>($"{_prefabFilePath}/{_selected.name}.prefab");

        _prefabTable.prefabList.Remove(obj);
        EditorUtility.SetDirty(obj);
        AssetDatabase.DeleteAsset($"{_prefabFilePath}/{_selected.name}.prefab");
        AssetDatabase.Refresh();
    }
}
