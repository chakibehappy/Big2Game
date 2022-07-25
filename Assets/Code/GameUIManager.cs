using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Big2Game
{
    public class GameUIManager : MonoBehaviour
    {
        private GameMaster GM;

        public Button centerButton;
        public TextMeshProUGUI txtCenterButton;

        public delegate void OnCenterButtonPress();
        public event OnCenterButtonPress OnCenterButtonEvent;

        public Button confirmButton;
        public TextMeshProUGUI txtConfirmButton;

        public delegate void OnConfirmPress();
        public event OnConfirmPress OnConfirmButtonEvent;

        public Button backButton;

        public delegate void OnBackPress();
        public event OnBackPress OnBackButtonEvent;

        public Button exitButton;

        public delegate void OnExitPress();
        public event OnExitPress OnExitButtonEvent;

        public TextMeshProUGUI txtTagLine;

        [HideInInspector]
        public int selectedCharacter;

        public List<Sprite> charSpriteIdle;
        public List<Sprite> charSpriteHappy;
        public List<Sprite> charSpriteSad;

        public int[] playerAvatarOrder = new int[4];

        public GameObject playerCoinUI;
        public TextMeshProUGUI txtPlayerCoin;

        public GameObject coinResultUI;
        public TextMeshProUGUI txtCoinResult;

        private void Awake()
        {
            ShowConfirmButton(false);
            ShowTagline(false);
            AssignConfirmButton();
        }

        void Start()
        {
            GM = GameMaster.Instance;
        }

        void AssignConfirmButton()
        {
            centerButton.onClick.RemoveAllListeners();
            centerButton.onClick.AddListener(() => { CenterButtonPress(); });

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => { ConfirmButtonPress(); });

            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => { BackButtonPress(); });

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() => { ExitButtonPress(); });
        }

        void CenterButtonPress()
        {
            GM.PlayButtonSFX(0);
            OnCenterButtonEvent?.Invoke();
        }

        public void ShowCenterButton(bool isShow, string buttonName = "OK")
        {
            txtCenterButton.text = buttonName;
            centerButton.gameObject.SetActive(isShow);
        }


        void ConfirmButtonPress()
        {
            GM.PlayButtonSFX(1);
            OnConfirmButtonEvent?.Invoke();
        }

        public void ShowConfirmButton(bool isShow, string buttonName = "OK")
        {
            txtConfirmButton.text = buttonName;
            confirmButton.gameObject.SetActive(isShow);
        }

        void BackButtonPress()
        {
            GM.PlayButtonSFX(2);
            OnBackButtonEvent?.Invoke();
        }

        public void ShowBackButton(bool isShow = true)
        {
            backButton.gameObject.SetActive(isShow);
        }

        void ExitButtonPress()
        {
            OnBackButtonEvent?.Invoke();
        }

        public void ShowExitButton(bool isShow = true)
        {
            exitButton.gameObject.SetActive(isShow);
        }

        public void HideAllButtons()
        {
            ShowCenterButton(false);
            ShowConfirmButton(false);
            ShowBackButton(false);
        }

        public void ShowTagline(bool isShow = true, string text = "")
        {
            txtTagLine.text = text;
            txtTagLine.gameObject.SetActive(isShow);
        }

        #region Character UI Helper
        public void SetCharacterSprite(SpriteRenderer[] spriteObj)
        {
            List<int> order = new List<int>() { 0, 1, 2, 3 };
            playerAvatarOrder[0] = selectedCharacter;
            spriteObj[0].sprite = charSpriteIdle[selectedCharacter];

            order.Remove(selectedCharacter);
            
            for (int i = 1; i < spriteObj.Length; i++)
            {
                playerAvatarOrder[i] = order[i - 1];
                spriteObj[i].sprite = charSpriteIdle[order[i - 1]];
            }
        }
        #endregion


        public void ShowPlayerCoins(bool isShow = true, int coins = 0)
        {
            txtPlayerCoin.text = coins.ToString();
            playerCoinUI.SetActive(isShow);
        }

        public void ShowCoinResult(bool isShow = true, int coins = 0)
        {
            txtCoinResult.text = coins.ToString();
            coinResultUI.SetActive(isShow);
        }

    }
}
