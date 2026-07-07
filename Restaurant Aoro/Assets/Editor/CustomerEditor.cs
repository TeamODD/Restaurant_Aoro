using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Customer))]
public class CustomerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Customer customer = (Customer)target;

        // �մ� ����
        EditorGUILayout.LabelField("�մ� ����", EditorStyles.boldLabel);
        Draw("CustomerID");
        Draw("CustomerName");
        Draw("tribe");
        Draw("NPCType");
        Draw("appearStartHour");
        Draw("appearEndHour");
        Draw("codexDescription");
        Draw("codexDetailDescription");

        // ���� ����
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("���� ����", EditorStyles.boldLabel);
        Draw("favoriteTastes", true);
        Draw("dislikedTastes", true);
        Draw("favoriteFoods", true);
        Draw("dislikedFoods", true);

        // ���� ����
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("���� ����", EditorStyles.boldLabel);
        if (customer.tribe == TribeType.Human)
            Draw("payable", true);
        else
            Draw("payItem", true);

        // IK ������
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

        // ���
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("��� ����", EditorStyles.boldLabel);
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