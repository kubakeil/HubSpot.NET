using HubSpot.NET.Api.Properties.Dto;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.Properties
{
    public class HubSpotDealsPropertiesApi : IHubSpotDealPropertiesApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotDealsPropertiesApi(IHubSpotClient client)
        {
            _client = client;
        }

        public PropertiesListHubSpotModel<DealPropertyHubSpotModel> GetAll()
        {
            var path = $"{new PropertiesListHubSpotModel<DealPropertyHubSpotModel>().RouteBasePath}";

            return _client.ExecuteList<PropertiesListHubSpotModel<DealPropertyHubSpotModel>>(path, convertToPropertiesSchema: false);
        }

        public DealPropertyHubSpotModel GetProperty(string name)
        {
            var path = $"/properties/v1/deals/properties/named/{name}";

            return _client.ExecuteList<DealPropertyHubSpotModel>(path, convertToPropertiesSchema: false);
        }

        public DealPropertyHubSpotModel Create(DealPropertyHubSpotModel property)
        {
            var path = $"{new PropertiesListHubSpotModel<DealPropertyHubSpotModel>().RouteBasePath}";

            return _client.Execute<DealPropertyHubSpotModel>(path, property, Method.POST, convertToPropertiesSchema: false);
        }

    }
}
