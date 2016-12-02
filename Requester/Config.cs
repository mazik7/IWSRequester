using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requester
{
    public class Config
    {
        public string Request { get; set; }
        public string Responce { get; set; }
        public string Body { get; set; }
        public string BaseURL
        {
            get
            {
                if (IsBiz)
                    return UrlBiz;
                else
                    return UrlCom;
            }
        }
        public string Token
        {
            get
            {
                if (IsBiz)
                    return TokenBiz;
                else
                    return TokenCom;
            }
        }
        public string Method { get; set; }
        public string UrlBiz { get; set; }
        public string UrlCom { get; set; }
        public string TokenBiz { get; set; }
        public string TokenCom { get; set; }
        public bool IsBiz { get; set; }
    }
}
