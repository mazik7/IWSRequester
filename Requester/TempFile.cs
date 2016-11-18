using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Requester
{
    public class TempFile
    {
        public TempFile(string path)
        {
            System.IO.FileInfo fInfo = new System.IO.FileInfo(path);
            Name = fInfo.Name;
            Size = fInfo.Length;
            LocalPath = path;
            MimeType = MimeMapping.GetMimeMapping(path);

        }
        public string LocalPath { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string CloudUrl { get; set; }
    }
}
