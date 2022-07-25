using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Card
{
    public string Name;
    public int Value;
    public string Label;
    public CardType Type;
    public Sprite CardSprite;
}

[System.Serializable]
public class CardType
{
    public CardSymbol Symbol;
    public CardColor Color;
}

public enum CardSymbol
{
    Spades,
    Hearts,
    Clubs,
    Diamonds
}

public enum CardColor
{
    Black,
    Red
}