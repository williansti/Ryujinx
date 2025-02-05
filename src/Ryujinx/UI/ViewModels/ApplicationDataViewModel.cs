using Gommon;
using Ryujinx.Ava.Utilities.AppLibrary;

namespace Ryujinx.Ava.UI.ViewModels
{
    public class ApplicationDataViewModel : BaseModel
    {
        private const string FormatVersion = "Current Version: {0}";
        private const string FormatDeveloper = "Developed by {0}";
        
        private const string FormatExtension = "Game type: {0}";
        private const string FormatLastPlayed = "Last played: {0}";
        private const string FormatPlayTime = "Play time: {0}";
        private const string FormatSize = "Size: {0}";

        private const string FormatHostedGames = "Hosted Games: {0}";
        private const string FormatPlayerCount = "Online Players: {0}";
        
        public ApplicationData AppData { get; }

        public ApplicationDataViewModel(ApplicationData appData) => AppData = appData;

        public string FormattedVersion => FormatVersion.Format(AppData.Version);
        public string FormattedDeveloper => FormatDeveloper.Format(AppData.Developer);
        
        public string FormattedFileExtension => FormatExtension.Format(AppData.FileExtension);
        public string FormattedLastPlayed => FormatLastPlayed.Format(AppData.LastPlayedString);
        public string FormattedPlayTime => FormatPlayTime.Format(AppData.TimePlayedString);
        public string FormattedFileSize => FormatSize.Format(AppData.FileSizeString);
        
        public string FormattedLdnInfo => 
            $"{FormatHostedGames.Format(AppData.GameCount)}\n{FormatPlayerCount.Format(AppData.PlayerCount)}";
    }
}
