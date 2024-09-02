using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class PrefabManager : EditorWindow
{
    private readonly string _prefabSavePath = "Assets/Test/CreatePrefabWindow/03_Prefab/";
    
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField]
    private PrefabTableSO _prefabTable;
    
    private VisualElement _root;

    [MenuItem("Editor/Prefab/PrefabManager")]
    public static void CShowWindow()
    {
        PrefabManager wnd = GetWindow<PrefabManager>();
        wnd.titleContent = new GUIContent("PrefabManager");
        wnd.minSize = new Vector2(600, 800);
        wnd.maxSize = new Vector2(600, 800);
    }

    #region Elements
    private EnumField _viewType;
    private TextField _nameField2D;
    private ObjectField _visualField2D;
    private ObjectField _statField2D;
    private EnumField _aiType2D;
    private EnumField _colliderType2D;
    private TextField _nameField3D;
    private ObjectField _visualField3D;
    private ObjectField _statField3D;
    private EnumField _aiType3D;
    private EnumField _colliderType3D;
    #endregion
    
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        _root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUxml = m_VisualTreeAsset.Instantiate();
        _root.Add(labelFromUxml);
        labelFromUxml.style.flexGrow = 1;
        
        _viewType = _root.Q<EnumField>("ViewEnum");
        VisualElement element2D = _root.Q<VisualElement>("2DSetting");
        VisualElement element3D = _root.Q<VisualElement>("3DSetting");
        Button makeBtn = _root.Q<Button>("CreateBtn");

        #region 2D
        _nameField2D = element2D.Q<TextField>("NameTag");
        _visualField2D = element2D.Q<ObjectField>("VisualField");
        _statField2D = element2D.Q<ObjectField>("StatField");
        _aiType2D = element2D.Q<EnumField>("AIType");
        _colliderType2D = element2D.Q<EnumField>("ColliderType");
        #endregion
        
        #region 3D
        _nameField3D = element3D.Q<TextField>("NameTag");
        _visualField3D = element3D.Q<ObjectField>("VisualField");
        _statField3D = element3D.Q<ObjectField>("StatField");
        _aiType3D = element3D.Q<EnumField>("AIType");
        _colliderType3D = element3D.Q<EnumField>("ColliderType");
        #endregion
        
        DefaultPrefabInfo2D prefabInfo2D = new();
        DefaultPrefabInfo3D prefabInfo3D = new();
        
        // Prefab의 2차원으로 만들지 3차원으로 만들지 Enum으로 Switch로 분류하는 Event이다.
        _viewType.RegisterValueChangedCallback(evt => {
            switch (_viewType.value)
            {
                case ViewSetting.None:
                    element2D.style.display = DisplayStyle.None;
                    element3D.style.display = DisplayStyle.None;
                    break;
                case ViewSetting.View2D:
                    element2D.style.display = DisplayStyle.Flex;
                    element3D.style.display = DisplayStyle.None;
                    break;
                case ViewSetting.View3D:
                    element3D.style.display = DisplayStyle.Flex;
                    element2D.style.display = DisplayStyle.None;
                    break;
            }
        });
        
        // Value Change Event(RegisterValueChangedCallback) 2D와 3D 둘다 이벤트 구독함
        // todo(Stat 만들면 넣어주기!!)
        #region NameField
        _nameField2D.RegisterValueChangedCallback(evt =>
        {
            prefabInfo2D.FileName = _nameField2D.value;
        });
        
        _nameField3D.RegisterValueChangedCallback(evt =>
        {
            prefabInfo3D.FileName = _nameField3D.value;
        });
        #endregion
        #region VisualField
        _visualField2D.RegisterValueChangedCallback(evt =>
        {
            prefabInfo2D.VisualPrefab = _visualField2D.value as GameObject;
        });
        
        _visualField3D.RegisterValueChangedCallback(evt =>
        {
            prefabInfo3D.VisualPrefab = _visualField3D.value as GameObject;
        });
        #endregion
        #region StatField
        
        #endregion
        #region AIType
        _aiType2D.RegisterValueChangedCallback(evt =>
        {
            prefabInfo2D.AIType = (AIType)_aiType2D.value;
        });
        
        _aiType3D.RegisterValueChangedCallback(evt =>
        {
            prefabInfo3D.AIType = (AIType)_aiType3D.value;
        });
        #endregion
        #region ColliderType
        _colliderType2D.RegisterValueChangedCallback(evt =>
        {
            _colliderType2D.value = (ColliderType2D)_colliderType2D.value;
        });

        _colliderType3D.RegisterValueChangedCallback(evt =>
        {
            _colliderType3D.value = (ColliderType3D)_colliderType3D.value;
        });
        #endregion

        makeBtn.clicked += ClickMakeBtnEvent;
    }

    private void ClickMakeBtnEvent()
    {
        if((ViewSetting)_viewType.value == ViewSetting.None) return;
        
        Guid id = Guid.NewGuid();
        GameObject obj = new GameObject();

        switch (_viewType.value)
        {
            case ViewSetting.View2D:
                var info2D = obj.AddComponent<DefaultPrefabInfo2D>();
                info2D.FileName = _nameField2D.value;
                info2D.VisualPrefab = _visualField2D.value as GameObject;
                SetAICompo(obj, info2D);
                SetColliderCompo(obj, info2D, (ViewSetting)_viewType.value);
                break;
            case ViewSetting.View3D:
                var info3D = obj.AddComponent<DefaultPrefabInfo3D>();
                info3D.FileName = _nameField3D.value;
                info3D.VisualPrefab = _visualField2D.value as GameObject;
                SetAICompo(obj, info3D);
                SetColliderCompo(obj, info3D, (ViewSetting)_viewType.value);
                break; 
            default:
                DestroyImmediate(obj);
                return;
        }

        obj.name = id.ToString();
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obj,
            $"{_prefabSavePath}{id}.prefab" , out bool isSuccess);
        _prefabTable.prefabList.Add(prefab);
        
        if(isSuccess)
        {
            // ViewItem(prefab.name);
            Debug.Log($"Success Create Prefab \n Name : {id} \nPath : {_prefabSavePath}{id}");
        }
        else{
            Debug.Log("Failure Create Prefab");
        }
        EditorUtility.SetDirty(_prefabTable);
        AssetDatabase.SaveAssets();
        DestroyImmediate(obj);
    }

    private void SetAICompo(GameObject obj, DefaultPrefabInfo info)
    {
        switch (info.AIType)
        {
            case AIType.None:
                return;
            case AIType.BT:
                // Add BT Compo
                obj.AddComponent<BehaviourTreeRunner>();
                return;
            case AIType.FSM:
                // Add FSM Compo
                return;
        }
    }
    
    private void SetColliderCompo(GameObject obj, DefaultPrefabInfo info, ViewSetting viewType)
    {
        if (viewType == ViewSetting.View2D)
        {
            switch (((DefaultPrefabInfo2D)info).ColliderType)
            {
                case ColliderType2D.None:
                    return;
                case ColliderType2D.Box:
                    obj.AddComponent<BoxCollider2D>();
                    return;
                case ColliderType2D.Capsule:
                    obj.AddComponent<CapsuleCollider2D>();
                    return;
                case ColliderType2D.Circle:
                    obj.AddComponent<CapsuleCollider2D>();
                    return;
            }
        }
        else if (viewType == ViewSetting.View3D)
        {
            switch (((DefaultPrefabInfo3D)info).ColliderType)
            {
                case ColliderType3D.None:
                    return;
                case ColliderType3D.Box:
                    obj.AddComponent<BoxCollider>();
                    return;
                case ColliderType3D.Capsule:
                    obj.AddComponent<CapsuleCollider>();
                    return;
                case ColliderType3D.Sphere:
                    obj.AddComponent<SphereCollider>();
                    return;
            }
        }
    }
}
