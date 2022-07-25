using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Big2Game
{
    public class ScoringCardState : BaseState
    {
        private readonly GameSM gsm;

        public ScoringCardState(GameSM stateMachine) : base("Scoring Card", stateMachine)
        {
            gsm = stateMachine;
        }
        public override void Enter()
        {
            base.Enter();

            gsm.StartCoroutine(StartComparingCardsIE());
        }

        IEnumerator StartComparingCardsIE()
        {
            gsm.PlaySFX(2);
            gsm.GM.SetBGMVolume(0.25f);
            gsm.playerPoints = new int[gsm.playerCount];

            ShowCardOnTable(0, gsm.playerCardOnHand[0]);

            for (int i = 1; i < 4; i++)
            {
                ShowCardOnTable(i, gsm.playerCardOnHand[i]);
            }

            yield return gsm.StartCoroutine(CompareAllPlayerCardsIE());

            stateMachine.ChangeState(gsm.end);
        }

        
        void ShowCardOnTable(int playerIndex, List<Card> cardList)
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                gsm.playerCardObjOnHand[playerIndex][i].GetComponent<SpriteRenderer>().sprite = cardList[i].CardSprite;
            }
        }

        IEnumerator CompareAllPlayerCardsIE()
        {
            yield return gsm.StartCoroutine(ComparePlayerCardsIE("FRONT", 0, 3));

            yield return gsm.StartCoroutine(ComparePlayerCardsIE("MIDDLE", 3, 8));

            yield return gsm.StartCoroutine(ComparePlayerCardsIE("BACK", 8, 13));

            gsm.GM.SetBGMVolume(1f);

            string info = "";
            if (gsm.playerPoints[0] >= 9)
                info = "YOU'RE WINNING BIG!";
            else if (gsm.playerPoints[0] > 0 && gsm.playerPoints[0] < 9)
                info = "YOU'RE WINNING SOME COINS";
            else if (gsm.playerPoints[0] > -9 && gsm.playerPoints[0] < 0)
                info = "YOU'RE LOSING SOME COINS";
            else if (gsm.playerPoints[0] >= 9)
                info = "YOU'RE LOSING BIG!";

            int coinResult = gsm.playerPoints[0] * gsm.coinBetCount;

            gsm.GM.UI.ShowCoinResult(true, coinResult);
            gsm.GM.AddPlayesCoins(coinResult);
            gsm.GM.UI.ShowTagline(true, info);

            for (int i = 0; i < gsm.playerPoints.Length; i++)
            {
                int charIndex = gsm.GM.UI.playerAvatarOrder[i];
                if (gsm.playerPoints[i] > 0)
                    gsm.charSpriteObj[i].sprite = gsm.GM.UI.charSpriteHappy[charIndex];
                else if (gsm.playerPoints[i] < 0)
                    gsm.charSpriteObj[i].sprite = gsm.GM.UI.charSpriteSad[charIndex];
            }
            gsm.GM.StopSFX();
            gsm.PlaySFX(3);
            yield return new WaitForSeconds(gsm.sfxClip[3].length);
            gsm.PlaySFX(Random.Range(4, 10));

            yield return new WaitForSeconds(1.5f);

            for (int i = 0; i < gsm.playerCount; i++)
            {
                ShowPlayerInfo(i, "", false);
            }
            gsm.GM.UI.ShowCoinResult(false);
            gsm.GM.UI.ShowTagline(false);
            gsm.GM.UI.SetCharacterSprite(gsm.charSpriteObj);
        }

        IEnumerator ComparePlayerCardsIE(string tag, int startIndex, int endIndex)
        {
            float yPos = 0.375f;

            gsm.GM.UI.ShowTagline(true, "CHECK " + tag + " CARDS");
            yield return new WaitForSeconds(0.5f);
            gsm.GM.UI.ShowTagline(false);

            List<List<GameObject>> cardSetObj = new List<List<GameObject>>();
            List<List<Card>> cardSet = new List<List<Card>>();

            for (int i = 0; i < gsm.playerCount; i++)
            {
                List<Card> newSet = new List<Card>();
                List<GameObject> newObj = new List<GameObject>();

                for (int j = startIndex; j < endIndex; j++)
                {
                    newSet.Add(gsm.playerCardOnHand[i][j]);
                    newObj.Add(gsm.playerCardObjOnHand[i][j]);
                }
                cardSet.Add(newSet);
                cardSetObj.Add(newObj);

                ShowPlayerInfo(i);
            }

            for (int i = 0; i < gsm.playerCount - 1; i++)
            {
                int point1 = gsm.GetHighestComboSetPoint(cardSet[i], out string combo1);
                int point2;
                for (int j = i + 1; j < gsm.playerCount; j++)
                {
                    point2 = gsm.GetHighestComboSetPoint(cardSet[j], out string combo2);
                    int charIndex1 = gsm.GM.UI.playerAvatarOrder[i];
                    int charIndex2 = gsm.GM.UI.playerAvatarOrder[j];

                    if (point1 != point2)
                    {
                        if (point1 > point2)
                        {
                            gsm.playerPoints[i]++;
                            gsm.charSpriteObj[i].sprite = gsm.GM.UI.charSpriteHappy[charIndex1];

                            gsm.playerPoints[j]--;
                            gsm.charSpriteObj[j].sprite = gsm.GM.UI.charSpriteSad[charIndex2];
                        }
                        else
                        {
                            gsm.playerPoints[i]--;
                            gsm.charSpriteObj[i].sprite = gsm.GM.UI.charSpriteSad[charIndex1];

                            gsm.playerPoints[j]++;
                            gsm.charSpriteObj[j].sprite = gsm.GM.UI.charSpriteHappy[charIndex2];
                        }
                    }

                    HighlightCardSet(cardSetObj, i, yPos);
                    HighlightCardSet(cardSetObj, j, yPos);
                    ShowPlayerInfo(i, combo1);
                    ShowPlayerInfo(j, combo2);

                    yield return new WaitForSeconds(0.75f);

                    HighlightCardSet(cardSetObj, i, -yPos);
                    HighlightCardSet(cardSetObj, j, -yPos);
                    SetPlayerInfo(i);
                    SetPlayerInfo(j);

                    gsm.charSpriteObj[i].sprite = gsm.GM.UI.charSpriteIdle[charIndex1];
                    gsm.charSpriteObj[j].sprite = gsm.GM.UI.charSpriteIdle[charIndex2];

                    yield return new WaitForSeconds(0.5f);
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        void HighlightCardSet(List<List<GameObject>> cardSetObj, int playerIndex, float yPos)
        {
            cardSetObj[playerIndex].ForEach(obj =>
            {
                Vector3 newPos = new Vector3(obj.transform.position.x, obj.transform.position.y + yPos, 0);
                obj.transform.position = newPos;
                if (yPos > 0)
                {
                    obj.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 1, 1).SetEase(Ease.Linear);
                }
            });
        }

        void ShowPlayerInfo(int playerIndex, string info = "", bool isShow = true)
        {
            ShowPlayerPoint(playerIndex);
            SetPlayerInfo(playerIndex, info);
            gsm.playerInfo[playerIndex].SetActive(isShow);
            gsm.txtPlayerInfo[playerIndex].transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 1, 1)
                .SetEase(Ease.Linear);
        }

        void ShowPlayerPoint(int playerIndex)
        {
            string numSign = gsm.playerPoints[playerIndex] < 0 ? "" : "+";
            gsm.txtPlayerPoint[playerIndex].text = numSign + gsm.playerPoints[playerIndex].ToString();
        }

        void SetPlayerInfo(int playerIndex, string info = "")
        {
            gsm.txtPlayerInfo[playerIndex].text = info;
        }
    }
}