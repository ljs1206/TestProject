using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

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
    private Label selectedLabel;

    private Dictionary<string, Label> _viewLableDictionary = new();
    
    [HideInInspector] public VisualElement _selected;

    public void CreateGUI()
    {
        _root = rootVisualElement;
        
        VisualElement labelFromUxml = m_VisualTreeAsset.Instantiate();
        labelFromUxml.style.flexGrow = 1;
        _root.Add(labelFromUxml);

        Button makeBtn = _root.Q<Button>("MakeBtn");
        Button deleteBtn = _root.Q<Button>("DeleteBtn");
        _prefabView = _root.Q<VisualElement>("PrefabView");
        EnumField viewType = _root.Q<EnumField>("ViewType");
        VisualElement element2D = _root.Q<VisualElement>("2DSetting");
        VisualElement element3D = _root.Q<VisualElement>("3DSetting");
        TextField fileNameField = _root.Q<TextField>("FileName");
        selectedLabel = _root.Q<Label>("NameLabel");

        if (_prefabTable.prefabList.Count > 0)
        {
            foreach(var item in _prefabTable.prefabList){
                ViewItem(item.name);
            }
        }
        
        fileNameField.RegisterValueChangedCallback(evt =>
        {
            if (_selected != null)
            {
                _viewLableDictionary.Add(fileNameField.text, _viewLableDictionary[_selected.name]);
                _viewLableDictionary.Remove(_selected.name);
                _viewLableDictionary[fileNameField.text].text = fileNameField.text;
                
                AssetDatabase.RenameAsset($"{_prefabFilePath}/{_selected.name}.prefab",
                    fileNameField.text);
            }
        });

        viewType.RegisterValueChangedCallback(evt => {
            switch (viewType.value)
            {
                case ViewSetting.None:
                element2D.style.display = DisplayStyle.None;
                element3D.style.display = DisplayStyle.None;
                break;
                case ViewSetting.View2D:
                element2D.style.display = DisplayStyle.Flex;
                break;
                case ViewSetting.View3D:
                element3D.style.display = DisplayStyle.Flex;
                break;
            }
        });

        makeBtn.clicked += HandleMakeBtnCllikEvent;
        deleteBtn.clicked += HandleDeleteBtnClickEvent;
    }

    private void ViewItem(string name){
        VisualElement element = new VisualElement();
        element.AddToClassList(_prefabVisual);
        element.name = name;

        element.RegisterCallback<PointerDownEvent>(ElementPointerDownEvent);

        Label label = new Label();
        label.AddToClassList(_prefabLabel);
        label.text = name;
        
        _viewLableDictionary.Add(name, label);
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

                selectedLabel.text = element.name;
            }
        }
    }

    private void HandleMakeBtnCllikEvent()
    {
        Guid id = Guid.NewGuid();
        GameObject obj = new GameObject();
        _prefabTable.prefabList.Add(PrefabUtility.SaveAsPrefabAsset(obj, 
            $"{_prefabSavePath}{id}.prefab", out bool isSuccess));
        obj.name = id.ToString();

        if(isSuccess){
            ViewItem(obj.name);
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

        VisualElement deleteElement = _prefabView.Q<VisualElement>(_selected.name);
        _prefabView.Remove(deleteElement);

        _prefabTable.prefabList.Remove(obj);
        AssetDatabase.DeleteAsset($"{_prefabFilePath}/{_selected.name}.prefab");
        EditorUtility.SetDirty(_prefabTable);
        AssetDatabase.SaveAssets();
    }
}
