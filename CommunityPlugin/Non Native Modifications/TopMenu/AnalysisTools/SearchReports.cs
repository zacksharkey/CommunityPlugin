using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.ClientServer.Reporting;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu.AnalysisTools
{
    public class SearchReports : AnalysisBase
    {
        private List<ReportResult> ReportResults;
        public override AnalysisResult ExecuteTest() { return null; }

        public override bool IsTest() { return false; }

        public override AnalysisResult SearchResults(string Search)
        {
            ReportResults = new List<ReportResult>();
            Sessions.Session def = Session.DefaultInstance;
            FSExplorer rpt = new FSExplorer(def);
            ReportMainControl r = new ReportMainControl(def, false);
            ReportIFSExplorer ifsExplorer = new ReportIFSExplorer(r, def);
            FileSystemEntry entry = new FileSystemEntry("\\", FileSystemEntry.Types.Folder, (string)null);
            FileSystemEntry[] entries = ifsExplorer.GetFileSystemEntries(entry);
            foreach(FileSystemEntry e in entries)
            {
                Recurse(e, ifsExplorer, Search);
            }

            return new AnalysisResult(nameof(SearchReports)) { Result = ReportResults };

        }

        private void Recurse(FileSystemEntry Entry, ReportIFSExplorer IFSExplorer, string Search)
        {
            if(Entry.Type.Equals(FileSystemEntry.Types.File))
            {
                ReportSettings settings = Session.ReportManager.GetReportSettings(Entry);
                if(settings.Columns != null)
                    ReportResults.AddRange(settings.Columns.Where(x => x.ID.Equals(Search, StringComparison.OrdinalIgnoreCase))
                                                           .Select(x => new ReportResult() 
                                                           { 
                                                               Name = Entry.Name, 
                                                               Path = Entry.Path, 
                                                               MatchingProperty = "Column" 
                                                           }));
            

                if(settings.Filters != null) 
                    ReportResults.AddRange(settings.Filters.Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new ReportResult() 
                        { 
                            Name = Entry.Name, 
                            Path = Entry.Path, 
                            MatchingProperty = $"Filter [{x.FieldID}] {x.OperatorTypeAsString} {x.ValueFrom} {x.ValueTo}"
                        }));

            }
            else
            {
                foreach(FileSystemEntry file in IFSExplorer.GetFileSystemEntries(Entry))
                {
                    Recurse(file, IFSExplorer, Search);
                }
            }
        }
    }
}
