using UnityEngine;
using static Define;

public class DefaultPrefabInfo : MonoBehaviour
{
    [HideInInspector] public string FileName;
    [HideInInspector] public ViewSetting ViewType;
    [HideInInspector] public GameObject VisualPrefab;
    [HideInInspector] public AIType AIType;
}
