using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace Big2Game
{
    public class ArrangingCardState : BaseState
    {
        private readonly GameSM gsm;

        public ArrangingCardState(GameSM stateMachine) : base("Arranging Card", stateMachine)
        {
            gsm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            gsm.GM.UI.ShowBackButton(false);
            gsm.GM.UI.ShowPlayerCoins(false);

            gsm.GM.UI.OnConfirmButtonEvent += FinishArangingCard;

            gsm.dimScreen.SetActive(true);
            
            for (int i = 1; i < gsm.playerCount; i++)
            {
                List<Card> tempCard = GetAllSuggestionCardSet(gsm.playerCardOnHand[i], out List<List<string>> name)[0];
                gsm.playerCardOnHand[i] = new List<Card>(tempCard);

            }
            gsm.suggestionComboSet = GetAllSuggestionCardSet(gsm.playerCardOnHand[0], out gsm.suggestionComboSetName);
           
            AssignClickableSuggestion();
            AssignClickableOnCard();
            ShowPlayerCardOnHand(gsm.playerCardOnHand[0]);
            CheckPlayerCardFormation();
        }

        void FinishArangingCard()
        {
            gsm.readyPlayerCard.gameObject.SetActive(false);
            gsm.dimScreen.SetActive(false);

            gsm.GM.UI.ShowConfirmButton(false);
            gsm.GM.UI.ShowBackButton(true);
            gsm.GM.ShowPlayerStats();

            gsm.GM.UI.ShowBackButton();
            gsm.GM.UI.OnConfirmButtonEvent -= FinishArangingCard;

            stateMachine.ChangeState(gsm.scoringCard);
        }

        void AssignClickableOnCard()
        {
            for (int i = 0; i < gsm.playerCardTrigger.Length; i++)
            {
                EventTrigger trigger = gsm.playerCardTrigger[i];

                EventTrigger.Entry onCardClick = new EventTrigger.Entry();
                onCardClick.eventID = EventTriggerType.PointerDown;
                int index = i;
                onCardClick.callback.AddListener((data) => { TapOnCard(index); });

                trigger.triggers.Clear();
                trigger.triggers.Add(onCardClick);
            }
        }

        void TapOnCard(int cardIndex)
        {
            if (!gsm.isSelectingCard)
            {
                gsm.currentSelectedCardIndex = cardIndex;
                gsm.selectedCard = gsm.readyPlayerCard.GetChild(cardIndex).GetComponent<SpriteRenderer>();
                gsm.readyPlayerCard.GetChild(cardIndex).localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }
            else
            {
                gsm.PlaySFX(1);

                ResetColorOnSuggestionBoxs();
                gsm.selectedCard.transform.localScale = Vector3.one;
                if (cardIndex != gsm.currentSelectedCardIndex)
                {
                    Card tempCard = gsm.playerCardOnHand[0][gsm.currentSelectedCardIndex];
                    gsm.playerCardOnHand[0][gsm.currentSelectedCardIndex] = gsm.playerCardOnHand[0][cardIndex];
                    gsm.playerCardOnHand[0][cardIndex] = tempCard;

                    ShowPlayerCardOnHand(gsm.playerCardOnHand[0]);
                    CheckPlayerCardFormation();
                }
            }
            gsm.isSelectingCard = !gsm.isSelectingCard;
        }

        void ShowPlayerCardOnHand(List<Card> cardsInHand)
        {
            for (int i = 0; i < cardsInHand.Count; i++)
            {
                gsm.readyPlayerCard.GetChild(i).GetComponent<SpriteRenderer>().sprite = cardsInHand[i].CardSprite;
            }
            gsm.readyPlayerCard.gameObject.SetActive(true);
        }

        void ResetColorOnSuggestionBoxs()
        {
            for (int i = 0; i < gsm.suggestionBox.Length; i++)
            {
                for (int j = 0; j < gsm.suggestionBox[i].transform.childCount; j++)
                {
                    gsm.suggestionBox[i].transform.GetChild(j).GetComponent<TextMeshPro>().color = gsm.inactiveSuggestionColor;
                }
            }
        }

        void AssignClickableSuggestion()
        {
            ResetColorOnSuggestionBoxs();
            for (int i = 0; i < gsm.suggestionBox.Length; i++)
            {
                gsm.suggestionBox[i].gameObject.SetActive(false);
            }

            int totalSuggestion = Mathf.Min(gsm.suggestionComboSet.Count, gsm.suggestionBox.Length);
            for (int i = 0; i < totalSuggestion; i++)
            {
                for (int j = 0; j < gsm.suggestionBox[i].transform.childCount; j++)
                {
                    gsm.suggestionBox[i].transform.GetChild(j).GetComponent<TextMeshPro>().text = gsm.suggestionComboSetName[i][j];
                }

                EventTrigger trigger = gsm.suggestionBox[i];
                EventTrigger.Entry onBoxClick = new EventTrigger.Entry();
                onBoxClick.eventID = EventTriggerType.PointerDown;
                int index = i;
                onBoxClick.callback.AddListener((data) => { TapOnSuggestion(index); });
                trigger.triggers.Clear();
                trigger.triggers.Add(onBoxClick);
                gsm.suggestionBox[i].gameObject.SetActive(true);
            }
        }

        void TapOnSuggestion(int index)
        {
            gsm.PlaySFX(1);
            ResetColorOnSuggestionBoxs();
            for (int j = 0; j < gsm.suggestionBox[index].transform.childCount; j++)
            {
                gsm.suggestionBox[index].transform.GetChild(j).GetComponent<TextMeshPro>().color = gsm.activeSuggestionColor;
            }

            gsm.playerCardOnHand[0] = new List<Card>(gsm.suggestionComboSet[index]);
            ShowPlayerCardOnHand(gsm.playerCardOnHand[0]);
            CheckPlayerCardFormation();
        }

        void CheckPlayerCardFormation()
        {
            bool isValid = CheckCardFormation(gsm.playerCardOnHand[0]);
            gsm.GM.UI.ShowConfirmButton(isValid);
        }

        bool CheckCardFormation(List<Card> cardsInHand)
        {
            if (cardsInHand.Count < 13)
                return false;

            List<Card> frontCardSet = new List<Card>();
            for (int i = 0; i < 3; i++)
            {
                frontCardSet.Add(cardsInHand[i]);
            }
            List<Card> middleCardSet = new List<Card>();
            for (int i = 3; i < 8; i++)
            {
                middleCardSet.Add(cardsInHand[i]);
            }
            List<Card> backCardSet = new List<Card>();
            for (int i = 8; i < cardsInHand.Count; i++)
            {
                backCardSet.Add(cardsInHand[i]);
            }

            bool[] isValid = gsm.CardFormationIsValid(frontCardSet, middleCardSet, backCardSet, out string[] comboSetName);

            for (int i = 0; i < gsm.checkIconObj.Length; i++)
            {
                gsm.checkIconObj[i].sprite = gsm.checkIcon[isValid[i] ? 0 : 1];
                gsm.txtPlayerCardCombo[i].text = comboSetName[i];
            }

            return isValid[0] && isValid[1] && isValid[2];
        }

        List<List<Card>> GetAllSuggestionCardSet(List<Card> cardList, out List<List<string>> comboSetName)
        {
            List<List<Card>> resultComboSet = new List<List<Card>>();
            List<List<string>> comboName = new List<List<string>>();
            List<List<Card>> comboSet = FillComboSet(cardList);

            for (int i = 0; i < comboSet.Count; i++)
            {
                List<Card> tempCard = new List<Card>(cardList);

                List<Card> frontCard = new List<Card>();
                List<Card> middleCard = new List<Card>();
                List<Card> backCard = new List<Card>();

                foreach (Card card in comboSet[i])
                {
                    backCard.Add(card);
                    tempCard.Remove(card);
                }

                List<List<Card>> nextComboSet = FillComboSet(tempCard);
                foreach (Card card in nextComboSet[0])
                {
                    middleCard.Add(card);
                    tempCard.Remove(card);
                }

                FillTheEmptyCard(backCard, tempCard, 5);
                FillTheEmptyCard(middleCard, tempCard, 5);

                nextComboSet = FillComboSet(tempCard);
                foreach (Card card in nextComboSet[0])
                {
                    frontCard.Add(card);
                    tempCard.Remove(card);
                }
                FillTheEmptyCard(frontCard, tempCard, 3);

                List<Card> newCardList = new List<Card>();
                frontCard.ForEach((card) => newCardList.Add(card));
                middleCard.ForEach((card) => newCardList.Add(card));
                backCard.ForEach((card) => newCardList.Add(card));

                string[] combo = new string[3];
                bool[] isValid = gsm.CardFormationIsValid(frontCard, middleCard, backCard, out combo);

                if (isValid[0] && isValid[1] && isValid[2])
                {
                    resultComboSet.Add(newCardList);
                    comboName.Add(new List<string>() { combo[0], combo[1], combo[2] });
                }
            }

            comboSetName = new List<List<string>>(comboName);
            return resultComboSet;
        }

        void FillTheEmptyCard(List<Card> cardSet, List<Card> cardList, int maxCount)
        {
            while (cardSet.Count < maxCount)
            {
                Card randomCard = cardList[Random.Range(0, cardList.Count)];
                cardSet.Add(randomCard);
                cardList.Remove(randomCard);
            }
        }

        List<List<Card>> FillComboSet(List<Card> cardList)
        {
            List<List<Card>> comboSet = new List<List<Card>>();

            List<List<Card>> sameSymbolCardSet = gsm.GetCardWithSameSymbol(cardList);
            List<List<Card>> sameValueCardSet = gsm.GetPairedCards(cardList);
            List<List<Card>> valueOnOrderCardSet = gsm.GetCardOrderList(cardList);

            foreach (List<Card> cards in sameSymbolCardSet) // royal flush & straight flush
            {
                gsm.GetHighestComboSetPoint(cards, out string comboName);
                if (comboName == "Royal Flush")
                    comboSet.Add(cards);

                if (comboName == "Straight Flush")
                    comboSet.Add(cards);
            }

            foreach (List<Card> cards in sameValueCardSet) // 4 of a Kind
            {
                if (cards.Count == 4)
                    comboSet.Add(cards);
            }

            if (sameValueCardSet.Count > 1) // fullhouse
            {
                for (int i = 0; i < sameValueCardSet.Count; i++)
                {
                    if (sameValueCardSet[i].Count == 3 && i < sameValueCardSet.Count - 1)
                    {
                        for (int j = i + 1; j < sameValueCardSet.Count; j++)
                        {
                            if (sameValueCardSet[i].Count + sameValueCardSet[j].Count <= 5)
                            {
                                List<Card> newPairSet = new List<Card>();
                                sameValueCardSet[i].ForEach((card) => newPairSet.Add(card));
                                sameValueCardSet[j].ForEach((card) => newPairSet.Add(card));
                                comboSet.Add(newPairSet);
                            }
                        }
                    }
                }
            }

            foreach (List<Card> cards in sameSymbolCardSet) // flush
            {
                gsm.GetHighestComboSetPoint(cards, out string comboName);
                if (comboName == "Flush")
                    comboSet.Add(cards);
            }

            foreach (List<Card> cards in valueOnOrderCardSet) // straight
            {
                comboSet.Add(cards);
            }

            foreach (List<Card> cards in sameValueCardSet) // 3 of a Kind
            {
                if (cards.Count == 3)
                    comboSet.Add(cards);
            }

            if (sameValueCardSet.Count > 1) // 2 pair
            {
                for (int i = 0; i < sameValueCardSet.Count; i++)
                {
                    if (sameValueCardSet[i].Count == 2 && i < sameValueCardSet.Count - 1)
                    {
                        for (int j = i + 1; j < sameValueCardSet.Count; j++)
                        {
                            if (sameValueCardSet[i].Count + sameValueCardSet[j].Count <= 5)
                            {
                                List<Card> newPairSet = new List<Card>();
                                sameValueCardSet[i].ForEach((card) => newPairSet.Add(card));
                                sameValueCardSet[j].ForEach((card) => newPairSet.Add(card));
                                comboSet.Add(newPairSet);
                            }
                        }
                    }
                }
            }

            foreach (List<Card> cards in sameValueCardSet) // 1 pair
            {
                if (cards.Count == 2)
                    comboSet.Add(cards);
            }

            if (sameSymbolCardSet.Count == 0 && sameValueCardSet.Count == 0 && valueOnOrderCardSet.Count == 0) // high card
            {
                cardList.Sort(SortByValue);
                comboSet.Add(new List<Card> { cardList[cardList.Count - 1] });
            }

            return comboSet;
        }

        private static int SortByValue(Card card1, Card card2)
        {
            return card1.Value.CompareTo(card2.Value);
        }
    }
}
