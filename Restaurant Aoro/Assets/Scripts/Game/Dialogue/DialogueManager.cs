using System.Collections.Generic;
using UnityEngine;

public enum DialogueType { Greeting, Order, Result }

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [System.Serializable]
    private class Buckets
    {
        public Queue<string> greeting = new();
        public Queue<string> order = new();

        public Dictionary<ResultType, Queue<string>> result = new();
    }

    private readonly Dictionary<int, Buckets> store = new();

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private DialogueInputMode greetingMode = DialogueInputMode.Global;
    [SerializeField] private DialogueInputMode normalMode = DialogueInputMode.Blocker;

    private bool counterVisible = true;

    private CustomerManager pendingGreetingOwner;
    private List<string> pendingGreetingLines;
    private Transform pendingGreetingAnchor;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetCounterVisible(bool visible)
    {
        counterVisible = visible;
        TryFlushGreeting();
    }

    public void Register(CustomerManager cm, Customer data)
    {
        if (cm == null || data == null) return;
        int key = cm.GetInstanceID();

        var b = new Buckets();
        EnqueueAll(b.greeting, data.greetingLines);
        EnqueueAll(b.order, data.orderLines);

        b.result = new Dictionary<ResultType, Queue<string>>();
        if (data.resultBuckets != null)
        {
            foreach (var bucket in data.resultBuckets)
            {
                if (bucket == null) continue;
                if (!b.result.TryGetValue(bucket.type, out var q))
                {
                    q = new Queue<string>();
                    b.result[bucket.type] = q;
                }
                EnqueueAll(q, bucket.lines);
            }
        }

        store[key] = b;
    }

    public bool TryPresentNext(CustomerManager cm, DialogueType type, Transform anchor)
    {
        if (cm == null) return false;
        if (!store.TryGetValue(cm.GetInstanceID(), out var b)) return false;

        Queue<string> q = type switch
        {
            DialogueType.Greeting => b.greeting,
            DialogueType.Order => b.order,
            _ => null
        };
        if (q == null || q.Count == 0) return false;

        string line = q.Dequeue();
        PresentOne(line, cm, anchor);
        return true;
    }

    public void RequestGreeting(CustomerManager cm, IList<string> lines, Transform anchor)
    {
        if (cm == null || lines == null || lines.Count == 0) return;
        if (cm.HasGreeted()) return;   

        pendingGreetingOwner = cm;
        pendingGreetingLines = new List<string>(lines);
        pendingGreetingAnchor = anchor;

        TryFlushGreeting(); 
    }

    private void TryFlushGreeting()
    {
        if (!counterVisible) return;             
        if (dialogueUI == null) return;
        if (dialogueUI.IsShowing()) return;     
        if (pendingGreetingOwner == null) return;  
        if (pendingGreetingOwner.HasGreeted())
        {
            ClearPending();
            return;
        }


        var anchor = pendingGreetingAnchor != null ? pendingGreetingAnchor : pendingGreetingOwner.transform;
        dialogueUI.ShowLines(pendingGreetingLines, anchor, 0f, greetingMode);
        //dialogueUI.ShowLines(pendingGreetingLines, pendingGreetingAnchor != null ? pendingGreetingAnchor : pendingGreetingOwner.transform);

        pendingGreetingOwner.MarkGreeted();
        ClearPending();
    }

    private void ClearPending()
    {
        pendingGreetingOwner = null;
        pendingGreetingLines = null;
        pendingGreetingAnchor = null;
    }

    public bool TryPresentResult(CustomerManager cm, ResultType resultType, Transform anchor = null, bool randomPick = false)
    {
        if (cm == null) return false;
        if (!store.TryGetValue(cm.GetInstanceID(), out var b)) return false;
        if (b.result == null || !b.result.TryGetValue(resultType, out var q) || q.Count == 0)
            return false;

        string line;
        if (!randomPick)
        {
            line = q.Dequeue();
        }
        else
        {
            var arr = q.ToArray();
            int idx = Random.Range(0, arr.Length);
            line = arr[idx];

            var newQ = new Queue<string>();
            for (int i = 0; i < arr.Length; i++)
                if (i != idx) newQ.Enqueue(arr[i]);
            b.result[resultType] = newQ;
        }

        PresentOne(line, cm, anchor);
        return true;
    }

    private void PresentOne(string line, CustomerManager cm, Transform anchor, DialogueInputMode? mode = null)
    {
        if (dialogueUI != null)
        {
            var a = anchor != null ? anchor : cm.transform;
            dialogueUI.ShowOne(line, a, 1.6f, mode ?? normalMode);
        }
        else
        {
            Debug.Log($"[Dialogue] {cm.name}: {line}");
        }
    }

    private void PresentLines(IEnumerable<string> lines, CustomerManager cm, Transform anchor, DialogueInputMode? mode = null)
    {
        if (dialogueUI != null)
        {
            var a = anchor != null ? anchor : cm.transform;
            dialogueUI.ShowLines(lines, a, 0f, mode ?? normalMode);
        }
        else
        {
            foreach (var l in lines) Debug.Log($"[Dialogue] {cm.name}: {l}");
        }
    }

    public bool HasRemaining(CustomerManager cm, DialogueType type)
    {
        if (cm == null) return false;
        if (!store.TryGetValue(cm.GetInstanceID(), out var b)) return false;

        return type switch
        {
            DialogueType.Greeting => b.greeting.Count > 0,
            DialogueType.Order => b.order.Count > 0,
            DialogueType.Result => false, // 결과는 타입 지정이 필요하므로 별도 API를 사용하세요
            _ => false
        };
    }
    public bool HasResultRemaining(CustomerManager cm, ResultType resultType)
    {
        if (cm == null) return false;
        if (!store.TryGetValue(cm.GetInstanceID(), out var b)) return false;
        return b.result != null && b.result.TryGetValue(resultType, out var q) && q.Count > 0;
    }

    public void ClearAll(CustomerManager cm)
    {
        if (cm == null) return;
        store.Remove(cm.GetInstanceID());
    }

    public void Clear(CustomerManager cm, DialogueType type)
    {
        if (cm == null) return;
        if (!store.TryGetValue(cm.GetInstanceID(), out var b)) return;

        switch (type)
        {
            case DialogueType.Greeting: b.greeting.Clear(); break;
            case DialogueType.Order: b.order.Clear(); break;
            case DialogueType.Result: b.result?.Clear(); break;
        }
    }

    public void ClearResult(CustomerManager cm, ResultType resultType)
    {
        if (cm == null) return;
        if (!store.TryGetValue(cm.GetInstanceID(), out var b)) return;
        if (b.result == null) return;
        b.result.Remove(resultType);
    }

    private static void EnqueueAll(Queue<string> q, IEnumerable<string> lines)
    {
        if (lines == null) return;
        foreach (var s in lines)
            if (!string.IsNullOrWhiteSpace(s)) q.Enqueue(s.Trim());
    }

}