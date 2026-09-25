using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorEditor
{
    public class ItemEditor : EditorWindow
    {
        private TextAsset csvFile;
        private DefaultAsset imageFolder;

        private List<Item> items = new();

        private ListView itemList;
        private ScrollView rightPane;

        private Label statusLabel;

        private Item selectedItem;

        private const string ItemFolder = "Assets/Resources/Cook/Items";

        [MenuItem("Tools/Item Editor")]
        public static void ShowEditor()
        {
            var window = GetWindow<ItemEditor>();

            window.titleContent = new GUIContent("Item Editor");
            window.minSize = new Vector2(800, 500);
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();

            CreateToolbar();
            CreateMainUI();

            RefreshItemList();
        }

        private void CreateToolbar()
        {
            var toolbar = new Toolbar();
        
            var csvField = new ObjectField("CSV")
            {
                objectType = typeof(TextAsset),
                value = csvFile,
                style =
                {
                    minWidth = 300
                }
            };

            csvField.RegisterValueChangedCallback(evt =>
            {
                csvFile = evt.newValue as TextAsset;
            });

            toolbar.Add(csvField);

            var imageFolderField = new ObjectField("Image Folder")
            {
                objectType = typeof(DefaultAsset),
                value = imageFolder
            };

            imageFolderField.RegisterValueChangedCallback(evt =>
            {
                imageFolder = evt.newValue as DefaultAsset;
            });

            toolbar.Add(imageFolderField);
            
        
            var importButton = new Button(ImportItems)
            {
                text = "Import CSV + Images"
            };

            toolbar.Add(importButton);
        
            var refreshButton = new Button(RefreshItemList)
            {
                text = "Refresh"
            };

            toolbar.Add(refreshButton);

            rootVisualElement.Add(toolbar);
        }

        private void CreateMainUI()
        {
            var splitView = new TwoPaneSplitView(
                0,
                250,
                TwoPaneSplitViewOrientation.Horizontal
            );

            rootVisualElement.Add(splitView);
        
            var leftPane = new VisualElement
            {
                style =
                {
                    flexGrow = 1
                }
            };

            itemList = new ListView
            {
                selectionType = SelectionType.Single,
                fixedItemHeight = 24,
                style =
                {
                    flexGrow = 1
                },
                makeItem = () => new Label(),
                bindItem = (element, index) =>
                {
                    if (index < 0 || index >= items.Count)
                        return;

                    var item = items[index];

                    var label = (Label)element;

                    label.text = item != null
                        ? $"{item.ItemID}  |  {item.ItemName}"
                        : "NULL";
                }
            };

            itemList.selectionChanged += OnItemSelected;

            leftPane.Add(itemList);

            splitView.Add(leftPane);
        
            rightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal)
            {
                style =
                {
                    flexGrow = 1
                }
            };

            splitView.Add(rightPane);
        
            statusLabel = new Label
            {
                style =
                {
                    paddingLeft = 5,
                    paddingTop = 5,
                    paddingBottom = 5
                }
            };

            rootVisualElement.Add(statusLabel);
        }

        private void RefreshItemList()
        {
            items.Clear();

            var folderPath = ItemFolder;

            if (string.IsNullOrEmpty(folderPath))
            {
                itemList?.Rebuild();
                return;
            }

            var guids = AssetDatabase.FindAssets(
                "t:Item",
                new[] { folderPath }
            );

            foreach (string guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                var item = AssetDatabase.LoadAssetAtPath<Item>(path);

                if (item != null)
                {
                    items.Add(item);
                }
            }

            items = items
                .OrderBy(x => x.ItemID)
                .ToList();

            itemList.itemsSource = items;
            itemList.Rebuild();

            statusLabel.text = $"Items: {items.Count}";
        }

        private void OnItemSelected(IEnumerable<object> selection)
        {
            selectedItem = selection
                .OfType<Item>()
                .FirstOrDefault();

            DrawItemInspector(selectedItem);
        }

        private void DrawItemInspector(Item item)
        {
            rightPane.Clear();

            if (item == null)
                return;

            var title = new Label(item.ItemID)
            {
                style =
                {
                    fontSize = 18,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 10
                }
            };

            rightPane.Add(title);

            var serializedItem = new SerializedObject(item);

            serializedItem.Update();

            var iterator = serializedItem.GetIterator();

            var enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.propertyPath == "m_Script")
                    continue;

                var field = new PropertyField(iterator.Copy());

                rightPane.Add(field);
            }

            rightPane.Bind(serializedItem);

            var path = AssetDatabase.GetAssetPath(item);

            var pathLabel = new Label($"Asset: {path}")
            {
                style =
                {
                    marginTop = 10
                }
            };

            rightPane.Add(pathLabel);
        }

        private void ImportItems()
        {
            if (csvFile == null)
            {
                EditorUtility.DisplayDialog(
                    "Item Import",
                    "CSV 파일을 지정해주세요.",
                    "OK"
                );

                return;
            }

            if (string.IsNullOrEmpty(ItemFolder))
            {
                EnsureFolderExists(ItemFolder);
            }

            var imageFolderPath = GetAssetFolderPath(imageFolder);

            if (string.IsNullOrEmpty(imageFolderPath))
            {
                EditorUtility.DisplayDialog(
                    "Item Import",
                    "Image Folder를 지정해주세요.",
                    "OK"
                );

                return;
            }

            List<CsvItemData> csvItems;

            try
            {
                csvItems = ParseCSV(csvFile.text);
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

            var createdCount = 0;
            var updatedCount = 0;
            var imageAssignedCount = 0;
            var imageMissingCount = 0;
            var errorCount = 0;

            var errors = new List<string>();

            AssetDatabase.StartAssetEditing();

            try
            {
                foreach (var data in csvItems)
                {
                    if (string.IsNullOrWhiteSpace(data.ItemID))
                    {
                        errors.Add("ItemID가 비어있는 행이 있습니다.");
                        errorCount++;
                        continue;
                    }

                    var item = FindItemByID(
                        data.ItemID,
                        ItemFolder
                    );

                    var isNew = item == null;

                    if (isNew)
                    {
                        item = CreateInstance<Item>();
                        item.ItemID = data.ItemID;

                        var assetPath =
                            $"{ItemFolder}/{SanitizeFileName(data.ItemID)}.asset";

                        assetPath =
                            AssetDatabase.GenerateUniqueAssetPath(assetPath);

                        AssetDatabase.CreateAsset(item, assetPath);

                        createdCount++;
                    }
                    else
                    {
                        updatedCount++;
                    }

                    ApplyCSVData(item, data);
                
                    var sprite = FindSpriteByName(
                        imageFolderPath,
                        data.ItemName,
                        int.Parse(data.ItemGrade)
                    );

                    if (sprite != null)
                    {
                        item.ItemSprite = sprite;
                        imageAssignedCount++;
                    }
                    else
                    {
                        imageMissingCount++;

                        errors.Add(
                            $"[{data.ItemID}] 이미지 없음"
                        );
                    }

                    EditorUtility.SetDirty(item);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            RefreshItemList();

            statusLabel.text =
                $"Created: {createdCount} | " +
                $"Updated: {updatedCount} | " +
                $"Images: {imageAssignedCount} | " +
                $"Missing Images: {imageMissingCount} | " +
                $"Errors: {errorCount}";

            Debug.Log(
                $"Item Import Complete\n" +
                $"Created: {createdCount}\n" +
                $"Updated: {updatedCount}\n" +
                $"Images Assigned: {imageAssignedCount}\n" +
                $"Missing Images: {imageMissingCount}\n" +
                $"Errors: {errorCount}"
            );

            if (errors.Count > 0)
            {
                Debug.LogWarning(
                    "Item Import Errors:\n" +
                    string.Join("\n", errors)
                );
            }

            EditorUtility.DisplayDialog(
                "Item Import Complete",
                $"Created : {createdCount}\n" +
                $"Updated : {updatedCount}\n" +
                $"Images : {imageAssignedCount}\n" +
                $"Missing Images : {imageMissingCount}\n" +
                $"Errors : {errorCount}",
                "OK"
            );
        }

        private void ApplyCSVData(
            Item item,
            CsvItemData data
        )
        {
            item.ItemID = data.ItemID;
            item.ItemName = data.ItemName;

            if (!Enum.TryParse(
                    data.ItemType,
                    true,
                    out ItemType itemType))
            {
                Debug.LogWarning(
                    $"[{data.ItemID}] ItemType 변환 실패: {data.ItemType}"
                );
            }
            else
            {
                item.ItemType = itemType;
            }

            if (!Enum.TryParse(
                    data.ItemMainCategory,
                    true,
                    out ItemMainCategory mainCategory))
            {
                Debug.LogWarning(
                    $"[{data.ItemID}] ItemMainCategory 변환 실패: {data.ItemMainCategory}"
                );
            }
            else
            {
                item.ItemMainCategory = mainCategory;
            }

            if (!Enum.TryParse(
                    data.ItemSubCategory,
                    true,
                    out ItemSubCategory subCategory))
            {
                Debug.LogWarning(
                    $"[{data.ItemID}] ItemSubCategory 변환 실패: {data.ItemSubCategory}"
                );
            }
            else
            {
                item.ItemSubCategory = subCategory;
            }

            if (!Enum.TryParse(
                    data.ItemGrade,
                    true,
                    out ItemGrade grade))
            {
                Debug.LogWarning(
                    $"[{data.ItemID}] ItemGrade 변환 실패: {data.ItemGrade}"
                );
            }
            else
            {
                item.ItemGrade = grade;
            }

            if (string.IsNullOrWhiteSpace(data.Foodtaste)) return;
            if (!Enum.TryParse(
                    data.Foodtaste,
                    true,
                    out FoodTaste foodTaste))
            {
                Debug.LogWarning(
                    $"[{data.ItemID}] FoodTaste 변환 실패: {data.Foodtaste}"
                );
            }
            else
            {
                item.Foodtaste = foodTaste;
            }
        }

        private Item FindItemByID(
            string itemID,
            string folderPath
        )
        {
            var guids = AssetDatabase.FindAssets(
                "t:Item",
                new[] { folderPath }
            );

            return guids.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Item>).FirstOrDefault(item => item != null && item.ItemID == itemID);
        }

        private Sprite FindSpriteByName(
            string folderPath,
            string itemName,
            int grade
        )
        {
            var targetName = $"{itemName}_{grade}";

            var guids = AssetDatabase.FindAssets(
                targetName,
                new[] { folderPath }
            );

            foreach (var guid in guids)
            {
                var path =
                    AssetDatabase.GUIDToAssetPath(guid);

                var fileName =
                    Path.GetFileNameWithoutExtension(path);
            
                if (!string.Equals(
                        fileName,
                        targetName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var sprite =
                    AssetDatabase.LoadAssetAtPath<Sprite>(path);

                if (sprite != null)
                    return sprite;
            
                var importer =
                    AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null) continue;
                importer.textureType =
                    TextureImporterType.Sprite;

                importer.spriteImportMode =
                    SpriteImportMode.Single;

                importer.SaveAndReimport();

                sprite =
                    AssetDatabase.LoadAssetAtPath<Sprite>(path);

                if (sprite != null)
                    return sprite;
            }

            return null;
        }

        private List<CsvItemData> ParseCSV(string csv)
        {
            var result = new List<CsvItemData>();

            var rows = ParseCSVRows(csv);

            if (rows.Count < 2)
                throw new Exception("CSV에 데이터가 없습니다.");

            var headers = rows[0];

            var headerMap =
                new Dictionary<string, int>(
                    StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < headers.Length; i++)
            {
                var header =
                    headers[i].Trim();

                headerMap.TryAdd(header, i);
            }

            string[] requiredHeaders =
            {
                "ItemID",
                "ItemName",
                "ItemType",
                "ItemMainCategory",
                "ItemSubCategory",
                "ItemGrade",
                "Foodtaste"
            };

            foreach (var header in requiredHeaders)
            {
                if (!headerMap.ContainsKey(header))
                {
                    throw new Exception(
                        $"CSV에 '{header}' 컬럼이 없습니다."
                    );
                }
            }

            for (var rowIndex = 1;
                 rowIndex < rows.Count;
                 rowIndex++)
            {
                var row = rows[rowIndex];

                if (row.Length == 0)
                    continue;

                result.Add(new CsvItemData
                {
                    ItemID = GetValue("ItemID"),
                    ItemName = GetValue("ItemName"),
                    ItemType = GetValue("ItemType"),
                    ItemMainCategory =
                        GetValue("ItemMainCategory"),
                    ItemSubCategory =
                        GetValue("ItemSubCategory"),
                    ItemGrade = GetValue("ItemGrade"),
                    Foodtaste = GetValue("Foodtaste")
                });
                continue;

                string GetValue(string header)
                {
                    if (!headerMap.TryGetValue(
                            header,
                            out var index) || index >= row.Length)
                    {
                        return string.Empty;
                    }

                    return row[index].Trim();
                }
            }

            return result;
        }

        private List<string[]> ParseCSVRows(string csv)
        {
            var rows = new List<string[]>();

            var currentRow = new List<string>();
            var currentValue = new StringBuilder();

            var insideQuotes = false;

            for (var i = 0; i < csv.Length; i++)
            {
                var c = csv[i];

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

                        continue;
                    }
                    case ',' when !insideQuotes:
                        currentRow.Add(
                            currentValue.ToString());

                        currentValue.Clear();

                        continue;
                    case '\n' or '\r' when
                        !insideQuotes:
                    {
                        if (c == '\r' &&
                            i + 1 < csv.Length &&
                            csv[i + 1] == '\n')
                        {
                            i++;
                        }

                        currentRow.Add(
                            currentValue.ToString());

                        currentValue.Clear();

                        if (currentRow.Any(
                                x => !string.IsNullOrWhiteSpace(x)))
                        {
                            rows.Add(currentRow.ToArray());
                        }

                        currentRow.Clear();

                        continue;
                    }
                    default:
                        currentValue.Append(c);
                        break;
                }
            }

            if (currentValue.Length <= 0 &&
                currentRow.Count <= 0) return rows;
            currentRow.Add(
                currentValue.ToString());

            rows.Add(currentRow.ToArray());

            return rows;
        }

        private string GetAssetFolderPath(
            DefaultAsset folder)
        {
            if (folder == null)
                return null;

            var path =
                AssetDatabase.GetAssetPath(folder);

            return AssetDatabase.IsValidFolder(path) ? path : null;
        }

        private void EnsureFolderExists(
            string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
                return;

            var parts =
                folderPath.Split('/');

            var current = parts[0];

            for (var i = 1; i < parts.Length; i++)
            {
                var next =
                    $"{current}/{parts[i]}";

                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(
                        current,
                        parts[i]);
                }

                current = next;
            }
        }

        private string SanitizeFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '_'));
        }

        [Serializable]
        private class CsvItemData
        {
            public string ItemID;
            public string ItemName;
            public string ItemType;
            public string ItemMainCategory;
            public string ItemSubCategory;
            public string ItemGrade;
            public string Foodtaste;
        }
    }
}