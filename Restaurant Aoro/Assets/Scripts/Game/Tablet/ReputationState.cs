using Unity.VisualScripting;
using UnityEngine;

public class ReputationState : MonoBehaviour
{
    [Header("평판 아이콘")]
    public Sprite Customer_1;
    public Sprite Customer_2;
    public Sprite Customer_3;
    public Sprite Youkai_1;
    public Sprite Youkai_2;
    public Sprite Youkai_3;

    private int Youkai_Human_reputation = 0;
    private int Customer_reputation = -100;
    private Sprite CustomerSprite;
    private Sprite YoukaiSprite;

    public void setSprite()
    {
        if(Youkai_Human_reputation >= 33)
        {
            YoukaiSprite = Youkai_3;
        }
        else if (Youkai_Human_reputation >= -33)
        {
            YoukaiSprite = Youkai_2;
        }
        else
        {
            YoukaiSprite = Youkai_1;
        }

        if (Customer_reputation >= 33)
        {
            CustomerSprite = Customer_3;
        }
        else if (Customer_reputation >= -33)
        {
            CustomerSprite = Customer_2;
        }
        else
        {
            CustomerSprite = Customer_1;
        }
    }

    public void addCustomerReputation(int value)
    {
        Customer_reputation += value;
    }

    public void addYoukaiReputation(int value)
    {
        Youkai_Human_reputation += value;
    }
}
