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
            GM.UI.OnCenterButtonEvent += GoToNextScene;
            GM.PlayBGM();
        }

        private void OnDestroy()
        {
            GM.UI.OnCenterButtonEvent -= GoToNextScene;
        }

        void GoToNextScene()
        {
            GM.UI.ShowTagline(false);
            GM.UI.ShowCenterButton(false);
            SceneManager.LoadScene("CharacterSelect");
        }

    }
}