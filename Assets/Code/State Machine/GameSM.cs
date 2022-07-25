using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Big2Game
{
    public class GameSM : StateMachine
    {
        [HideInInspector] public GameStartState start;
        [HideInInspector] public SplitCardState splitCard;
        [HideInInspector] public ArrangingCardState arrangingCard;
        [HideInInspector] public ScoringCardState scoringCard;
        [HideInInspector] public GameEndState end;

        [HideInInspector] public GameMaster GM;
        
        public CardDeckData cardSet;
        public CombinationSet[] combinationDataSets;

        [HideInInspector] public List<Card> cards;

        public Transform[] startCardPositions;
        public float cardSpaceX = 0.4f;
        public int maxCardCountOnHand = 13;
        public int playerCount = 4;

        public GameObject middleDeckCard;
        public Sprite backCardSprite;
        public float cardMovementSpeed = 0.01f;

        [HideInInspector] public List<List<GameObject>> playerCardObjOnHand;
        [HideInInspector] public List<List<Card>> playerCardOnHand;
        
        public List<Transform> cardSetOnTablePos;
        public SpriteRenderer[] charSpriteObj;

        [Header("Player Cards Selection")]
        public GameObject dimScreen;
        public EventTrigger[] playerCardTrigger;
        public SpriteRenderer[] checkIconObj;
        public TextMeshPro[] txtPlayerCardCombo;
        public Sprite[] checkIcon;

        [HideInInspector] public bool isSelectingCard = false;
        [HideInInspector] public SpriteRenderer selectedCard;
        [HideInInspector] public int currentSelectedCardIndex = 0;
        
        public Transform readyPlayerCard;

        [Header("Suggestion Section")]
        public EventTrigger[] suggestionBox;
        public List<List<Card>> suggestionComboSet;
        public List<List<string>> suggestionComboSetName;
        public Color inactiveSuggestionColor;
        public Color activeSuggestionColor;

        [HideInInspector] public int[] playerPoints;
        public GameObject[] playerInfo;
        public TextMeshPro[] txtPlayerInfo;
        public TextMeshPro[] txtPlayerPoint;
        public int coinBetCount = 3;

        [Header("Audio")]
        public AudioClip[] sfxClip;


        public void Awake()
        {
            start = new GameStartState(this);
            splitCard = new SplitCardState(this);
            arrangingCard = new ArrangingCardState(this);
            scoringCard = new ScoringCardState(this);
            end = new GameEndState(this);

            GM = GameMaster.Instance;
            GM.UI.ShowBackButton();
            GM.UI.OnBackButtonEvent += BackToCharacterSelect;
        }

        private void OnDestroy()
        {
            HideAllCards();
            GM.UI.OnBackButtonEvent -= BackToCharacterSelect;
        }

        public void HideAllCards()
        {
            GM.ObjectPooler.HideAllPoolObjects("Card");
        }

        private void BackToCharacterSelect()
        {
            SceneManager.LoadScene("CharacterSelect");
        }

        protected override BaseState GetInitialState()
        {
            return start;
        }

        public void PlaySFX(int clipIndex, bool isLoop = false)
        {
            GM.PlaySFX(sfxClip[clipIndex], isLoop);
        }

        #region Logic
        public List<List<Card>> GetCardWithSameSymbol(List<Card> cardList)
        {
            string[] symbolOrder = new string[] { "Spades", "Hearts", "Clubs", "Diamonds" };

            List<Card> tempCard = new List<Card>(cardList);
            tempCard.Reverse();

            List<Card> spadesCards = new List<Card>();
            List<Card> heartsCards = new List<Card>();
            List<Card> clubsCards = new List<Card>();
            List<Card> diamondsCards = new List<Card>();

            for (int i = 0; i < tempCard.Count; i++)
            {
                switch (tempCard[i].Type.Symbol)
                {
                    case CardSymbol.Spades:
                        if (spadesCards.Count < 5)
                            spadesCards.Add(tempCard[i]);
                        break;
                    case CardSymbol.Hearts:
                        if (heartsCards.Count < 5)
                            heartsCards.Add(tempCard[i]);
                        break;
                    case CardSymbol.Clubs:
                        if (clubsCards.Count < 5)
                            clubsCards.Add(tempCard[i]);
                        break;
                    case CardSymbol.Diamonds:
                        if (diamondsCards.Count < 5)
                            diamondsCards.Add(tempCard[i]);
                        break;
                    default:
                        break;
                }
            }

            List<List<Card>> cardSet = new List<List<Card>>();

            if (spadesCards.Count >= 5)
                cardSet.Add(spadesCards);

            if (heartsCards.Count >= 5)
                cardSet.Add(heartsCards);

            if (clubsCards.Count >= 5)
                cardSet.Add(clubsCards);

            if (diamondsCards.Count >= 5)
                cardSet.Add(diamondsCards);

            return cardSet;
        }

        public List<List<Card>> GetCardOrderList(List<Card> cardList)
        {
            List<Card> tempCard = new List<Card>(cardList);

            tempCard.Sort(SortByValue);
            tempCard.Reverse();

            int smallestCardValue = tempCard[tempCard.Count - 1].Value;
            int highestCardValue = tempCard[0].Value;
            int currentCardOrderingValue = 0;

            List<List<Card>> cardOnOrderSet = new List<List<Card>>();
            List<Card> cardOrderList = new List<Card>();
            int orderStreakCount = 1;

            cardOrderList.Add(tempCard[0]);
            currentCardOrderingValue = highestCardValue;

            int startcardIndex = 0;
            bool isDone = false;
            while (!isDone)
            {
                for (int i = startcardIndex; i < tempCard.Count; i++)
                {
                    if (i == tempCard.Count - 1)
                    {
                        isDone = true;
                    }

                    if (tempCard[i].Value != currentCardOrderingValue)
                    {
                        if (tempCard[i].Value == currentCardOrderingValue - 1)
                        {
                            cardOrderList.Add(tempCard[i]);
                            orderStreakCount++;
                            if (orderStreakCount == 5)
                            {
                                cardOnOrderSet.Add(cardOrderList);
                                Card nextCard = cardOrderList[1];
                                cardOrderList = new List<Card>();

                                orderStreakCount = 1;
                                currentCardOrderingValue = nextCard.Value;
                                cardOrderList.Add(nextCard);
                                startcardIndex = tempCard.IndexOf(nextCard);
                                break;
                            }
                            else
                            {
                                currentCardOrderingValue = tempCard[i].Value;
                            }
                        }
                        else
                        {
                            cardOrderList = new List<Card>
                        {
                            tempCard[i]
                        };
                            orderStreakCount = 1;
                            currentCardOrderingValue = tempCard[i].Value;
                        }
                    }
                }
            }

            // checking the order for A, 2, 3, 4, 5 :
            List<Card> newCard = tempCard.Where(x => x.Value == 14 || x.Value <= 5).ToList();
            List<int> newValue = new List<int>();
            newCard.ForEach((card) => { newValue.Add(card.Value); });
            if (newValue.Contains(2) && newValue.Contains(3) && newValue.Contains(4) && newValue.Contains(5) && newValue.Contains(14))
            {
                List<Card> lastOrderCard = new List<Card>();
                List<int> num = new List<int>() { 2, 3, 4, 5, 14 };
                for (int i = 0; i < num.Count; i++)
                {
                    lastOrderCard.Add(newCard.First(x => x.Value == num[i]));
                }
                cardOnOrderSet.Add(lastOrderCard);
            }

            return cardOnOrderSet;
        }

        private static int SortByValue(Card card1, Card card2)
        {
            return card1.Value.CompareTo(card2.Value);
        }

        public List<List<Card>> GetPairedCards(List<Card> cardList)
        {
            List<Card> tempCard = new List<Card>(cardList);
            List<List<Card>> pairedCardSets = new List<List<Card>>();
            int[] cardValues = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

            for (int i = 0; i < cardValues.Length; i++)
            {
                List<Card> serialCard = new List<Card>();
                for (int j = 0; j < tempCard.Count; j++)
                {
                    if (tempCard[j].Value == cardValues[i])
                    {
                        serialCard.Add(tempCard[j]);
                    }
                }
                if (serialCard.Count > 1)
                {
                    pairedCardSets.Add(serialCard);
                }
            }

            if (pairedCardSets.Count > 1)
            {
                pairedCardSets.Sort(SortByItemCount);
                pairedCardSets.Reverse();
            }

            return pairedCardSets;
        }

        private static int SortByItemCount(List<Card> cardSet1, List<Card> cardSet2)
        {
            return cardSet1.Count.CompareTo(cardSet2.Count);
        }

        public bool[] CardFormationIsValid(List<Card> frontCardSet, List<Card> middleCardSet, List<Card> backCardSet, out string[] comboSetName)
        {
            bool[] isValid = new bool[3];
            comboSetName = new string[3];

            int frontSetValue = GetHighestComboSetPoint(frontCardSet, out comboSetName[0]);
            int middleSetValue = GetHighestComboSetPoint(middleCardSet, out comboSetName[1]);
            int backSetValue = GetHighestComboSetPoint(backCardSet, out comboSetName[2]);

            isValid[0] = frontSetValue < middleSetValue && frontSetValue < backSetValue;
            isValid[1] = middleSetValue > frontSetValue && middleSetValue < backSetValue;
            isValid[2] = backSetValue > middleSetValue && backSetValue > frontSetValue;

            return isValid;
        }

        public int GetHighestComboSetPoint(List<Card> cardList, out string comboName)
        {
            // First, Get only the Highest Posible card combo, start from royal flush - high card
            // 2nd, count the Point by : ComboPoint on Combination Set Data and also add card or card set value (except full house)
            // - for full house add multiplier for three of kind (multiply by 15, higher value than ace) and add the pair value, ex 222AA vs 33344 == 30 + 14 vs 45 + 4
            // highest total full House Point will be : AAAKK =  6000 + (15 x 14) + 13 = 6223 which mean is still under the 4 of the kind base Point
            
            int point = 0;
            comboName = "High Card";

            List<List<Card>> sameSymbolCardSet = GetCardWithSameSymbol(cardList);
            List<int> cardValueOrder = new List<int>();
            bool isHavingSameSymbol = false;
            foreach (var item in sameSymbolCardSet)
            {
                if (item.Count >= 5)
                {
                    isHavingSameSymbol = true;
                    foreach (var card in item)
                    {
                        cardValueOrder.Add(card.Value);
                    }
                    cardValueOrder.Sort();
                    cardValueOrder.Reverse();
                }
            }

            List<List<Card>> valueOnOrderCardSet = GetCardOrderList(cardList);
            bool valueOnOrder = valueOnOrderCardSet.Count > 0;
            List<int> royalFlushValueOrder = new List<int>() { 14, 13, 12, 11, 10 };
            List<int> specialValueOrder = new List<int>();
            if (isHavingSameSymbol && IsEqualList(royalFlushValueOrder, cardValueOrder))
            {
                specialValueOrder = royalFlushValueOrder;
            }

            List<List<Card>> sameValueCardSet = GetPairedCards(cardList);
            List<int> sameValueCount = new List<int>();
            if (sameValueCardSet.Count > 0)
            {
                foreach (var item in sameValueCardSet)
                {
                    sameValueCount.Add(item.Count);
                }
            }

            foreach (var item in combinationDataSets)
            {
                if (isHavingSameSymbol == item.HavingSameSymbol
                    && valueOnOrder == item.ValueOnOrder
                    && IsEqualList(specialValueOrder, item.SpecialValueOrder)
                    && IsEqualList(sameValueCount, item.SameValueCount))
                {
                    point += item.ComboPoint;
                    if (item.Name == "High Card")
                    {
                        cardList.Sort(SortByValue);
                        point += cardList[cardList.Count - 1].Value;
                    }
                    else if (item.Name == "One Pair" || item.Name == "Three of a Kind" || item.Name == "Four of a Kind")
                    {
                        point += sameValueCardSet[0][0].Value;
                    }
                    else if (item.Name == "Two Pair")
                    {
                        point += sameValueCardSet[0][0].Value + sameValueCardSet[1][0].Value;
                    }
                    else if (item.Name == "Full House")
                    {
                        point += (15 * sameValueCardSet[0][0].Value) + sameValueCardSet[1][0].Value;
                    }
                    else
                    {
                        for (int i = 0; i < cardList.Count; i++)
                        {
                            point += cardList[i].Value;
                        }
                    }

                    // ace is on low order :
                    cardValueOrder.Sort();
                    if (IsEqualList(cardValueOrder, new List<int>() { 2, 3, 4, 5, 14 }))
                    {
                        point -= 13;
                    }

                    comboName = item.Name;
                    return point;
                }
            }
            return point;
        }

        public bool IsEqualList(List<int> list1, List<int> list2)
        {
            if (list1.Count == list2.Count)
            {
                if (list1.Count == 0)
                    return true;

                int sameValue = 0;
                for (int i = 0; i < list1.Count; i++)
                {
                    if (list1[i] == list2[i])
                    {
                        sameValue++;
                    }
                }
                return sameValue == list1.Count;
            }
            return false;
        }

        #endregion
    }
}