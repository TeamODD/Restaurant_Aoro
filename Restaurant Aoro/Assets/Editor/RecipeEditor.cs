using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Scriptable_Object; // 사용자의 네임스페이스

public class RecipeEditor : EditorWindow
{
    private CookRulesSO currentDatabase;
    private SerializedObject serializedDatabase;
    
    private TwoPaneSplitView splitView;
    private ListView leftList;
    private ScrollView rightPane;

    [MenuItem("Tools/Recipe Editor")]
    public static void ShowEditor()
    {
        EditorWindow wnd = GetWindow<RecipeEditor>();
        wnd.titleContent = new GUIContent("Recipe Editor");
        wnd.minSize = new Vector2(450, 200);
    }

    public void CreateGUI()
    {
        var toolbar = new Toolbar();
        rootVisualElement.Add(toolbar);

        var databaseField = new ObjectField("CookRules DB")
        {
            objectType = typeof(CookRulesSO),
            allowSceneObjects = false,
            value = currentDatabase
        };
        
        databaseField.RegisterValueChangedCallback(evt =>
        {
            currentDatabase = evt.newValue as CookRulesSO;
            RefreshUI();
        });
        toolbar.Add(databaseField);
        
        splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        rootVisualElement.Add(splitView);
        
        var leftPaneContainer = new VisualElement();
        splitView.Add(leftPaneContainer);

        leftList = new ListView
        {
            style =
            {
                flexGrow = 1
            }
        };
        leftPaneContainer.Add(leftList);
        
        var buttonGroup = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                height = 25
            }
        };

        var addButton = new Button(CreateNewRule) { text = "+", style = { flexGrow = 1 } };
        var removeButton = new Button(RemoveSelectedRule) { text = "-", style = { flexGrow = 1 } };
        
        buttonGroup.Add(addButton);
        buttonGroup.Add(removeButton);
        leftPaneContainer.Add(buttonGroup);
        
        rightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(rightPane);
        
        if (currentDatabase != null)
        {
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        leftList.itemsSource = null;
        leftList.Rebuild();
        rightPane.Clear();

        if (currentDatabase == null) return;

        serializedDatabase = new SerializedObject(currentDatabase);
        var rulesProperty = serializedDatabase.FindProperty("cookRules");
        
        leftList.makeItem = () => new Label();
        leftList.bindItem = (element, i) =>
        {
            var prop = rulesProperty.GetArrayElementAtIndex(i);
            var rule = prop.objectReferenceValue as CookRule;
            ((Label)element).text = rule != null ? rule.name : "Null Rule";
        };

        leftList.itemsSource = currentDatabase.cookRules;
        leftList.selectionType = SelectionType.Single;
        leftList.fixedItemHeight = 25;
        
        leftList.selectionChanged += (items) =>
        {
            var enumerable = items as object[] ?? items.ToArray();
            if (enumerable.Any() && enumerable.First() is CookRule selectedRule)
            {
                DrawRightInspector(selectedRule);
            }
            else
            {
                rightPane.Clear();
            }
        };
        
        leftList.Rebuild();
    }

    private void DrawRightInspector(CookRule rule)
    {
        rightPane.Clear();
        if (rule == null) return;
        
        var nameField = new TextField("Rule Name")
        {
            value = rule.name,
            isDelayed = true,
            style =
            {
                marginBottom = 10,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };

        nameField.RegisterValueChangedCallback(evt =>
        {
            var newName = evt.newValue;
            if (string.IsNullOrEmpty(newName)) return;

            var assetPath = AssetDatabase.GetAssetPath(rule);
            var result = AssetDatabase.RenameAsset(assetPath, newName);
            
            if (string.IsNullOrEmpty(result))
            {
                AssetDatabase.SaveAssets();
                leftList.Rebuild();
            }
            else
            {
                nameField.value = rule.name; 
            }
        });

        rightPane.Add(nameField);
        
        var serializedRule = new SerializedObject(rule);
        var iterator = serializedRule.GetIterator();
        iterator.NextVisible(true);
        
        while (iterator.NextVisible(false))
        {
            var propField = new PropertyField(iterator);
            propField.Bind(serializedRule);
            rightPane.Add(propField);
        }
    }

    private void CreateNewRule()
    {
        if (currentDatabase == null)
        {
            Debug.LogWarning("먼저 CookRulesSO 데이터베이스를 선택해주세요.");
            return;
        }
        
        CookRule newRule = CreateInstance<CookRule>();
        newRule.name = "New CookRule";

        string path = AssetDatabase.GetAssetPath(currentDatabase);
        string folder = System.IO.Path.GetDirectoryName(path);
        string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{folder}/NewCookRule.asset");

        AssetDatabase.CreateAsset(newRule, uniquePath);
        AssetDatabase.SaveAssets();
        
        serializedDatabase.Update();
        var rulesProperty = serializedDatabase.FindProperty("cookRules");
        rulesProperty.arraySize++;
        rulesProperty.GetArrayElementAtIndex(rulesProperty.arraySize - 1).objectReferenceValue = newRule;
        serializedDatabase.ApplyModifiedProperties();
        
        RefreshUI();
        leftList.SetSelection(rulesProperty.arraySize - 1);
    }

    private void RemoveSelectedRule()
    {
        if (leftList.selectedIndex == -1 || currentDatabase == null) return;
        
        int index = leftList.selectedIndex;
        serializedDatabase.Update();
        var rulesProperty = serializedDatabase.FindProperty("cookRules");
        
        if (rulesProperty.GetArrayElementAtIndex(index).objectReferenceValue != null)
        {
            rulesProperty.DeleteArrayElementAtIndex(index); 
        }
        
        rulesProperty.DeleteArrayElementAtIndex(index);
        serializedDatabase.ApplyModifiedProperties();
        RefreshUI();
        rightPane.Clear();
    }
}