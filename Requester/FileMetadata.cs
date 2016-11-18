using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requester
{
    public class FileMetadata
    {
        public string OwnerType { get; set; }

        public string OwnerKey { get; set; }

        public Guid Guid { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }

        public int StorageType { get; set; }

        public string CreatorUserKey { get; set; }

        public long Size { get; set; }

        public string MimeType { get; set; }

        public int Order { get; set; }

        public DateTime Date { get; set; }
    }
}
