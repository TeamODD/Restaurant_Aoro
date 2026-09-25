using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Scriptable_Object;

namespace EditorEditor
{
    public class RecipeEditor : EditorWindow
    {
        private class CsvRecipeData
        {
            public string RecipeID;
            public string ItemID;
        }

        private CookRulesSO currentDatabase;
        private SerializedObject serializedDatabase;
        private TwoPaneSplitView splitView;
        private ListView leftList;
        private ScrollView rightPane;
        private TextAsset csvFile;

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

            var csvField = new ObjectField("Recipe CSV")
            {
                objectType = typeof(TextAsset),
                allowSceneObjects = false,
                value = csvFile
            };

            csvField.RegisterValueChangedCallback(evt =>
            {
                csvFile = evt.newValue as TextAsset;
            });

            toolbar.Add(csvField);

            var importButton = new Button(ImportRecipeCSV)
            {
                text = "Import Recipe CSV"
            };

            toolbar.Add(importButton);

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
                if (string.IsNullOrEmpty(newName) || newName == rule.name) return;

                var assetPath = AssetDatabase.GetAssetPath(rule);
                var error = AssetDatabase.RenameAsset(assetPath, newName);

                if (string.IsNullOrEmpty(error))
                {
                    var newPath = AssetDatabase.GetAssetPath(rule);
                    var finalName = System.IO.Path.GetFileNameWithoutExtension(newPath);

                    rule.name = finalName;
                    EditorUtility.SetDirty(rule);
                    AssetDatabase.SaveAssets();
                    nameField.value = finalName;
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

        private void ImportRecipeCSV()
        {
            if (currentDatabase == null)
            {
                EditorUtility.DisplayDialog(
                    "Recipe Import",
                    "CookRules DB를 지정해주세요.",
                    "OK"
                );

                return;
            }

            if (csvFile == null)
            {
                EditorUtility.DisplayDialog(
                    "Recipe Import",
                    "Recipe CSV를 지정해주세요.",
                    "OK"
                );

                return;
            }

            List<CsvRecipeData> csvData;

            try
            {
                csvData = ParseRecipeCSV(csvFile.text);
            }
            catch (Exception e)
            {
                Debug.LogError(e);

                EditorUtility.DisplayDialog(
                    "CSV Error",
                    e.Message,
                    "OK"
                );

                return;
            }

            serializedDatabase.Update();

            var rulesProperty =
                serializedDatabase.FindProperty("cookRules");

            var groupedRecipes =
                csvData
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.RecipeID) &&
                        !string.IsNullOrWhiteSpace(x.ItemID))
                    .GroupBy(x => x.RecipeID);

            foreach (var group in groupedRecipes)
            {
                CookRule rule = FindCookRule(group.Key);

                if (rule == null)
                {
                    rule = CreateCookRule(group.Key);

                    rulesProperty.arraySize++;

                    rulesProperty
                        .GetArrayElementAtIndex(rulesProperty.arraySize - 1)
                        .objectReferenceValue = rule;
                }

                var ingredients = new List<Item>();

                foreach (var row in group)
                {
                    var item = FindItemByID(row.ItemID);

                    if (item == null)
                    {
                        Debug.LogWarning(
                            $"[{group.Key}] ItemID를 찾을 수 없습니다: {row.ItemID}"
                        );

                        continue;
                    }

                    if (!ingredients.Contains(item))
                    {
                        ingredients.Add(item);
                    }
                }

                rule.ingredients = ingredients.ToArray();

                EditorUtility.SetDirty(rule);
            }

            serializedDatabase.ApplyModifiedProperties();

            EditorUtility.SetDirty(currentDatabase);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            RefreshUI();

            EditorUtility.DisplayDialog(
                "Recipe Import Complete",
                $"Recipe {groupedRecipes.Count()}개를 Import했습니다.",
                "OK"
            );
        }

        private List<CsvRecipeData> ParseRecipeCSV(string csv)
        {
            var rows = ParseCSVRows(csv);

            if (rows.Count == 0)
                throw new Exception("CSV가 비어있습니다.");

            var headers = rows[0];

            var recipeIDIndex = System.Array.IndexOf(
                headers,
                "RecipeID"
            );

            var itemIDIndex = System.Array.IndexOf(
                headers,
                "ItemID"
            );

            if (recipeIDIndex == -1)
                throw new Exception("CSV에 'RecipeID' 컬럼이 없습니다.");

            if (itemIDIndex == -1)
                throw new Exception("CSV에 'ItemID' 컬럼이 없습니다.");

            var result = new List<CsvRecipeData>();

            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];

                if (row.Length == 0)
                    continue;

                var recipeID =
                    recipeIDIndex < row.Length
                        ? row[recipeIDIndex].Trim()
                        : string.Empty;

                var itemID =
                    itemIDIndex < row.Length
                        ? row[itemIDIndex].Trim()
                        : string.Empty;

                result.Add(new CsvRecipeData
                {
                    RecipeID = recipeID,
                    ItemID = itemID
                });
            }

            return result;
        }

        private List<string[]> ParseCSVRows(string csv)
        {
            var rows = new List<string[]>();

            var currentRow = new List<string>();
            var currentValue = new StringBuilder();

            bool insideQuotes = false;

            for (int i = 0; i < csv.Length; i++)
            {
                char c = csv[i];

                switch (c)
                {
                    case '"':
                    {
                        if (insideQuotes &&
                            i + 1 < csv.Length &&
                            csv[i + 1] == '"')
                        {
                            currentValue.Append('"');
                            i++;
                        }
                        else
                        {
                            insideQuotes = !insideQuotes;
                        }

                        break;
                    }

                    case ',':
                    {
                        if (insideQuotes)
                        {
                            currentValue.Append(c);
                        }
                        else
                        {
                            currentRow.Add(currentValue.ToString());
                            currentValue.Clear();
                        }

                        break;
                    }

                    case '\n':
                    {
                        if (insideQuotes)
                        {
                            currentValue.Append(c);
                        }
                        else
                        {
                            currentRow.Add(currentValue.ToString());
                            currentValue.Clear();

                            rows.Add(currentRow.ToArray());
                            currentRow.Clear();
                        }

                        break;
                    }

                    case '\r':
                        break;

                    default:
                        currentValue.Append(c);
                        break;
                }
            }

            if (currentValue.Length > 0 || currentRow.Count > 0)
            {
                currentRow.Add(currentValue.ToString());
                rows.Add(currentRow.ToArray());
            }

            return rows;
        }

        private Item FindItemByID(string itemID)
        {
            const string itemFolder =
                "Assets/Resources/Cook/Items";

            var guids = AssetDatabase.FindAssets(
                "t:Item",
                new[] { itemFolder }
            );

            foreach (var guid in guids)
            {
                var path =
                    AssetDatabase.GUIDToAssetPath(guid);

                var item =
                    AssetDatabase.LoadAssetAtPath<Item>(path);

                if (item != null &&
                    item.ItemID == itemID)
                {
                    return item;
                }
            }

            return null;
        }

        private CookRule FindCookRule(string recipeID)
        {
            var guids = AssetDatabase.FindAssets(
                "t:CookRule"
            );

            foreach (var guid in guids)
            {
                var path =
                    AssetDatabase.GUIDToAssetPath(guid);

                var rule =
                    AssetDatabase.LoadAssetAtPath<CookRule>(path);

                if (rule != null &&
                    rule.name == recipeID)
                {
                    return rule;
                }
            }

            return null;
        }

        private CookRule CreateCookRule(string recipeID)
        {
            var rule = CreateInstance<CookRule>();
            rule.name = recipeID;

            string databasePath =
                AssetDatabase.GetAssetPath(currentDatabase);

            string folder =
                Path.GetDirectoryName(databasePath);

            string assetPath =
                AssetDatabase.GenerateUniqueAssetPath(
                    $"{folder}/{recipeID}.asset"
                );

            AssetDatabase.CreateAsset(
                rule,
                assetPath
            );

            AssetDatabase.SaveAssets();

            return rule;
        }
    }
}