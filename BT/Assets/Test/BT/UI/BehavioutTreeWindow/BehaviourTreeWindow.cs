using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeWindow : EditorWindow
{
    private BehaviourTreeView _treeView;
    private InspectorView _inspectorView;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Editor/BehaviourTreeWindow")]
    public static void OpenWindow()
    {
        BehaviourTreeWindow wnd = GetWindow<BehaviourTreeWindow>();
        wnd.titleContent = new GUIContent("BehaviourTreeWindow");
        wnd.minSize = new Vector2(800, 600);
        wnd.maxSize = new Vector2(800, 600);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var labelFromUXML = m_VisualTreeAsset.Instantiate();
        labelFromUXML.style.flexGrow = 1;
        root.Add(labelFromUXML);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BT/UI/BehavioutTreeWindow/BehaviourTreeWindow.uss");
        root.styleSheets.Add(styleSheet);

        _treeView = root.Q<BehaviourTreeView>();
        _inspectorView = root.Q<InspectorView>();
        _treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    private void OnNodeSelectionChanged(NodeView node)
    {
        _inspectorView.UpdateSelection(node);
    }

    private void OnSelectionChange() {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        if(tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())){
            _treeView.PopulateView(tree);
        }
    }
}
