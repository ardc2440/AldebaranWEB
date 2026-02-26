using System;
using System.IO;

namespace Aldebaran.Infraestructure.Common.Utils
{
    public static class TempFileCleaner
    {
        /// <summary>
        /// Delete temporary inventory files older than specified hours.
        /// Looks for files starting with 'inventory_' in the temp directory or custom path.
        /// </summary>
        public static void CleanInventoryTempFiles(int olderThanHours = 48)
        {
            try
            {
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? ".", "temp");
                if (!Directory.Exists(dir)) return;

                var threshold = DateTime.UtcNow.AddHours(-Math.Max(1, olderThanHours));

                foreach (var file in Directory.EnumerateFiles(dir, "inventory_*"))
                {
                    try
                    {
                        var info = new FileInfo(file);
                        var lastWriteUtc = info.LastWriteTimeUtc;
                        if (lastWriteUtc < threshold)
                        {
                            // Attempt to open exclusively to ensure file is not in use/being written
                            try
                            {
                                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))
                                {
                                    // opened exclusively, safe to delete
                                }
                                info.Delete();
                            }
                            catch
                            {
                                // file is in use or cannot be opened exclusively - skip deletion
                            }
                        }
                    }
                    catch
                    {
                        // ignore single file errors
                    }
                }
            }
            catch
            {
                // ignore any errors during cleanup to avoid blocking startup
            }
        }
    }
}
