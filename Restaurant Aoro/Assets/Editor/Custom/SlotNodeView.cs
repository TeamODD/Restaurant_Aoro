using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SlotNodeView : UnityEditor.Experimental.GraphView.Node
{
    private ObjectField soField;
    private VisualElement soFieldContainer;
    private ScriptableObject currentSO;

    public SlotNodeView()
    {
        title = "Slot";

        inputContainer.style.display = DisplayStyle.None;
        outputContainer.style.display = DisplayStyle.None;

        var memoField = new TextField("Memo")
        {
            multiline = true,
            value = "", // �ʱⰪ
        };
        memoField.style.marginBottom = 4;
        mainContainer.Add(memoField);

        // Asset �ʵ�
        soField = new ObjectField("Asset")
        {
            objectType = typeof(ScriptableObject),
            allowSceneObjects = false
        };
        soField.RegisterValueChangedCallback(OnSOAssigned);
        mainContainer.Add(soField);

        // �ʵ� ������ ����
        soFieldContainer = new VisualElement();
        mainContainer.Add(soFieldContainer);
    }

    private void OnSOAssigned(ChangeEvent<UnityEngine.Object> evt)
    {
        currentSO = evt.newValue as ScriptableObject;
        soFieldContainer.Clear();

        if (currentSO != null)
        {
            SerializedObject serialized = new SerializedObject(currentSO);
            SerializedProperty prop = serialized.GetIterator();

            if (prop.NextVisible(true)) // ù visible property
            {
                while (prop.NextVisible(false))
                {
                    PropertyField field = new PropertyField(prop.Copy());
                    field.Bind(serialized);
                    soFieldContainer.Add(field);
                }
            }
        }
    }
}
