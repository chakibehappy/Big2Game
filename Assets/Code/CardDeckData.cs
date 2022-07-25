using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDeckData", menuName = "Big Two Card/Card Deck Data", order = 1)]
public class CardDeckData : ScriptableObject
{
    public List<Card> cards;
}
