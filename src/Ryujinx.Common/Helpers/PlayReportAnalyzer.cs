using Gommon;
using MsgPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ryujinx.Common.Helper
{
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

        public Optional<string> Run(string runningGameId, MessagePackObject playReport)
        {
            if (!playReport.IsDictionary) 
                return Optional<string>.None;

            if (!_specs.TryGetFirst(s => s.TitleIdStr.EqualsIgnoreCase(runningGameId), out PlayReportGameSpec spec))
                return Optional<string>.None;

            foreach (PlayReportValueFormatterSpec formatSpec in spec.Analyses.OrderBy(x => x.Priority))
            {
                if (!playReport.AsDictionary().TryGetValue(formatSpec.ReportKey, out MessagePackObject valuePackObject))
                    continue;

                return formatSpec.ValueFormatter(valuePackObject.ToObject());
            }
            
            return Optional<string>.None;
        }
        
    }

    public class PlayReportGameSpec
    {
        public required string TitleIdStr { get; init; }
        public List<PlayReportValueFormatterSpec> Analyses { get; } = [];

        public PlayReportGameSpec AddValueFormatter(string reportKey, Func<object, string> valueFormatter)
        {
            Analyses.Add(new PlayReportValueFormatterSpec
            {
                Priority = Analyses.Count,
                ReportKey = reportKey, 
                ValueFormatter = valueFormatter
            });
            return this;
        }
        
        public PlayReportGameSpec AddValueFormatter(int priority, string reportKey, Func<object, string> valueFormatter)
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

    public struct PlayReportValueFormatterSpec
    {
        public required int Priority { get; init; }
        public required string ReportKey { get; init; }
        public required Func<object, string> ValueFormatter { get; init; }
    }
}
