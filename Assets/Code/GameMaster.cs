using UnityEngine;
using UnityEngine.SceneManagement;

namespace Big2Game
{
    public class GameMaster : MonoBehaviour
    {
        #region Singleton
        public static GameMaster Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                PlayBGM();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        public GameUIManager UI;
        public ObjectPooler ObjectPooler;

        public int Coins = 30;
        public int[] playerAvatarOrder;

        public AudioSource BGM;
        public AudioSource SFX;

        public AudioClip[] UISfxClip;
        public AudioClip bgmClip;

        private void Start()
        {
            SetScreenRatio();
            SceneManager.sceneLoaded += CheckScreenRatio;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= CheckScreenRatio;
        }

        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        void CheckScreenRatio(Scene scene, LoadSceneMode loadSceneMode)
        {
            SetScreenRatio();
        }

        void SetScreenRatio()
        {
            if (Camera.main.aspect <= 1.4f)
                Camera.main.orthographicSize = 7f;
            else
                Camera.main.orthographicSize = 5f;
        }

        public void ShowPlayerStats()
        {
            UI.ShowPlayerCoins(true, Coins);
        }

        public void AddPlayesCoins(int coin, bool isShowUI = true)
        {
            Coins += coin;
            if (Coins < 0)
                Coins = 0;
            ShowPlayerStats();
        }

        public void SetBGMVolume(float vol)
        {
            BGM.volume = vol;
        }

        public void PlayBGM()
        {
            if (BGM.isPlaying)
                return;

            BGM.clip = bgmClip;
            BGM.ignoreListenerVolume = true;
            BGM.Play();
        }

        public void PlaySFX(AudioClip clip, bool isLooping = false)
        {
            SFX.loop = isLooping;
            if (isLooping)
            {
                SFX.clip = clip;
                SFX.Play();
            }
            else
            {
                SFX.PlayOneShot(clip);
            }
        }

        public void StopSFX()
        {
            SFX.Stop();
        }

        public void PlayButtonSFX(int index)
        {
            PlaySFX(UISfxClip[index]);
        }
    }
}
