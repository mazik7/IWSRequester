using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Requester
{
    public class CurrentUser
    {
        private string _url;
        private string _token;
        public CurrentUser(Config config)
        {
            _url = config.BaseURL;
            _token = config.Token;
        }
        public string UserName
        {
            get
            {
                RestClient client = new RestClient(_url);
                var request = new RestRequest("users/current");
                request.AddHeader("oauth_token", _token);
                request.RequestFormat = RestSharp.DataFormat.Json;
                IRestResponse responce = client.Execute(request);
                //lblEventStatus.Content;
                CurrentUser user = SimpleJson.DeserializeObject<CurrentUser>(responce.Content);
                return user.UserName;
            }
        }
    }
}
