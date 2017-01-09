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
        public string UserName
        {
            get
            {
                RestClient client = new RestClient(_config.BaseURL);
                var request = new RestRequest("users/current");
                request.AddHeader("oauth_token", _config.Token);
                request.RequestFormat = RestSharp.DataFormat.Json;
                IRestResponse responce = client.Execute(request);
                //lblEventStatus.Content;
                CurrentUser user = SimpleJson.DeserializeObject<CurrentUser>(responce.Content);
                return user.UserName;
            }
        }
    }
}
