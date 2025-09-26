using UnityEngine;
using System.Collections.Generic;
public enum ResultType { Success, Fail, Perfect, Late, WrongOrder}

[System.Serializable]
public class ResultBucket
{
    public ResultType type;
    [TextArea(2, 5)] public List<string> lines = new();
}

[CreateAssetMenu(menuName ="Customer")]
public class Customer : ScriptableObject
{
    public string CustomerID;
    public string CustomerName;

    [Header("Animations")]
    public AnimationClip frontAnim;
    public AnimationClip leftAnim;
    public AnimationClip rightAnim;
    public AnimationClip seatedAnim;
    public AnimationClip eatingAnim;
    public AnimationClip upAnim;

    /*public Sprite leftSprite;
    public Sprite frontSprite;
    public Sprite rightSprite;
    public Sprite SeatedSprite;
    public Sprite EatingSprite;*/

    [Header("대사")]
    [TextArea(2, 5)] public List<string> greetingLines = new();
    [TextArea(2, 5)] public List<string> orderLines = new();

    [Header("결과 대사 (타입별)")]
    public List<ResultBucket> resultBuckets = new();
}
