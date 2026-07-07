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
    [Tooltip("�⺻ ����(State name). ����θ� ���� ��� �� ��")]
    public string baseState;

    [Tooltip("�߰� ���� ���µ�. (��: sit_good, sit_bad / left_0, left_1)")]
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
    [TextArea(3, 8)] public string codexDetailDescription;
    public List<FoodTaste> favoriteTastes = new();
    public List<FoodTaste> dislikedTastes = new();
    public List<ItemMainCategory> favoriteFoods = new();
    public List<ItemMainCategory> dislikedFoods = new();
    public List<int> payable;
    public List<Item> payItem;

    [Header("IK Motion Prefabs (큰 상태)")]
    public GameObject prefabStand;     // �⺻
    public GameObject prefabSeated;    // �ɱ�
    public GameObject prefabEating;    // �Ա�
    public GameObject prefabLeft;      // ��
    //public GameObject prefabRight;     // ��

    [Header("Animator State Names (Variants)")]
    public VariantStates standStates;
    public VariantStates seatedStates; // ��: bear_sit, bear_sit_good/bad...
    public VariantStates eatingStates; // ��: bear_eat ������ ������
    public VariantStates leftStates;   // ��: bear_left, bear_left 0/1...
    public VariantStates rightStates;  // ��: bear_right... (������ leftStates�� fallback ����)
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
