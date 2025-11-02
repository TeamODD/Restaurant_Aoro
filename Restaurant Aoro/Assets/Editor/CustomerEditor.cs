using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Customer))]
public class CustomerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Customer customer = (Customer)target;

        serializedObject.Update();

        EditorGUILayout.LabelField("손님 정보", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CustomerID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CustomerName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tribe"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("NPCType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("appearStartHour"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("appearEndHour"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("codexDescription"));

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("음식 취향", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("favoriteTastes"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dislikedTastes"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("favoriteFoods"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dislikedFoods"), true);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("결제 정보", EditorStyles.boldLabel);

        if (customer.tribe == TribeType.Human)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("payable"), true);
        }
        else if (customer.tribe == TribeType.Youkai)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("payItem"), true);
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("frontAnim"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftAnim"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightAnim"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("seatedAnim"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("eatingAnim"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("upAnim"));

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("대사 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("greetingLines"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("orderLines"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("resultBuckets"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
