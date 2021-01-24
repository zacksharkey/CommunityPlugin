using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Models.Translation;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommunityPlugin.Objects.Helpers
{
    public class FileParser
    {
        private static IDictionary<string, string> __filesCached = (IDictionary<string, string>)new Dictionary<string, string>(8);
        private static IDictionary<string, IList<string[]>> __csvFilesCached = (IDictionary<string, IList<string[]>>)new Dictionary<string, IList<string[]>>(8);

        public static IList<string[]> GetCsvCacheFile(string fileName)
        {
            string key = fileName;
            if (!FileParser.__csvFilesCached.ContainsKey(key))
            {
                lock (FileParser.__csvFilesCached)
                {
                    if (!FileParser.__csvFilesCached.ContainsKey(key))
                    {
                        IList<string[]> strArrayList = (IList<string[]>)new List<string[]>();
                        using (Stream stream = FileParser.GetStream(fileName))
                        {
                            if (stream.CanSeek)
                                stream.Seek(0L, SeekOrigin.Begin);
                            using (StreamReader streamReader = new StreamReader(stream))
                            {
                                while (!streamReader.EndOfStream)
                                {
                                    string line = streamReader.ReadLine();
                                    if (!string.IsNullOrWhiteSpace(line))
                                        strArrayList.Add(CsvLine.Split(line).ToArray<string>());
                                }
                            }
                        }
                        FileParser.__csvFilesCached.Add(key, strArrayList);
                    }
                }
            }
            return FileParser.__csvFilesCached[key];
        }

        public static string GetCacheFile(string fileName)
        {
            if (!FileParser.__filesCached.ContainsKey(fileName))
            {
                lock (FileParser.__filesCached)
                {
                    if (!FileParser.__filesCached.ContainsKey(fileName))
                        FileParser.__filesCached.Add(fileName, FileParser.GetFile(fileName));
                }
            }
            return FileParser.__filesCached[fileName];
        }

        public static string GetFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (GlobalConfiguration.CurrentSession == null)
                throw new NotSupportedException("Please init global session in the first!");
            Session session = GlobalConfiguration.CurrentSession;
            Func<string, string> func1 = (Func<string, string>)(filename => new StreamReader(session.DataExchange.GetCustomDataObject(filename).OpenStream()).ReadToEnd());
            Func<string, string> func2 = (Func<string, string>)(filename => new StreamReader(new FileInfo(fileName).FullName).ReadToEnd());
            switch (GlobalConfiguration.Mode)
            {
                case RunMode.EncompassServer:
                    return func1(fileName);
                case RunMode.WebServer:
                case RunMode.Client:
                    return !File.Exists(fileName) ? func1(fileName) : func2(fileName);
                default:
                    throw new NotSupportedException(string.Format("Invalid support mode:{0}", (object)GlobalConfiguration.Mode));
            }
        }

        public static Stream GetStream(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (GlobalConfiguration.CurrentSession == null)
                throw new NotSupportedException("Please init global session in the first!");
            Session session = GlobalConfiguration.CurrentSession;
            Func<string, Stream> func1 = (Func<string, Stream>)(filename => session.DataExchange.GetCustomDataObject(filename).OpenStream());
            Func<string, Stream> func2 = (Func<string, Stream>)(filename => (Stream)File.OpenRead(new FileInfo(fileName).FullName));
            switch (GlobalConfiguration.Mode)
            {
                case RunMode.EncompassServer:
                    return func1(fileName);
                case RunMode.WebServer:
                case RunMode.Client:
                    return !File.Exists(fileName) ? func1(fileName) : func2(fileName);
                default:
                    throw new NotSupportedException(string.Format("Invalid support mode:{0}", (object)GlobalConfiguration.Mode));
            }
        }
    }
}
