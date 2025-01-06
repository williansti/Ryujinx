using Gommon;
using nietras.SeparatedValues;
using Ryujinx.Ava.Common.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ryujinx.Ava.Utilities.Compat
{
    public class CompatibilityCsv
    {
        public static CompatibilityCsv Shared { get; set; }
        
        public CompatibilityCsv(SepReader reader)
        {
            var entries = new List<CompatibilityEntry>();

            foreach (var row in reader)
            {
                entries.Add(new CompatibilityEntry(reader.Header, row));
            }

            Entries = entries.Where(x => x.Status != null)
                .OrderBy(it => it.GameName).ToArray();
        }

        public CompatibilityEntry[] Entries { get; }
    }

    public class CompatibilityEntry
    {
        public CompatibilityEntry(SepReaderHeader header, SepReader.Row row)
        {
            IssueNumber = row[header.IndexOf("issue_number")].Parse<int>();

            var titleIdRow = row[header.IndexOf("extracted_game_id")].ToString();
            TitleId = !string.IsNullOrEmpty(titleIdRow) 
                ? titleIdRow 
                : default(Optional<string>);

            var issueTitleRow = row[header.IndexOf("issue_title")].ToString();
            if (TitleId.HasValue)
                issueTitleRow = issueTitleRow.ReplaceIgnoreCase($" - {TitleId}", string.Empty);

            GameName = issueTitleRow.Trim().Trim('"');

            IssueLabels = row[header.IndexOf("issue_labels")].ToString().Split(';');
            Status = row[header.IndexOf("extracted_status")].ToString().ToLower() switch
            {
                "playable" => LocaleKeys.CompatibilityListPlayable,
                "ingame" => LocaleKeys.CompatibilityListIngame,
                "menus" => LocaleKeys.CompatibilityListMenus,
                "boots" => LocaleKeys.CompatibilityListBoots,
                "nothing" => LocaleKeys.CompatibilityListNothing,
                _ => null
            };

            if (row[header.IndexOf("last_event_date")].TryParse<DateTime>(out var dt))
                LastEvent = dt;

            if (row[header.IndexOf("events_count")].TryParse<int>(out var eventsCount))
                EventCount = eventsCount;
        }

        public int IssueNumber { get; }
        public string GameName { get; }
        public Optional<string> TitleId { get; }
        public string[] IssueLabels { get; }
        public LocaleKeys? Status { get; }
        public DateTime LastEvent { get; }
        public int EventCount { get; }

        public string LocalizedStatus => LocaleManager.Instance[Status!.Value];
        public string FormattedTitleId => TitleId.OrElse(new string(' ', 16));

        public string FormattedIssueLabels => IssueLabels
            .Where(it => !it.StartsWithIgnoreCase("status"))
            .Select(FormatLabelName)
            .JoinToString(", ");

        public override string ToString()
        {
            var sb = new StringBuilder("CompatibilityEntry: {");
            sb.Append($"{nameof(IssueNumber)}={IssueNumber}, ");
            sb.Append($"{nameof(GameName)}=\"{GameName}\", ");
            sb.Append($"{nameof(TitleId)}={TitleId}, ");
            sb.Append($"{nameof(IssueLabels)}=\"{IssueLabels}\", ");
            sb.Append($"{nameof(Status)}=\"{Status}\", ");
            sb.Append($"{nameof(LastEvent)}=\"{LastEvent}\", ");
            sb.Append($"{nameof(EventCount)}={EventCount}");
            sb.Append('}');

            return sb.ToString();
        }

        public static string FormatLabelName(string labelName) => labelName.ToLower() switch
        {
            "audio" => "Audio",
            "bug" => "Bug",
            "cpu" => "CPU",
            "gpu" => "GPU",
            "gui" => "GUI",
            "help wanted" => "Help Wanted",
            "horizon" => "Horizon",
            "infra" => "Project Infra",
            "invalid" => "Invalid",
            "kernel" => "Kernel",
            "ldn" => "LDN",
            "linux" => "Linux",
            "macos" => "macOS",
            "question" => "Question",
            "windows" => "Windows",
            "graphics-backend:opengl" => "Graphics: OpenGL",
            "graphics-backend:vulkan" => "Graphics: Vulkan",
            "ldn-works" => "LDN Works",
            "ldn-untested" => "LDN Untested",
            "ldn-broken" => "LDN Broken",
            "ldn-partial" => "Partial LDN",
            "nvdec" => "NVDEC",
            "services" => "NX Services",
            "services-horizon" => "Horizon OS Services",
            "slow" => "Runs Slow",
            "crash" => "Crashes",
            "deadlock" => "Deadlock",
            "regression" => "Regression",
            "opengl" => "OpenGL",
            "opengl-backend-bug" => "OpenGL Backend Bug",
            "vulkan-backend-bug" => "Vulkan Backend Bug",
            "mac-bug" => "Mac-specific Bug(s)",
            "amd-vendor-bug" => "AMD GPU Bug",
            "intel-vendor-bug" => "Intel GPU Bug",
            "loader-allocator" => "Loader Allocator",
            "audout" => "AudOut",
            "32-bit" => "32-bit Game",
            "UE4" => "Unreal Engine 4",
            "homebrew" => "Homebrew Content",
            "online-broken" => "Online Broken",
            _ => Capitalize(labelName)
        };
        
        public static string Capitalize(string value)
        {
            if (value == string.Empty)
                return string.Empty;
        
            var firstChar = value[0];
            var rest = value[1..];

            return $"{char.ToUpper(firstChar)}{rest}";
        }
    }
}
