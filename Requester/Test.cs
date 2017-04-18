using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Requester
{
    public enum State
    {
        Ready = 0,
        Started = 1,
        Ended = 2,
        Pidor = 5

    }
    public class Test
    {
        private RestRequest _request;
        private string _description;
        private string _name;
        private RestClient _client;
        public State Status { get; set; }
        public RestRequest Request
        {
            get
            {
                return _request;
            }
            set
            {
                _request = value;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        public Test(RestClient client, RestRequest request, string name, string description)
        {
            _request = request;
            _name = name;
            _description = description;
            _client = client;
            Status = State.Ready;
        }
        public Test(RestClient client, RestRequest request, string name, string description, string body)
        {
            _request = request;
            _name = name;
            _description = description;
            _client = client;
            _request.AddParameter("application/json", body, ParameterType.RequestBody);
            Status = State.Ready;
        }
        public Result Start(string oauth_token)
        {
            if(Status == State.Pidor)
                return new Result(null, false, null, null, null);
            else
            {
                foreach (Parameter parameter in _request.Parameters)
                {
                    if (parameter.Name.Contains("oauth_token"))
                    {
                        _request.Parameters.Remove(parameter);
                        break;
                    }
                }
                _request.AddHeader("oauth_token", oauth_token);
                IRestResponse responce;
                responce = _client.Execute(_request);
                if(responce.StatusCode==System.Net.HttpStatusCode.OK)
                    return new Result(responce.Request.Resource ,true, responce.StatusCode.ToString(), responce.Content.ToString(), responce.StatusDescription.ToString());
                else
                    return new Result(responce.Request.Resource, false, responce.StatusCode.ToString(), responce.Content.ToString(), responce.StatusDescription.ToString());
            }
        }
        public Result Start(string oauth_token, System.Net.HttpStatusCode expectedCode)
        {
            if (Status == State.Pidor)
                return new Result(null, false, null, null, null);
            else
            {
                foreach (Parameter parameter in _request.Parameters)
                {
                    if (parameter.Name.Contains("oauth_token"))
                    {
                        _request.Parameters.Remove(parameter);
                        break;
                    }
                }
                _request.AddHeader("oauth_token", oauth_token);
                IRestResponse responce;
                responce = _client.Execute(_request);
                if (responce.StatusCode == expectedCode)
                    return new Result(responce.Request.Resource, true, responce.StatusCode.ToString(), responce.Content.ToString(), responce.StatusDescription.ToString());
                else
                    return new Result(responce.Request.Resource, false, responce.StatusCode.ToString(), responce.Content.ToString(), responce.StatusDescription.ToString());
            }
        }
    }
}
