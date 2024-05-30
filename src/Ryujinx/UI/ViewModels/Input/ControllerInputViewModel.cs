using Avalonia.Svg.Skia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Ryujinx.Ava.UI.Helpers;
using Ryujinx.Ava.Input;
using Ryujinx.Ava.UI.Models.Input;
using Ryujinx.Ava.UI.Views.Input;
using Ryujinx.Input;
using System.Threading;
using System.Threading.Tasks;

namespace Ryujinx.Ava.UI.ViewModels.Input
{
    public partial class ControllerInputViewModel : BaseModel
    {
        [ObservableProperty] private GamepadInputConfig _config;
        
        private const int StickUiPollMs = 50; // Milliseconds per poll.
        private const float CanvasCenterOffset = 75f/2f;
        private const int StickScaleFactor = 30;

        private IGamepad _selectedGamepad;
        
        // Offset from origin for UI stick visualization.
        private (float, float) _uiStickLeft;
        private (float, float) _uiStickRight;

        internal CancellationTokenSource _pollTokenSource = new();
        private CancellationToken _pollToken;

        private bool _isLeft;
        public bool IsLeft
        {
            get => _isLeft;
            set
            {
                _isLeft = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSides));
            }
        }

        private bool _isRight;
        public bool IsRight
        {
            get => _isRight;
            set
            {
                _isRight = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSides));
            }
        }

        public bool HasSides => IsLeft ^ IsRight;

        [ObservableProperty] private SvgImage _image;
        
        public InputViewModel ParentModel { get; }
        
        public (float, float) UiStickLeft
        {
            get => (_uiStickLeft.Item1 * StickScaleFactor, _uiStickLeft.Item2 * StickScaleFactor);
            set
            {
                _uiStickLeft = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(UiStickRightX));
                OnPropertyChanged(nameof(UiStickRightY));
            }
        }
        
        public (float, float) UiStickRight
        {
            get => (_uiStickRight.Item1 * StickScaleFactor, _uiStickRight.Item2 * StickScaleFactor);
            set
            {
                _uiStickRight = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(UiStickLeftX));
                OnPropertyChanged(nameof(UiStickLeftY));
            }
        }

        public float canvasCenter => CanvasCenterOffset;

        public float UiStickLeftX => UiStickLeft.Item1 + CanvasCenterOffset;
        public float UiStickLeftY => UiStickLeft.Item2 + CanvasCenterOffset;
        public float UiStickRightX => UiStickRight.Item1 + CanvasCenterOffset;
        public float UiStickRightY => UiStickRight.Item2 + CanvasCenterOffset;

        public ControllerInputViewModel(InputViewModel model, GamepadInputConfig config)
        {
            ParentModel = model;
            model.NotifyChangesEvent += OnParentModelChanged;
            OnParentModelChanged();
            Config = config;

            _pollTokenSource = new();
            _pollToken = _pollTokenSource.Token;

            Task.Run(() => PollSticks(_pollToken));
        }

        public async void ShowMotionConfig()
        {
            await MotionInputView.Show(this);
        }

        public async void ShowRumbleConfig()
        {
            await RumbleInputView.Show(this);
        }
        
        public RelayCommand LedDisabledChanged => Commands.Create(() =>
        {
            if (!Config.EnableLedChanging) return;

            if (Config.TurnOffLed)
                ParentModel.SelectedGamepad.ClearLed();
            else
                ParentModel.SelectedGamepad.SetLed(Config.LedColor.ToUInt32());
        });

        private async Task PollSticks(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _selectedGamepad = ParentModel.SelectedGamepad;

                if (_selectedGamepad != null && _selectedGamepad is not AvaloniaKeyboard)
                {
                    UiStickLeft = _selectedGamepad.GetStick(StickInputId.Left);
                    UiStickRight = _selectedGamepad.GetStick(StickInputId.Right);
                }

                await Task.Delay(StickUiPollMs);
            }

            _pollTokenSource.Dispose();
        }

        public void OnParentModelChanged()
        {
            IsLeft = ParentModel.IsLeft;
            IsRight = ParentModel.IsRight;
            Image = ParentModel.Image;
        }
    }
}
