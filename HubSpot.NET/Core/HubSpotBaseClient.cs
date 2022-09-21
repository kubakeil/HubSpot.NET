using System;
using System.Collections.Generic;
using Flurl;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.Requests;
using RestSharp;

namespace HubSpot.NET.Core
{
    public class HubSpotBaseClient : IHubSpotClient
    {
        private readonly RequestSerializer _serializer = new RequestSerializer(new RequestDataConverter());
        private readonly RestClient _client;

        private string _baseUrl => "https://api.hubapi.com";
        private readonly bool _useOAuth;
        private readonly string _apiKey;

        public HubSpotBaseClient(string apiKey, bool useOAuth)
        {
            _useOAuth = useOAuth;
            _apiKey = apiKey;
            _client = new RestClient(_baseUrl);
        }

        public T Execute<T>(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            string json = entity == null ? null : _serializer.SerializeEntity(entity, convertToPropertiesSchema);

            var data = SendRequest(absoluteUriPath, method, json, responseData => (T)_serializer.DeserializeEntity<T>(responseData, convertToPropertiesSchema));

            return data;
        }

        public T Execute<T>(string absoluteUriPath, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            var data = SendRequest(absoluteUriPath, method, null, responseData => (T)_serializer.DeserializeEntity<T>(responseData, convertToPropertiesSchema));

            return data;
        }

        public void Execute(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true)
        {
            string json = entity == null ? null : _serializer.SerializeEntity(entity, convertToPropertiesSchema);
            
            SendRequest(absoluteUriPath, method, json);
        }

        public void ExecuteBatch(string absoluteUriPath, List<object> entities, Method method = Method.GET,
            bool convertToPropertiesSchema = true)
        {
            string json = entities == null ? null : _serializer.SerializeEntity(entities, convertToPropertiesSchema);

            SendRequest(absoluteUriPath, method, json);
        }

        public T ExecuteMultipart<T>(string absoluteUriPath, byte[] data, string filename, Dictionary<string,string> parameters, Method method = Method.POST) where T : new()
        {
            var fullUrl = _useOAuth
                ? new Url($"{_baseUrl}{absoluteUriPath}")
                : $"{_baseUrl}{absoluteUriPath}".SetQueryParam("hapikey", _apiKey);

            var request = new RestRequest(fullUrl, method);

            if (_useOAuth)
                request.AddHeader("Authorization", $"Bearer {_apiKey}");

            request.AddFile(filename, data, filename);

            foreach (var kvp in parameters)
            {
                request.AddParameter(kvp.Key, kvp.Value);
            }

            var response = _client.Execute<T>(request);

            var responseData = response.Data;

            if (!response.IsSuccessful())
            {
                throw new HubSpotException("Error from HubSpot", new HubSpotError(response.StatusCode, response.StatusDescription));
            }

            return responseData;
        }

        public T ExecuteList<T>(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            string json = entity == null ? null : _serializer.SerializeEntity(entity);

            var data = SendRequest(
                absoluteUriPath,
                method,
                json,
                responseData => (T)_serializer.DeserializeListEntity<T>(responseData, convertToPropertiesSchema));
            return data;
        }

        private T SendRequest<T>(string path, Method method, string json, Func<string, T> deserializeFunc) where T : IHubSpotModel, new()
        {
            var responseData = SendRequest(path, method, json);

            if (string.IsNullOrWhiteSpace(responseData))
            {
                return default;
            }

            return deserializeFunc(responseData);
        }

        private string SendRequest(string path, Method method, string json)
        {
            var url = _useOAuth
                ? new Url(path)
                : $"{path}".SetQueryParam("hapikey", _apiKey);

            var request = new RestRequest(url, method);

            if (_useOAuth)
                request.AddHeader("Authorization", $"Bearer {_apiKey}");

            if (!string.IsNullOrWhiteSpace(json))
            {
                request.AddParameter("application/json", json, ParameterType.RequestBody);
            }

            var response = _client.Execute(request);

            var responseData = response.Content;

            if (!response.IsSuccessful())
            {
                throw new HubSpotException("Error from HubSpot", new HubSpotError(response.StatusCode, response.StatusDescription), responseData);
            }

            return responseData;
        }
        
    }
}