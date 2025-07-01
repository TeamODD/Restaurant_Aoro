using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;

public class GameGraphView : GraphView
{
    public GameGraphView()
    {
        AddGridBackground();
        AddManipulators();
    }

    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();

        gridBackground.StretchToParentSize();

        Insert(0, gridBackground);
    }

    private void AddManipulators()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    public void CreateSlotNode(Vector2 position)
    {
        var slotNode = new SlotNodeView();
        slotNode.SetPosition(new Rect(position, new Vector2(300, 400)));
        AddElement(slotNode);
    }
}
