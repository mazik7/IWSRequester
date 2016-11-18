using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requester
{
    public class FileViewModelList
    {
        public FileViewModelList()
        {
            Files = new List<FileViewModel>();
        }
        public List<FileViewModel> Files { get; set; }
    }
    public class FileViewModel
    {
        public string Key { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }

        public string PolaroidFileUrl { get; set; }

        public string SmallFileUrl { get; set; }

        public string TinyFileUrl { get; set; }

        public string OriginalFileUrl { get; set; }

        public string DeleteUrl { get; set; }

        public long Size { get; set; }

        public string MimeType { get; set; }

        public string CreatorUserKey { get; set; }

        public object CreatorUser { get; set; }
    }
}
