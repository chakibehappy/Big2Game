using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Big2Game
{
    public class CardDataGenerator : MonoBehaviour
    {
        public GameMaster GM;

        [SerializeField] private CardDeckData deck;

        [SerializeField] private List<Sprite> spadesSet;
        [SerializeField] private List<Sprite> heartSet;
        [SerializeField] private List<Sprite> clubsSet;
        [SerializeField] private List<Sprite> diamondsSet;

        [SerializeField]
        private string[] cardLabel = new string[]
        {
        "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace"
        };

        [SerializeField] private int smallestValue = 2;

        [SerializeField]
        private CardSymbol[] cardSymbols = new CardSymbol[]
        {
        CardSymbol.Spades,
        CardSymbol.Hearts,
        CardSymbol.Clubs,
        CardSymbol.Diamonds,
        };

        [SerializeField]
        private CardColor[] cardColors = new CardColor[]
        {
        CardColor.Black,
        CardColor.Red,
        CardColor.Black,
        CardColor.Red,
        };

        List<List<Sprite>> cardSprites;
        List<GameObject> cardDisplaySet;

        [SerializeField] private GameObject cardDisplay;
        [SerializeField] private float spaceX = 1;
        [SerializeField] private float spaceY = 2;

        private void Start()
        {
            cardSprites = new List<List<Sprite>>
        {
            spadesSet,
            heartSet,
            clubsSet,
            diamondsSet
        };

            CheckAndFillCardDeck();
            ShowAllCards();
        }

        void CheckAndFillCardDeck()
        {
            if (deck.cards.Count == 0)
            {
                for (int i = 0; i < cardSprites.Count; i++)
                {
                    for (int j = 0; j < cardSprites[i].Count; j++)
                    {
                        Card card = new Card
                        {
                            Name = cardLabel[j] + " of " + cardSymbols[i].ToString(),
                            Value = smallestValue + j,
                            Label = cardLabel[j],
                            Type = new CardType
                            {
                                Symbol = cardSymbols[i],
                                Color = cardColors[i]
                            },
                            CardSprite = cardSprites[i][j]
                        };

                        deck.cards.Add(card);
                    }
                }

                EditorUtility.SetDirty(deck);
            }
        }

        void ShowAllCards()
        {
            cardDisplay.SetActive(false);
            Vector3 startPos = cardDisplay.transform.position;
            cardDisplaySet = new List<GameObject>();

            int cardIndex = 0;
            for (int i = 0; i < cardSprites.Count; i++)
            {
                for (int j = 0; j < cardSprites[i].Count; j++)
                {
                    Vector3 newPos = new Vector3(startPos.x + (spaceX * j), 0.5f + startPos.y - (spaceY * i), 0);
                    if (i % 2 == 0)
                    {
                        newPos.x -= 0.5f;
                    }
                    GameObject newCard = GM.ObjectPooler.GetObject("Card", newPos, Quaternion.identity);
                    newCard.name = deck.cards[cardIndex].Name;

                    SpriteRenderer cardSprite = newCard.GetComponent<SpriteRenderer>();
                    cardSprite.sprite = deck.cards[cardIndex].CardSprite;
                    cardSprite.sortingOrder = cardIndex;

                    newCard.SetActive(true);
                    cardDisplaySet.Add(newCard);
                    cardIndex++;
                }
            }
        }
    }
}