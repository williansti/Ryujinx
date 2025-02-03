using Gommon;
using MsgPack;
using Ryujinx.Ava.Utilities.AppLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ryujinx.Ava.Utilities
{
    public class PlayReportAnalyzer
    {
        private readonly List<PlayReportGameSpec> _specs = [];

        public PlayReportAnalyzer AddSpec(string titleId, Func<PlayReportGameSpec, PlayReportGameSpec> transform)
        {
            _specs.Add(transform(new PlayReportGameSpec { TitleIds = [titleId] }));
            return this;
        }
        
        public PlayReportAnalyzer AddSpec(string titleId, Action<PlayReportGameSpec> transform)
        {
            _specs.Add(new PlayReportGameSpec { TitleIds = [titleId] }.Apply(transform));
            return this;
        }
        
        public PlayReportAnalyzer AddSpec(IEnumerable<string> titleIds, Func<PlayReportGameSpec, PlayReportGameSpec> transform)
        {
            _specs.Add(transform(new PlayReportGameSpec { TitleIds = [..titleIds] }));
            return this;
        }
        
        public PlayReportAnalyzer AddSpec(IEnumerable<string> titleIds, Action<PlayReportGameSpec> transform)
        {
            _specs.Add(new PlayReportGameSpec { TitleIds = [..titleIds] }.Apply(transform));
            return this;
        }

        public PlayReportFormattedValue Run(string runningGameId, ApplicationMetadata appMeta, MessagePackObject playReport)
        {
            if (!playReport.IsDictionary) 
                return PlayReportFormattedValue.Unhandled;

            if (!_specs.TryGetFirst(s => runningGameId.EqualsAnyIgnoreCase(s.TitleIds), out PlayReportGameSpec spec))
                return PlayReportFormattedValue.Unhandled;

            foreach (PlayReportValueFormatterSpec formatSpec in spec.Analyses.OrderBy(x => x.Priority))
            {
                if (!playReport.AsDictionary().TryGetValue(formatSpec.ReportKey, out MessagePackObject valuePackObject))
                    continue;

                PlayReportValue value = new()
                {
                    Application = appMeta, 
                    PackedValue = valuePackObject
                };

                return formatSpec.ValueFormatter(ref value);
            }
            
            return PlayReportFormattedValue.Unhandled;
        }
        
    }

    public class PlayReportGameSpec
    {
        public required string[] TitleIds { get; init; }
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

    public readonly struct PlayReportValue
    {
        public ApplicationMetadata Application { get; init; }
        
        public MessagePackObject PackedValue { get; init; }

        public object BoxedValue => PackedValue.ToObject();
    }

    public struct PlayReportFormattedValue
    {
        public bool Handled { get; private init; }
        
        public bool Reset { get; private init; }
        
        public string FormattedString { get; private init; }

        public static implicit operator PlayReportFormattedValue(string formattedValue)
            => new() { Handled = true, FormattedString = formattedValue };

        public static PlayReportFormattedValue Unhandled => default;
        public static PlayReportFormattedValue ForceReset => new() { Handled = true, Reset = true };

        public static PlayReportValueFormatter AlwaysResets = AlwaysResetsImpl;
        
        private static PlayReportFormattedValue AlwaysResetsImpl(ref PlayReportValue _) => ForceReset;
    }

    public struct PlayReportValueFormatterSpec
    {
        public required int Priority { get; init; }
        public required string ReportKey { get; init; }
        public PlayReportValueFormatter ValueFormatter { get; init; }
    }

    public delegate PlayReportFormattedValue PlayReportValueFormatter(ref PlayReportValue value);
}
