using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Customer))]
public class CustomerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Customer customer = (Customer)target;

        // 손님 정보
        EditorGUILayout.LabelField("손님 정보", EditorStyles.boldLabel);
        Draw("CustomerID");
        Draw("CustomerName");
        Draw("tribe");
        Draw("NPCType");
        Draw("appearStartHour");
        Draw("appearEndHour");
        Draw("codexDescription");

        // 음식 취향
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("음식 취향", EditorStyles.boldLabel);
        Draw("favoriteTastes", true);
        Draw("dislikedTastes", true);
        Draw("favoriteFoods", true);
        Draw("dislikedFoods", true);

        // 결제 정보
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("결제 정보", EditorStyles.boldLabel);
        if (customer.tribe == TribeType.Human)
            Draw("payable", true);
        else
            Draw("payItem", true);

        // IK 프리팹
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("IK Motion Prefabs", EditorStyles.boldLabel);
        Draw("prefabStand");
        Draw("prefabLeft");
        Draw("prefabRight");
        Draw("prefabSeated");
        Draw("prefabEating");

        // Animator State Variants
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Animator State Variants", EditorStyles.boldLabel);
        Draw("standStates", true);
        Draw("leftStates", true);
        Draw("rightStates", true);
        Draw("seatedStates", true);
        Draw("eatingStates", true);

        // 대사
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("대사 설정", EditorStyles.boldLabel);
        Draw("greetingLines", true);
        Draw("orderLines", true);
        Draw("resultBuckets", true);

        serializedObject.ApplyModifiedProperties();
    }

    private void Draw(string name, bool includeChildren = false)
    {
        var prop = serializedObject.FindProperty(name);
        if (prop != null)
            EditorGUILayout.PropertyField(prop, includeChildren);
    }
}