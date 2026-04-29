using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card/Card Data")]
public class CardData : ScriptableObject
{
    public enum CardType
    {
        Attack,
        Heal,
        Buff,
        Utility,
    }

    public string cardName;
    public string description;
    public Sprite artwork;
    public int manaCost;
    public int effectAmount;
    public CardType cardType;

    //추가효과 리스트
    public List<AddiTionamEffect> additionalEffects = new List<AddiTionamEffect>();

    public enum AdditionalEffectType
    {
        None,
        DrawCard,
        DiscardCard,
        GainMana,
        ReduceEnemyMana,
        ReduceCardCost
    }

    public Color GetcardColor()
    {
        switch (cardType)
        {
            case CardType.Attack:
                return new Color(0.9f, 0.3f, 0.3f);

            case CardType.Heal:
                return new Color(0.3f, 0.9f, 0.3f); 
            case CardType.Buff:
                return new Color(0.9f, 0.3f, 0.3f);

            case CardType.Utility:
                return new Color(0.9f, 0.3f, 0.3f);

            default:
                return Color.white;




        }
    }

    public string GetAdditionalEffectDescription()
    {
        if (additionalEffects.Count == 0)
            return "";
        string result = "\n";
        foreach (var effect in additionalEffects)
        {
            result += effect.GetDescription() + "\n";
        }

        return result;
    }

}
