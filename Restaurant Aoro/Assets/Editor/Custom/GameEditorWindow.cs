using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GameEditorWindow : EditorWindow
{
    private GameGraphView graphView;

    [MenuItem("Window/Game Graph")]
    public static void Open()
    {
        GetWindow<GameEditorWindow>("Game Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void ConstructGraphView()
    {
        graphView = new GameGraphView();
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        Button addSlotButton = new Button(() =>
        {
            graphView.CreateSlotNode(new Vector2(20, 20)); // 원하는 초기 위치
        })
        {
            text = "Add Slot"
        };

        toolbar.Add(addSlotButton);
        rootVisualElement.Add(toolbar);
    }
}
