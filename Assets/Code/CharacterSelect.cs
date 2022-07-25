using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Big2Game.CharacterSelect
{
    public class CharacterSelect : MonoBehaviour
    {
        private GameMaster GM;
        [SerializeField] private EventTrigger[] charTrigger;
        [SerializeField] private SpriteRenderer[] charSpriteObj;
        [SerializeField] private SpriteRenderer dimScreen;
        [SerializeField] private string taglineMessage = "Select Your Character";
        bool isFirstSelect = true;

        private void Start()
        {
            GM = GameMaster.Instance;

            GM.UI.OnConfirmButtonEvent += GoToGameScene;
            GM.UI.OnBackButtonEvent += GoToMainMenuScene;
            
            GM.UI.ShowTagline(true, taglineMessage);
            GM.UI.ShowBackButton();
            GM.UI.ShowPlayerCoins(false);

            GM.SetBGMVolume(1f);

            AssignClickableOnCharacter();
        }

        private void OnDestroy()
        {
            GM.UI.OnConfirmButtonEvent -= GoToGameScene;
            GM.UI.OnBackButtonEvent -= GoToMainMenuScene;
        }

        void AssignClickableOnCharacter()
        {
            for (int i = 0; i < charTrigger.Length; i++)
            {
                EventTrigger.Entry onSelectCharacter = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerDown
                };
                int index = i;
                onSelectCharacter.callback.AddListener((data) => { SelectCharacter(index); });

                charTrigger[i].triggers.Clear();
                charTrigger[i].triggers.Add(onSelectCharacter);
            }
        }

        void SelectCharacter(int characterIndex)
        {
            GM.UI.ShowTagline(false);
            HighlightSelectedCharacter(characterIndex);
            StartCoroutine(SelectCharacterIE(characterIndex));
        }
        IEnumerator SelectCharacterIE(int characterIndex)
        {
            GM.PlayButtonSFX(0);

            if (isFirstSelect)
            {
                dimScreen.DOFade(0.5f, 1f).SetEase(Ease.Linear);
                isFirstSelect = false;
                yield return new WaitForSeconds(1f);
            }
            GM.UI.selectedCharacter = characterIndex;
            GM.UI.ShowConfirmButton(true, "START");
        }

        void HighlightSelectedCharacter(int index)
        {
            for (int i = 0; i < charTrigger.Length; i++)
            {
                bool isSelectedCharacter = i == index;
                charSpriteObj[i].sprite = isSelectedCharacter ? GM.UI.charSpriteHappy[i] : GM.UI.charSpriteIdle[i];
                charSpriteObj[i].sortingOrder = i == index ? 3 : 1;
            }
        }

        void GoToGameScene()
        {
            GM.UI.ShowTagline(false);
            GM.UI.ShowConfirmButton(false);
            SceneManager.LoadScene("GameTable");
        }

        void GoToMainMenuScene()
        {
            GM.UI.ShowTagline(false);
            GM.UI.HideAllButtons();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
