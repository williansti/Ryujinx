using Gommon;
using nietras.SeparatedValues;
using Ryujinx.Common.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ryujinx.Ava.Utilities.Compat
{
    public static class CompatibilityHelper
    {
        private static readonly string _downloadUrl =
            "https://gist.githubusercontent.com/ezhevita/b41ed3bf64d0cc01269cab036e884f3d/raw/002b1a1c1a5f7a83276625e8c479c987a5f5b722/Ryujinx%2520Games%2520List%2520Compatibility.csv";
        
        private static readonly FilePath _compatCsvPath = new FilePath(AppDataManager.BaseDirPath) / "system" / "compatibility.csv";

        public static async Task<SepReader> DownloadAsync()
        {
            if (_compatCsvPath.ExistsAsFile)
                return Sep.Reader().FromFile(_compatCsvPath.Path);
            
            using var httpClient = new HttpClient();
            var compatCsv = await httpClient.GetStringAsync(_downloadUrl);
            _compatCsvPath.WriteAllText(compatCsv);
            return Sep.Reader().FromText(compatCsv);
        }

        public static async Task InitAsync()
        {
            CompatibilityCsv.Shared = new CompatibilityCsv(await DownloadAsync());
        }
    }
}
