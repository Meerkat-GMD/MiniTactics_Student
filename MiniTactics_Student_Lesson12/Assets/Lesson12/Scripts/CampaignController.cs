using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniTactics.Lesson12
{
    public sealed class CampaignController : MonoBehaviour
    {
        [SerializeField] private StageCatalog _catalog;
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip[] _clips;

        private CampaignSave _state;
        private IAudioService _sound;
        private CampaignView _view;
        private GameManager _game;
        private int _activeIndex;
        private bool _isCampaignBattle;
        private bool _resultShown;

        public static CampaignController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Services.Save = new PlayerPrefsSaveService();
            _state = Services.Save.Load();
            _sound = new ClipAudioService(_source, _clips);
            Services.Audio = _state.Muted ? new NullAudioService() : _sound;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            if (Instance == this) ConnectScene();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ConnectScene();

        private void ConnectScene()
        {
            CampaignView view = Object.FindAnyObjectByType<CampaignView>();
            if (_view != null && _view == view) return;
            DisconnectScene();
            _view = view;
            _view.Clicked += OnCommand;
            _activeIndex = _catalog.IndexOf(SceneManager.GetActiveScene().name);
            _resultShown = false;
            if (_activeIndex >= 0)
            {
                _game = GameManager.Instance;
                _game.ResultChanged += OnResultChanged;
                _view.ShowBattle(_catalog.Stage(_activeIndex).DisplayName, _state.Muted);
                SetHelp(_isCampaignBattle && _activeIndex == 0);
                return;
            }
            _isCampaignBattle = false;
            if (SceneManager.GetActiveScene().name == _catalog.EndingScene) _view.ShowEnding(_state.Muted);
            else _view.ShowTitle(_state.HasProgress, _state.Muted);
        }

        private void Update()
        {
            if (_game == null || !_game.CanShowResult || _resultShown) return;
            _resultShown = true;
            Services.Audio.Play(SoundCue.Result);
            if (_game.Result == BattleResult.PlayerWon && _activeIndex == 2)
                LoadScene(_catalog.EndingScene);
            else _view.ShowResult(_game.Result == BattleResult.PlayerWon);
        }

        private void OnResultChanged(BattleResult result)
        {
            if (!_isCampaignBattle || result != BattleResult.PlayerWon) return;
            if (_activeIndex == 2) _state.Completed = true;
            else _state.StartingStageIndex = _activeIndex + 1;
            Services.Save.Save(_state);
        }

        public void NewGame()
        {
            if (_view.IsModalOpen) return;
            if (_state.HasProgress)
            {
                _view.ShowResetConfirmation(true);
                return;
            }
            StartNewGame();
        }

        public void ConfirmNewGame()
        {
            if (_view.IsResetOpen) StartNewGame();
        }

        private void StartNewGame()
        {
            _state.HasProgress = true;
            _state.StartingStageIndex = 0;
            _state.Completed = false;
            Services.Save.Save(_state);
            EnterBattle(0);
        }

        public void Continue()
        {
            if (!_state.HasProgress || _view.IsModalOpen) return;
            if (_state.Completed) LoadScene(_catalog.EndingScene);
            else EnterBattle(_state.StartingStageIndex);
        }

        public void SetMuted(bool muted)
        {
            _state.Muted = muted;
            Services.Save.Save(_state);
            Services.Audio = muted ? new NullAudioService() : _sound;
            _view.SetMuted(muted);
        }

        public void Retry()
        {
            if (_activeIndex < 0 || _view.IsModalOpen) return;
            LoadScene(_catalog.BattleScene(_activeIndex));
        }

        public void ReturnToTitle()
        {
            if (!_view.IsModalOpen) LoadScene(_catalog.TitleScene);
        }

        public void NextStage()
        {
            if (_view.IsModalOpen || !_game.CanShowResult || _game.Result != BattleResult.PlayerWon) return;
            if (_activeIndex < 2) LoadScene(_catalog.BattleScene(_activeIndex + 1));
        }

        public void SetHelp(bool show)
        {
            _view.ShowHelp(show);
            _game.SetInputBlocked(show);
        }

        private void EnterBattle(int index)
        {
            _isCampaignBattle = true;
            LoadScene(_catalog.BattleScene(index));
        }

        private void LoadScene(string name)
        {
            DisconnectScene();
            SceneManager.LoadScene(name);
        }

        private void DisconnectScene()
        {
            if (_view != null) _view.Clicked -= OnCommand;
            if (_game != null)
            {
                _game.ResultChanged -= OnResultChanged;
                _game.CancelPresentation();
            }
            _view = null;
            _game = null;
        }

        private void OnCommand(CampaignView.Command command)
        {
            switch (command)
            {
                case CampaignView.Command.NewGame:
                case CampaignView.Command.StartAgain: NewGame(); break;
                case CampaignView.Command.Continue: Continue(); break;
                case CampaignView.Command.Mute: SetMuted(!_state.Muted); break;
                case CampaignView.Command.Quit: Application.Quit(); break;
                case CampaignView.Command.Next: NextStage(); break;
                case CampaignView.Command.Retry: Retry(); break;
                case CampaignView.Command.Title: ReturnToTitle(); break;
                case CampaignView.Command.Help: SetHelp(true); break;
                case CampaignView.Command.CloseHelp: SetHelp(false); break;
                case CampaignView.Command.ConfirmReset: ConfirmNewGame(); break;
                case CampaignView.Command.CancelReset: _view.ShowResetConfirmation(false); break;
                case CampaignView.Command.EndTurn: Object.FindAnyObjectByType<BattleController>().OnEndTurnClicked(); break;
            }
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            DisconnectScene();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Services.Audio = new NullAudioService();
            Services.Save = new NullSaveService();
            Instance = null;
        }
    }
}
