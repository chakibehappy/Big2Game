using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardCombinationSet", menuName = "Big Two Card/Card Combination Data", order = 2)]
public class CombinationSet : ScriptableObject
{
    public string Name;
    public bool IsSpecialCombo;
    public bool HavingSameSymbol;
    public bool ValueOnOrder;
    public List<int> SpecialValueOrder;
    public List<int> SameValueCount;
    public int CardCount;
    public int ComboPoint;
}
