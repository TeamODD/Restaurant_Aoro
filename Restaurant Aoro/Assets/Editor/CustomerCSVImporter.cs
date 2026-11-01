
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CustomerCSVImporter
{
    private const string OutputFolder = "Assets/Resources/Customers";

    [MenuItem("Tools/Import Customers CSV")]
    public static void Import()
    {
        string file = EditorUtility.OpenFilePanel(
            "Customer CSV 선택", Application.dataPath, "csv");

        if (string.IsNullOrEmpty(file))
            return;

        List<List<string>> rows;

        try
        {
            rows = ParseCsv(File.ReadAllText(file, Encoding.UTF8));
        }
        catch (Exception ex)
        {
            Debug.LogError($"CSV 읽기 실패: {ex.Message}");
            return;
        }

        if (rows.Count < 2)
        {
            Debug.LogWarning("CSV 데이터가 없습니다.");
            return;
        }

        var headers = rows[0]
            .Select(h => h.Trim().TrimStart('\uFEFF'))
            .ToList();

        if (!headers.Contains("CustomerID"))
        {
            Debug.LogError("CustomerID 열이 필요합니다.");
            return;
        }

        EnsureFolder(OutputFolder);

        var existing = AssetDatabase
            .FindAssets("t:Customer", new[] { OutputFolder })
            .Select(g => AssetDatabase.LoadAssetAtPath<Customer>(
                AssetDatabase.GUIDToAssetPath(g)))
            .Where(c => c != null && !string.IsNullOrWhiteSpace(c.CustomerID))
            .GroupBy(c => c.CustomerID)
            .ToDictionary(g => g.Key, g => g.First());

        int created = 0;
        int updated = 0;
        int failed = 0;

        var seen = new HashSet<string>();

        for (int r = 1; r < rows.Count; r++)
        {
            var values = rows[r];

            if (values.All(string.IsNullOrWhiteSpace))
                continue;

            var data = new Dictionary<string, string>();

            for (int c = 0; c < headers.Count; c++)
            {
                if (!string.IsNullOrEmpty(headers[c]))
                {
                    data[headers[c]] =
                        c < values.Count ? values[c].Trim() : "";
                }
            }

            string id = Get(data, "CustomerID");

            if (string.IsNullOrWhiteSpace(id) || !seen.Add(id))
            {
                Debug.LogError(
                    $"CSV {r + 1}행: CustomerID 누락 또는 중복 ({id})");

                failed++;
                continue;
            }

            bool isNew = !existing.TryGetValue(id, out Customer customer);

            if (isNew)
                customer = ScriptableObject.CreateInstance<Customer>();

            try
            {
                // 기존 에셋은 복제본에 적용하여 부분 갱신 방지
                Customer target = isNew
                    ? customer
                    : UnityEngine.Object.Instantiate(customer);

                try
                {
                    Apply(target, data);

                    if (isNew)
                    {
                        string name = Get(data, "CustomerName");

                        if (string.IsNullOrWhiteSpace(name))
                            throw new Exception($"CustomerName이 비어 있습니다: {id}");

                        string customerFolder = $"{OutputFolder}/{SafeFileName(name)}";

                        EnsureFolder(customerFolder);

                        string path = $"{customerFolder}/{SafeFileName(name)}.asset";

                        AssetDatabase.CreateAsset(target, path);
                        existing[id] = target;
                        created++;
                    }
                    else
                    {
                        Undo.RecordObject(customer, "Import Customer CSV");
                        EditorUtility.CopySerialized(target, customer);
                        EditorUtility.SetDirty(customer);
                        updated++;

                        UnityEngine.Object.DestroyImmediate(target);
                    }
                }
                catch
                {
                    if (target != null && target != customer)
                        UnityEngine.Object.DestroyImmediate(target);

                    throw;
                }
            }
            catch (Exception ex)
            {
                if (isNew && customer != null &&
                    !AssetDatabase.Contains(customer))
                {
                    UnityEngine.Object.DestroyImmediate(customer);
                }

                Debug.LogError(
                    $"CSV {r + 1}행 ({id}): {ex.Message}");

                failed++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"Customer CSV 완료: 생성 {created}, 갱신 {updated}, 실패 {failed}");
    }

    private static void Apply(Customer c, Dictionary<string, string> d)
    {
        c.CustomerID = Get(d, "CustomerID");

        Set(d, "CustomerName", v => c.CustomerName = v);
        Set(d, "tribe", v => c.tribe = EnumValue<TribeType>(v));
        Set(d, "NPCType", v => c.NPCType = EnumValue<NPCType>(v));

        Set(d, "appearStartHour",
            v => c.appearStartHour = float.Parse(
                v, CultureInfo.InvariantCulture));

        Set(d, "appearEndHour",
            v => c.appearEndHour = float.Parse(
                v, CultureInfo.InvariantCulture));

        Set(d, "codexDescription",
            v => c.codexDescription = Unescape(v));

        Set(d, "codexDetailDescription",
            v => c.codexDetailDescription = Unescape(v));

        Set(d, "favoriteTastes",
            v => c.favoriteTastes =
                Parts(v).Select(EnumValue<FoodTaste>).ToList());

        Set(d, "dislikedTastes",
            v => c.dislikedTastes =
                Parts(v).Select(EnumValue<FoodTaste>).ToList());

        Set(d, "favoriteFoods",
            v => c.favoriteFoods =
                Parts(v).Select(EnumValue<ItemMainCategory>).ToList());

        Set(d, "dislikedFoods",
            v => c.dislikedFoods =
                Parts(v).Select(EnumValue<ItemMainCategory>).ToList());

        Set(d, "payable",
            v => c.payable = Parts(v)
                .Select(x => int.Parse(x, CultureInfo.InvariantCulture))
                .ToList());

        Set(d, "payItem",
            v => c.payItem = Parts(v).Select(FindItem).ToList());

        Set(d, "codexIcon",
            v => c.codexIcon = FindAsset<Sprite>(v));

        Set(d, "prefabStand",
            v => c.prefabStand = FindAsset<GameObject>(v));

        Set(d, "prefabSeated",
            v => c.prefabSeated = FindAsset<GameObject>(v));

        Set(d, "prefabEating",
            v => c.prefabEating = FindAsset<GameObject>(v));

        Set(d, "prefabLeft",
            v => c.prefabLeft = FindAsset<GameObject>(v));

        c.standStates = ApplyStates(c.standStates, d, "standStates");
        c.seatedStates = ApplyStates(c.seatedStates, d, "seatedStates");
        c.eatingStates = ApplyStates(c.eatingStates, d, "eatingStates");
        c.leftStates = ApplyStates(c.leftStates, d, "leftStates");
        c.rightStates = ApplyStates(c.rightStates, d, "rightStates");

        Set(d, "greetingLines",
            v => c.greetingLines = Parts(v).Select(Unescape).ToList());

        Set(d, "orderLines",
            v => c.orderLines = Parts(v).Select(Unescape).ToList());

        if (c.resultBuckets == null)
            c.resultBuckets = new List<ResultBucket>();

        foreach (ResultType type in Enum.GetValues(typeof(ResultType)))
        {
            string key = "result." + type;

            if (!d.TryGetValue(key, out string value) ||
                string.IsNullOrWhiteSpace(value))
                continue;

            var bucket = c.resultBuckets
                .FirstOrDefault(b => b.type == type);

            if (bucket == null)
            {
                bucket = new ResultBucket { type = type };
                c.resultBuckets.Add(bucket);
            }

            bucket.lines = Parts(value)
                .Select(Unescape)
                .ToList();
        }
    }

    private static VariantStates ApplyStates(
        VariantStates s,
        Dictionary<string, string> d,
        string prefix)
    {
        if (s == null)
            s = new VariantStates();

        Set(d, prefix + ".baseState",
            v => s.baseState = v);

        Set(d, prefix + ".variants",
            v => s.variants = Parts(v));

        return s;
    }

    // 빈 셀은 기존 값을 유지
    // 목록을 비우려면 <clear> 입력
    private static void Set(
        Dictionary<string, string> d,
        string key,
        Action<string> assign)
    {
        if (!d.TryGetValue(key, out string v) ||
            string.IsNullOrWhiteSpace(v))
            return;

        assign(v == "<clear>" ? "" : v);
    }

    private static string Get(
        Dictionary<string, string> d,
        string key)
    {
        return d.TryGetValue(key, out string v) ? v : "";
    }

    private static List<string> Parts(string value)
    {
        return value.Split(';')
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .ToList();
    }

    private static string Unescape(string value)
    {
        return value.Replace("\\n", "\n");
    }

    private static T EnumValue<T>(string value) where T : struct
    {
        if (!Enum.TryParse(value, true, out T result) ||
            !Enum.IsDefined(typeof(T), result))
        {
            throw new Exception(
                $"{typeof(T).Name} 값이 잘못되었습니다: {value}");
        }

        return result;
    }

    private static T FindAsset<T>(string key)
        where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(key))
            return null;

        string filter = typeof(T) == typeof(GameObject)
            ? $"{key} t:Prefab"
            : $"{key} t:{typeof(T).Name}";

        string[] guids;

        // lk 폴더 안에서만 검색
        if (typeof(T) == typeof(GameObject))
        {
            guids = AssetDatabase.FindAssets(
                filter,
                new[] { "Assets/Resources/Customers/lk" }
            );
        }
        else
        {
            guids = AssetDatabase.FindAssets(filter);
        }

        var matches = guids
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Where(p =>
                Path.GetFileNameWithoutExtension(p) == key &&
                (typeof(T) != typeof(GameObject) ||
                Path.GetExtension(p).Equals(
                    ".prefab", StringComparison.OrdinalIgnoreCase)))
            .Select(p => AssetDatabase.LoadAssetAtPath<T>(p))
            .Where(asset => asset != null)
            .ToList();

        if (matches.Count != 1)
        {
            throw new Exception(
                $"{typeof(T).Name} 검색 결과가 {matches.Count}개입니다: {key}");
        }

        return matches[0];
    }

    private static Item FindItem(string name)
    {
        var matches = AssetDatabase.FindAssets("t:Item")
            .Select(g => AssetDatabase.LoadAssetAtPath<Item>(
                AssetDatabase.GUIDToAssetPath(g)))
            .Where(i => i != null && i.ItemName == name)
            .ToList();

        if (matches.Count != 1)
        {
            throw new Exception(
                $"ItemID 검색 결과가 {matches.Count}개입니다: {name}");
        }

        return matches[0];
    }

    private static string SafeFileName(string name)
    {
        foreach (char ch in Path.GetInvalidFileNameChars())
            name = name.Replace(ch, '_');

        return name.Replace('/', '_').Replace('\\', '_');
    }

    private static void EnsureFolder(string path)
    {
        string current = "Assets";

        foreach (string part in path
            .Substring("Assets/".Length).Split('/'))
        {
            string next = current + "/" + part;

            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, part);

            current = next;
        }
    }

    // 따옴표, 쉼표, 셀 내부 줄바꿈 지원
    private static List<List<string>> ParseCsv(string text)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var field = new StringBuilder();

        bool quoted = false;

        for (int i = 0; i < text.Length; i++)
        {
            char ch = text[i];

            if (ch == '"')
            {
                if (quoted && i + 1 < text.Length &&
                    text[i + 1] == '"')
                {
                    field.Append('"');
                    i++;
                }
                else
                {
                    quoted = !quoted;
                }
            }
            else if (ch == ',' && !quoted)
            {
                row.Add(field.ToString());
                field.Clear();
            }
            else if ((ch == '\r' || ch == '\n') && !quoted)
            {
                if (ch == '\r' && i + 1 < text.Length &&
                    text[i + 1] == '\n')
                    i++;

                row.Add(field.ToString());
                field.Clear();

                rows.Add(row);
                row = new List<string>();
            }
            else
            {
                field.Append(ch);
            }
        }

        if (quoted)
            throw new Exception("닫히지 않은 CSV 따옴표가 있습니다.");

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row);
        }

        return rows;
    }
}
#endif
