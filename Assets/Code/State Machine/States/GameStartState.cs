using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Big2Game
{
    public class GameStartState : BaseState
    {
        private readonly GameSM gsm;
        public GameStartState(GameSM stateMachine) : base("Game Start", stateMachine) 
        {
            gsm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            gsm.GM.SetBGMVolume(1f);
            gsm.GM.ShowPlayerStats();

            InitCardDeck();
            StartGame();
        }

        void InitCardDeck()
        {
            gsm.cards = new List<Card>();
            foreach (Card card in gsm.cardSet.cards)
            {
                gsm.cards.Add(card);
            }
            gsm.middleDeckCard.GetComponent<SpriteRenderer>().sprite = gsm.backCardSprite;

            ShuffleCard();
        }

        void ShuffleCard()
        {
            for (int i = 0; i < gsm.cards.Count; i++)
            {
                Card temp = gsm.cards[i];
                int randomIndex = Random.Range(i, gsm.cards.Count);
                gsm.cards[i] = gsm.cards[randomIndex];
                gsm.cards[randomIndex] = temp;
            }
        }

        void StartGame()
        {
            gsm.middleDeckCard.SetActive(true);
            gsm.GM.UI.SetCharacterSprite(gsm.charSpriteObj);

            stateMachine.ChangeState(gsm.splitCard);
        }
        public override void Exit()
        {
            base.Exit();
        }
    }

}