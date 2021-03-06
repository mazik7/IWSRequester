﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using RestSharp;

namespace Requester
{
    public enum State
    {
        Ready = 0,
        Started = 1,
        Ended = 2,
        Faggot = 5

    }
    public class Test
    {
        private RestRequest _request;
        private string _description;
        private string _name;
        private RestClient _client;
        private HttpStatusCode _expectedCode;
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
        public HttpStatusCode ExpectedCode
        {
            get
            {
                return _expectedCode;
            }
            set
            {
                _expectedCode = value;
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
            _expectedCode = HttpStatusCode.OK;
            Status = State.Ready;
        }
        public Test(RestClient client, RestRequest request, string name, string description, string body)
        {
            _request = request;
            _name = name;
            _description = description;
            _client = client;
            _request.AddParameter("application/json", body, ParameterType.RequestBody);
            _expectedCode = HttpStatusCode.OK;
            Status = State.Ready;
        }
        public Test(RestClient client, RestRequest request, string name, string description, System.Net.HttpStatusCode expectedCode)
        {
            _request = request;
            _name = name;
            _description = description;
            _client = client;
            _expectedCode = expectedCode;
            Status = State.Ready;
        }
        public Test(RestClient client, RestRequest request, string name, string description, string body, System.Net.HttpStatusCode expectedCode)
        {
            _request = request;
            _name = name;
            _description = description;
            _client = client;
            _request.AddParameter("application/json", body, ParameterType.RequestBody);
            _expectedCode = expectedCode;
            Status = State.Ready;
        }
        public Result Start(string oauth_token)
        {
            if(Status == State.Faggot)
                return new Result(null, null, false, null, null, null);
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
                if(responce.StatusCode==_expectedCode)
                    return new Result(_name, responce.Request.Resource ,true, responce.StatusCode.ToString(), responce.Content, responce.StatusDescription);
                else
                    return new Result(_name, responce.Request.Resource, false, responce.StatusCode.ToString(), responce.Content, responce.StatusDescription);
            }
        }

    }
}
