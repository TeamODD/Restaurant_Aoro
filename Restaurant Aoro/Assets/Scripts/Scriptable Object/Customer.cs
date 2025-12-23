using UnityEngine;
using System.Collections.Generic;
public enum ResultType { Success, Fail, Perfect, Excellent, Late, WrongOrder }
public enum TribeType { Human, Youkai }
public enum NPCType { Common, Special }

public enum FoodTaste
{
    Sweet,
    Salty,
    Spicy,
    Sour,
    Bitter,
}

/*public enum FoodType
{
    Noodle,   
    Soup,   
    Meat,     
    Dessert,  
    Seafood,  
    Vegetable, 
    Drink    
}*/

[System.Serializable]
public class ResultBucket
{
    public ResultType type;
    [TextArea(2, 5)] public List<string> lines = new();
}

[System.Serializable]
public class VariantStates
{
    [Tooltip("기본 상태(State name). 비워두면 변형 재생 안 함")]
    public string baseState;

    [Tooltip("추가 변형 상태들. (예: sit_good, sit_bad / left_0, left_1)")]
    public List<string> variants = new();
}

[CreateAssetMenu(menuName = "Customer")]
public class Customer : ScriptableObject
{
    [Header("손님 정보")]
    public string CustomerID;
    public string CustomerName;
    public TribeType tribe;
    public NPCType NPCType;
    [Range(0f, 24f)] public float appearStartHour = 17f;
    [Range(0f, 24f)] public float appearEndHour = 20f;
    [TextArea(3, 8)] public string codexDescription;
    public List<FoodTaste> favoriteTastes = new();
    public List<FoodTaste> dislikedTastes = new();
    public List<ItemMainCategory> favoriteFoods = new();
    public List<ItemMainCategory> dislikedFoods = new();
    public List<int> payable;
    public List<Item> payItem;

    [Header("IK Motion Prefabs (큰 상태)")]
    public GameObject prefabStand;     // 기본
    public GameObject prefabSeated;    // 앉기
    public GameObject prefabEating;    // 먹기
    public GameObject prefabLeft;      // 좌
    //public GameObject prefabRight;     // 우

    [Header("Animator State Names (Variants)")]
    public VariantStates standStates;
    public VariantStates seatedStates; // 예: bear_sit, bear_sit_good/bad...
    public VariantStates eatingStates; // 예: bear_eat 변형이 있으면
    public VariantStates leftStates;   // 예: bear_left, bear_left 0/1...
    public VariantStates rightStates;  // 예: bear_right... (없으면 leftStates로 fallback 가능)
    /*[Header("Animations")]
    public AnimationClip frontAnim;
    public AnimationClip leftAnim;
    public AnimationClip rightAnim;
    public AnimationClip seatedAnim;
    public AnimationClip eatingAnim;
    public AnimationClip upAnim;*/

    [Header("입장 대사")]
    [TextArea(2, 5)] public List<string> greetingLines = new();
    [Header("주문 대사")]
    [TextArea(2, 5)] public List<string> orderLines = new();

    [Header("결과 대사 (타입별)")]
    public List<ResultBucket> resultBuckets = new();
}
