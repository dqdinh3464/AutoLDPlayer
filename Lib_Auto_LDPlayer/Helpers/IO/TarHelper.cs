using SharpCompress.Common;
using SharpCompress.Readers;
using System.IO;

namespace Auto_LDPlayer.Helpers.IO
{
    public class TarHelper
    {
        public static void ExtractTGZ(string gzArchiveName, string destFolder)
        {
            try
            {
                using (Stream stream = File.OpenRead(gzArchiveName))
                {
                    IReader reader = ReaderFactory.Open(stream, null);
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            ExtractionOptions extractionOptions = new ExtractionOptions
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            };
                            try
                            {
                                IReaderExtensions.WriteEntryToDirectory(reader, destFolder, extractionOptions);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }
    }
}