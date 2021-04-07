using HubSpot.NET.Api.Properties.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotDealPropertiesApi
    {
        PropertiesListHubSpotModel<DealPropertyHubSpotModel> GetAll();
        DealPropertyHubSpotModel Create(DealPropertyHubSpotModel property);
    }
}
