using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RestSharp;

namespace Requester
{
    public class CurrentUser
    {
        private string _url;
        private string _token;
        private string _userName;
        public CurrentUser(Config config)
        {
            _url = config.BaseURL;
            _token = config.Token;
            RestClient client = new RestClient(_url);
            var request = new RestRequest("users/current");
            request.AddHeader("oauth_token", _token);
            request.RequestFormat = DataFormat.Json;
            IRestResponse responce = client.Execute(request);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                string a = responce.Content;
                int start = a.IndexOf("\"UserName\"");
                a = a.Remove(0, start);
                a = a.Replace("}", "");
                _userName = a;
            }
            else
            {
                _userName = responce.StatusDescription;
            }
        }
        public string UserName
        {
            get
            {
                return _userName;
            } 
        }
    }
}
