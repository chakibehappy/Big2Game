using UnityEngine;
using UnityEngine.SceneManagement;

namespace Big2Game.CharacterSelect
{
    public class MainMenu : MonoBehaviour
    {
        private GameMaster GM;
        private void Start()
        {
            GM = GameMaster.Instance;
            GM.UI.ShowCenterButton(true, "START GAME");
            GM.UI.ShowExitButton();

            GM.UI.OnCenterButtonEvent += GoToNextScene;
            GM.UI.OnExitButtonEvent += ExitApplication;
            
            GM.PlayBGM();
        }

        private void OnDestroy()
        {
            GM.UI.OnCenterButtonEvent -= GoToNextScene;
            GM.UI.OnExitButtonEvent -= ExitApplication;
        }

        void GoToNextScene()
        {
            GM.UI.ShowTagline(false);
            GM.UI.ShowCenterButton(false);
            GM.UI.ShowExitButton(false);
            SceneManager.LoadScene("CharacterSelect");
        }

        void ExitApplication()
        {
            Application.Quit();
        }

    }
}