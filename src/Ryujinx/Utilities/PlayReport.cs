using Ryujinx.Common.Helper;

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

        private static string BreathOfTheWild_MasterMode(object val)
            => val is 1 ? "Playing Master Mode" : "Playing Normal Mode";

        private static string SuperMarioOdyssey_AssistMode(object val)
            => val is 1 ? "Playing in Assist Mode" : "Playing in Regular Mode";

        private static string SuperMarioOdysseyChina_AssistMode(object val)
            => val is 1 ? "Playing in 帮助模式" : "Playing in 普通模式";

        private static string SuperMario3DWorldOrBowsersFury(object val)
            => val is 0 ? "Playing Super Mario 3D World" : "Playing Bowser's Fury";
        
        private static string MarioKart8Deluxe_Mode(object obj) 
            => obj switch
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
                //TODO: refactor value formatting system to pass in the name from the content archive so this can be localized properly
                _ => "Playing Mario Kart 8 Deluxe"
            };
    }
}
