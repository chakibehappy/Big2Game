using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Big2Game
{
    public class SplitCardState : BaseState
    {
        private readonly GameSM gsm;
        public SplitCardState(GameSM stateMachine) : base("Split Card", stateMachine)
        {
            gsm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();

            DistributeCardsToPlayers();
        }

        void DistributeCardsToPlayers()
        {
            gsm.playerCardObjOnHand = new List<List<GameObject>>();
            gsm.playerCardOnHand = new List<List<Card>>();
            for (int i = 0; i < gsm.playerCount; i++)
            {
                gsm.playerCardObjOnHand.Add(new List<GameObject>());
                gsm.playerCardOnHand.Add(new List<Card>());
            }

            int cardIndex = 0;

            for (int j = 0; j < gsm.maxCardCountOnHand; j++)
            {
                for (int i = 0; i < gsm.playerCount; i++)
                {
                    gsm.playerCardOnHand[i].Add(gsm.cards[cardIndex]);
                    cardIndex++;
                }
            }

            gsm.StartCoroutine(DistributeCardsToPlayersIE());
        }

        public IEnumerator DistributeCardsToPlayersIE()
        {
            yield return new WaitForSeconds(1f);

            gsm.PlaySFX(0, true);

            for (int j = 0; j < gsm.maxCardCountOnHand; j++)
            {
                for (int i = 0; i < gsm.playerCount; i++)
                {
                    Vector3 centerPos = gsm.middleDeckCard.transform.position;
                    GameObject newCardObj = gsm.GM.ObjectPooler.GetObject("Card", centerPos, Quaternion.identity);
                    SpriteRenderer newCardSprite = newCardObj.GetComponent<SpriteRenderer>();
                    newCardSprite.sortingOrder = j;
                    newCardSprite.sprite = gsm.backCardSprite;

                    Vector3 newPos = gsm.startCardPositions[i].position;
                    if (i % 2 == 0)
                    {
                        newPos.x = 0;
                    }

                    float delay = GetMovementTime(gsm.middleDeckCard.transform.position, newPos, gsm.cardMovementSpeed);
                    newCardObj.transform.DOMove(newPos, delay).SetEase(Ease.Linear);
                    yield return new WaitForSeconds(delay);
                    gsm.playerCardObjOnHand[i].Add(newCardObj);

                    newPos = new Vector3(gsm.startCardPositions[i].position.x + (gsm.cardSpaceX * j), gsm.startCardPositions[i].position.y, 0);
                    delay = GetMovementTime(gsm.startCardPositions[i].position, newPos, gsm.cardMovementSpeed);
                    newCardObj.transform.DOMove(newPos, delay).SetEase(Ease.Linear);
                }
            }
            gsm.GM.StopSFX();
            gsm.middleDeckCard.SetActive(false);

            for (int j = 0; j < gsm.maxCardCountOnHand; j++)
            {
                for (int i = 0; i < gsm.playerCount; i++)
                {
                    Vector3 endPos = gsm.cardSetOnTablePos[i].GetChild(j).position;
                    Quaternion endRot = gsm.cardSetOnTablePos[i].GetChild(j).rotation;
                    gsm.playerCardObjOnHand[i][j].transform.DOMove(endPos, 0.25f).SetEase(Ease.Linear);
                    gsm.playerCardObjOnHand[i][j].transform.DORotateQuaternion(endRot, 0.25f).SetEase(Ease.Linear);
                }
            }
            yield return new WaitForSeconds(0.5f);

            stateMachine.ChangeState(gsm.arrangingCard);
        }


        float GetMovementTime(Vector3 posA, Vector3 posB, float speed)
        {
            return Vector3.Distance(posA, posB) * speed;
        }
    }
}
