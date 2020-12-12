using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.ClientServer.Reporting;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.DocEngine;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchReports : AnalysisBase
    {
        private List<ReportResult> ReportResults = new List<ReportResult>();
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override void LoadCache()
        {
            Sessions.Session def = Session.DefaultInstance;
            FSExplorer rpt = new FSExplorer(def);
            ReportMainControl r = new ReportMainControl(def, false);
            ReportIFSExplorer ifsExplorer = new ReportIFSExplorer(r, def);
            FileSystemEntry entry = new FileSystemEntry("\\", FileSystemEntry.Types.Folder, (string)null);
            FileSystemEntry[] entries = ifsExplorer.GetFileSystemEntries(entry);
            bool done = false;
            while (!done)
            {
                foreach (FileSystemEntry e in entries)
                {
                    Recurse(e, ifsExplorer);
                }
                done = true;
            }
            Cache = ReportResults;
        }

        public override AnalysisResult SearchResults(string Search)
        {
            List<ReportResult> cache = (List<ReportResult>)Cache;
            List<ReportResult> Results = cache.Where(x => (x.Filters?.Any(y => y.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase)) ?? false) || (x.Columns?.Any(z => z.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase)) ?? false)).ToList();

            return new AnalysisResult(nameof(SearchReports)) { Result = Results };

        }

        private void Recurse(FileSystemEntry Entry, ReportIFSExplorer IFSExplorer)
        {
            if (Entry.Type.Equals(FileSystemEntry.Types.File))
            {
                ReportSettings settings = Session.ReportManager.GetReportSettings(Entry);
                if (settings.Columns != null)
                    ReportResults.Add(new ReportResult()
                    {
                        Columns = settings.Columns,
                        Name = Entry.Name,
                        Path = Entry.Path,
                        MatchingProperty = "Column"
                    });
            

                if(settings.Filters != null) 
                    ReportResults.AddRange(settings.Filters.Select(x => new ReportResult()
                                                            {
                                                                Filters = settings.Filters,
                                                                Name = Entry.Name, 
                                                                Path = Entry.Path, 
                                                                MatchingProperty = $"Filter [{x.FieldID}] {x.OperatorTypeAsString} {x.ValueFrom} {x.ValueTo}"
                                                            }));

            }
            else
            {
                foreach(FileSystemEntry file in IFSExplorer.GetFileSystemEntries(Entry))
                {
                    Recurse(file, IFSExplorer);
                }
            }
        }
    }
}
