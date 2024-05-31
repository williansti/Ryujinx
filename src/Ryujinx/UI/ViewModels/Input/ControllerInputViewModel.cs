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
        private const int DrawStickPollRate = 50; // Milliseconds per poll.
        private const int DrawStickCircumference = 5;
        private const float DrawStickScaleFactor = DrawStickCanvasCenter;

        private const int DrawStickCanvasSize = 100;
        private const int DrawStickBorderSize = DrawStickCanvasSize + 5;
        private const float DrawStickCanvasCenter = (DrawStickCanvasSize - DrawStickCircumference) / 2;

        private const float MaxVectorLength = DrawStickCanvasSize / 2;
        
        private IGamepad _selectedGamepad;

        private StickVisualizer _stickVisualizer;
        public StickVisualizer StickVisualizer
        {
            get => _stickVisualizer;
            set
            {
                _stickVisualizer = value;

                OnPropertyChanged();
            }
        }
        
        private GamepadInputConfig _config;
        public GamepadInputConfig Config
        {
            get => _config;
            set
            {
                _config = value;
                StickVisualizer.UpdateConfig(Config);

                OnPropertyChanged();
            }
        }
        
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
        
        private (float, float) _uiStickLeft;
        
        public (float, float) UiStickLeft
        {
            get => (_uiStickLeft.Item1 * DrawStickScaleFactor, _uiStickLeft.Item2 * DrawStickScaleFactor);
            set
            {
                _uiStickLeft = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(UiStickRightX));
                OnPropertyChanged(nameof(UiStickRightY));
                OnPropertyChanged(nameof(UiDeadzoneRight));
            }
        }

        private (float, float) _uiStickRight;
        public (float, float) UiStickRight
        {
            get => (_uiStickRight.Item1 * DrawStickScaleFactor, _uiStickRight.Item2 * DrawStickScaleFactor);
            set
            {
                _uiStickRight = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(UiStickLeftX));
                OnPropertyChanged(nameof(UiStickLeftY));
                OnPropertyChanged(nameof(UiDeadzoneLeft));
            }
        }

        public int UiStickCircumference => DrawStickCircumference;
        public int UiCanvasSize => DrawStickCanvasSize;
        public int UiStickBorderSize => DrawStickBorderSize;
        
        public float UiStickLeftX => ClampVector(UiStickLeft).Item1;
        public float UiStickLeftY => ClampVector(UiStickLeft).Item2;
        public float UiStickRightX => ClampVector(UiStickRight).Item1;
        public float UiStickRightY => ClampVector(UiStickRight).Item2;

        public float UiDeadzoneLeft => Config.DeadzoneLeft * DrawStickCanvasSize - DrawStickCircumference;
        public float UiDeadzoneRight => Config.DeadzoneRight * DrawStickCanvasSize - DrawStickCircumference;
        
        public ControllerInputViewModel(InputViewModel model, GamepadInputConfig config)
        {
            ParentModel = model;
            model.NotifyChangesEvent += OnParentModelChanged;
            OnParentModelChanged();
            _stickVisualizer = new();
            Config = config;

            StickVisualizer.PollToken = StickVisualizer.PollTokenSource.Token;

            Task.Run(() => PollSticks(StickVisualizer.PollToken));
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
                    StickVisualizer.UiStickLeft = _selectedGamepad.GetStick(StickInputId.Left);
                    StickVisualizer.UiStickRight = _selectedGamepad.GetStick(StickInputId.Right);
                }

                await Task.Delay(StickVisualizer.DrawStickPollRate, token);
            }

            StickVisualizer.PollTokenSource.Dispose();
        }

        public void OnParentModelChanged()
        {
            IsLeft = ParentModel.IsLeft;
            IsRight = ParentModel.IsRight;
            Image = ParentModel.Image;
        }
    }
}
