using Gommon;
using MsgPack;
using Ryujinx.Ava.Utilities.AppLibrary;
using Ryujinx.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ryujinx.Ava.Utilities
{
    public static class PlayReport
    {
        public static PlayReportAnalyzer Analyzer { get; } = new PlayReportAnalyzer()
            .AddSpec(
                "01007ef00011e000",
                spec => spec.AddValueFormatter("IsHardMode", BreathOfTheWild_MasterMode)
            )
            .AddSpec( // Super Mario Odyssey
                "0100000000010000",
                spec =>
                    spec.AddValueFormatter("is_kids_mode", SuperMarioOdyssey_AssistMode)
            )
            .AddSpec( // Super Mario Odyssey (China)
                "010075000ECBE000",
                spec =>
                    spec.AddValueFormatter("is_kids_mode", SuperMarioOdysseyChina_AssistMode)
            )
            .AddSpec( // Super Mario 3D World + Bowser's Fury
                "010028600EBDA000",
                spec => spec.AddValueFormatter("mode", SuperMario3DWorldOrBowsersFury)
            )
            .AddSpec( // Mario Kart 8 Deluxe
                "0100152000022000",
                spec => spec.AddValueFormatter("To", MarioKart8Deluxe_Mode)
            )
            .AddSpec( // Mario Kart 8 Deluxe (China)
                "010075100E8EC000",
                spec => spec.AddValueFormatter("To", MarioKart8Deluxe_Mode)
            );

        private static string BreathOfTheWild_MasterMode(ref PlayReportValue value)
            => value.BoxedValue is 1 ? "Playing Master Mode" : "Playing Normal Mode";

        private static string SuperMarioOdyssey_AssistMode(ref PlayReportValue value)
            => value.BoxedValue is 1 ? "Playing in Assist Mode" : "Playing in Regular Mode";

        private static string SuperMarioOdysseyChina_AssistMode(ref PlayReportValue value)
            => value.BoxedValue is 1 ? "Playing in 帮助模式" : "Playing in 普通模式";

        private static string SuperMario3DWorldOrBowsersFury(ref PlayReportValue value)
            => value.BoxedValue is 0 ? "Playing Super Mario 3D World" : "Playing Bowser's Fury";
        
        private static string MarioKart8Deluxe_Mode(ref PlayReportValue value) 
            => value.BoxedValue switch
            {
                // Single Player
                "Single" => "Single Player",
                // Multiplayer
                "Multi-2players" => "Multiplayer 2 Players",
                "Multi-3players" => "Multiplayer 3 Players",
                "Multi-4players" => "Multiplayer 4 Players",
                // Wireless/LAN Play
                "Local-Single" => "Wireless/LAN Play",
                "Local-2players" => "Wireless/LAN Play 2 Players",
                // CC Classes
                "50cc" => "50cc",
                "100cc" => "100cc",
                "150cc" => "150cc",
                "Mirror" => "Mirror (150cc)",
                "200cc" => "200cc",
                // Modes
                "GrandPrix" => "Grand Prix",
                "TimeAttack" => "Time Trials",
                "VS" => "VS Races",
                "Battle" => "Battle Mode",
                "RaceStart" => "Selecting a Course",
                "Race" => "Racing",
                _ => $"Playing {value.Application.Title}"
            };
    }

    #region Analyzer implementation
    
    public class PlayReportAnalyzer
    {
        private readonly List<PlayReportGameSpec> _specs = [];

        public PlayReportAnalyzer AddSpec(string titleId, Func<PlayReportGameSpec, PlayReportGameSpec> transform)
        {
            _specs.Add(transform(new PlayReportGameSpec { TitleIdStr = titleId }));
            return this;
        }
        
        public PlayReportAnalyzer AddSpec(string titleId, Action<PlayReportGameSpec> transform)
        {
            _specs.Add(new PlayReportGameSpec { TitleIdStr = titleId }.Apply(transform));
            return this;
        }

        public Optional<string> Run(string runningGameId, ApplicationMetadata appMeta, MessagePackObject playReport)
        {
            if (!playReport.IsDictionary) 
                return Optional<string>.None;

            if (!_specs.TryGetFirst(s => s.TitleIdStr.EqualsIgnoreCase(runningGameId), out PlayReportGameSpec spec))
                return Optional<string>.None;

            foreach (PlayReportValueFormatterSpec formatSpec in spec.Analyses.OrderBy(x => x.Priority))
            {
                if (!playReport.AsDictionary().TryGetValue(formatSpec.ReportKey, out MessagePackObject valuePackObject))
                    continue;

                PlayReportValue value = new()
                {
                    Application = appMeta, 
                    BoxedValue = valuePackObject.ToObject()
                };

                return formatSpec.ValueFormatter(ref value);
            }
            
            return Optional<string>.None;
        }
        
    }

    public class PlayReportGameSpec
    {
        public required string TitleIdStr { get; init; }
        public List<PlayReportValueFormatterSpec> Analyses { get; } = [];

        public PlayReportGameSpec AddValueFormatter(string reportKey, PlayReportValueFormatter valueFormatter)
        {
            Analyses.Add(new PlayReportValueFormatterSpec
            {
                Priority = Analyses.Count,
                ReportKey = reportKey, 
                ValueFormatter = valueFormatter
            });
            return this;
        }
        
        public PlayReportGameSpec AddValueFormatter(int priority, string reportKey, PlayReportValueFormatter valueFormatter)
        {
            Analyses.Add(new PlayReportValueFormatterSpec
            {
                Priority = priority,
                ReportKey = reportKey, 
                ValueFormatter = valueFormatter
            });
            return this;
        }
    }

    public struct PlayReportValue
    {
        public ApplicationMetadata Application { get; init; }
        public object BoxedValue { get; init; }
    }

    public struct PlayReportValueFormatterSpec
    {
        public required int Priority { get; init; }
        public required string ReportKey { get; init; }
        public required PlayReportValueFormatter ValueFormatter { get; init; }
    }

    public delegate string PlayReportValueFormatter(ref PlayReportValue value);
    
    #endregion
}
