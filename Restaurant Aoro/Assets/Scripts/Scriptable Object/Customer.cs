using UnityEngine;
using System.Collections.Generic;
public enum ResultType { Success, Fail, Perfect, Excellent, Late, WrongOrder}
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

[CreateAssetMenu(menuName ="Customer")]
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
    public int payable;

    [Header("Animations")]
    public AnimationClip frontAnim;
    public AnimationClip leftAnim;
    public AnimationClip rightAnim;
    public AnimationClip seatedAnim;
    public AnimationClip eatingAnim;
    public AnimationClip upAnim;

    [Header("입장 대사")]
    [TextArea(2, 5)] public List<string> greetingLines = new();
    [Header("주문 대사")]
    [TextArea(2, 5)] public List<string> orderLines = new();

    [Header("결과 대사 (타입별)")]
    public List<ResultBucket> resultBuckets = new();
}
