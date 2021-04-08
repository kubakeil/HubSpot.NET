﻿namespace HubSpot.NET.Api.Deal
{
    using System;
    using System.Linq;
    using System.Net;
    using Flurl;
    using HubSpot.NET.Api.Deal.Dto;
    using HubSpot.NET.Core;
    using HubSpot.NET.Core.Interfaces;
    using RestSharp;

    public class HubSpotDealApi : IHubSpotDealApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotDealApi(IHubSpotClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a deal entity
        /// </summary>
        /// <typeparam name="T">Implementation of DealHubSpotModel</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>The created entity (with ID set)</returns>
        public T Create<T>(T entity) where T : DealHubSpotModel, new()
        {
            var path = $"{entity.RouteBasePath}/deal";
            var data = _client.Execute<T>(path, entity, Method.POST);
            return data;
        }

        /// <summary>
        /// Gets a single deal by ID
        /// </summary>
        /// <param name="dealId">ID of the deal</param>
        /// <typeparam name="T">Implementation of DealHubSpotModel</typeparam>
        /// <returns>The deal entity or null if the deal does not exist</returns>
        public T GetById<T>(long dealId) where T : DealHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/deal/{dealId}";

            try
            {
                var data = _client.Execute<T>(path, Method.GET);
                return data;
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        /// <summary>
        /// Updates a given deal
        /// </summary>
        /// <typeparam name="T">Implementation of DealHubSpotModel</typeparam>
        /// <param name="entity">The deal entity</param>
        /// <returns>The updated deal entity</returns>
        public T Update<T>(T entity) where T : DealHubSpotModel, new()
        {
            if (entity.Id < 1)
            {
                throw new ArgumentException("Deal entity must have an id set!");
            }

            var path = $"{entity.RouteBasePath}/deal/{entity.Id}";

            var data = _client.Execute<T>(path, entity, method: Method.PUT);
            return data;
        }

        /// <summary>
        /// Gets a list of deals
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealListHubSpotModel<T> List<T>(bool includeAssociations, ListRequestOptions opts = null) where T : DealHubSpotModel, new()
        {
            if (opts == null)
            {
                opts = new ListRequestOptions(250);
            }

            var path = $"{new DealListHubSpotModel<T>().RouteBasePath}/deal/paged"
                .SetQueryParam("limit", opts.Limit);

            if (opts.Offset.HasValue)
            {
                path = path.SetQueryParam("offset", opts.Offset);
            }

            if (includeAssociations)
            {
                path = path.SetQueryParam("includeAssociations", "true");
            }

            if (opts.PropertiesToInclude.Any())
            {
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);
            }

            var data = _client.ExecuteList<DealListHubSpotModel<T>>(path);

            return data;
        }


        /// <summary>
        /// Gets a list of deals associated to a hubSpot Object
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="includeAssociations">Bool include associated Ids</param>
        /// <param name="hubId">Long Id of Hubspot object related to deals</param>
        /// <param name="objectName">String name of Hubspot object related to deals (contact\account)</param>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealListHubSpotModel<T> ListAssociated<T>(bool includeAssociations, long hubId, ListRequestOptions opts = null, string objectName = "contact") where T : DealHubSpotModel, new()
        {
            if (opts == null)
            {
                opts = new ListRequestOptions();
            }

            var path = $"{new DealListHubSpotModel<T>().RouteBasePath}/deal/associated/{objectName}/{hubId}/paged"
                .SetQueryParam("limit", opts.Limit);

            if (opts.Offset.HasValue)
            {
                path = path.SetQueryParam("offset", opts.Offset);
            }

            if (includeAssociations)
            {
                path = path.SetQueryParam("includeAssociations", "true");
            }

            if (opts.PropertiesToInclude.Any())
            {
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);
            }

            var data = _client.ExecuteList<DealListHubSpotModel<T>>(path, opts);

            return data;
        }

        /// <summary>
        /// Deletes a given deal (by ID)
        /// </summary>
        /// <param name="dealId">ID of the deal</param>
        public void Delete(long dealId)
        {
            var path = $"{new DealHubSpotModel().RouteBasePath}/deal/{dealId}";

            _client.Execute(path, method: Method.DELETE);
        }

        /// <summary>
        /// Gets a list of recently created deals
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealRecentListHubSpotModel<T> RecentlyCreated<T>(DealRecentRequestOptions opts = null) where T : DealHubSpotModel, new()
        {

            if (opts == null)
            {
                opts = new DealRecentRequestOptions();
            }

            var path = $"{new DealRecentListHubSpotModel<T>().RouteBasePath}/deal/recent/created"
                .SetQueryParam("count", opts.Limit);

            if (opts.Offset.HasValue)
            {
                path = path.SetQueryParam("offset", opts.Offset);
            }

            if (opts.IncludePropertyVersion)
            {
                path = path.SetQueryParam("includePropertyVersions", "true");
            }

            if (!string.IsNullOrEmpty(opts.Since))
            {
                path = path.SetQueryParam("since", opts.Since);
            }

            var data = _client.ExecuteList<DealRecentListHubSpotModel<T>>(path);

            return data;
        }

        /// <summary>
        /// Gets a list of recently modified deals
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealRecentListHubSpotModel<T> RecentlyUpdated<T>(DealRecentRequestOptions opts = null) where T : DealHubSpotModel, new()
        {
            if (opts == null)
            {
                opts = new DealRecentRequestOptions();
            }

            var path = $"{new DealRecentListHubSpotModel<T>().RouteBasePath}/deal/recent/modified"
                .SetQueryParam("count", opts.Limit);

            if (opts.Offset.HasValue)
            {
                path = path.SetQueryParam("offset", opts.Offset);
            }

            if (opts.IncludePropertyVersion)
            {
                path = path.SetQueryParam("includePropertyVersions", "true");
            }

            if (!string.IsNullOrEmpty(opts.Since))
            {
                path = path.SetQueryParam("since", opts.Since);
            }

            var data = _client.ExecuteList<DealRecentListHubSpotModel<T>>(path);

            return data;
        }

        /// <summary>
        /// Gets a list of deals based on a search criteria
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="DealHubSpotModel"/></typeparam>
        /// <param name="opts">Options (limit, offset) and search criteria relating to request</param>
        /// <returns>List of deals</returns>
        public SearchHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : DealHubSpotModel, new()
        {
            if (opts == null)
            {
                opts = new SearchRequestOptions();
            }

            var path = "/crm/v3/objects/deals/search";

            var data = _client.ExecuteList<SearchHubSpotModel<T>>(path, opts, Method.POST);

            return data;
        }

        /// <summary>
        /// Associate a Company to a deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="DealHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to associate the company with</param>
        /// <param name="companyId">The Id of the company to associate the deal with</param>
        public T AssociateToCompany<T>(T entity, long companyId) where T : DealHubSpotModel, new()
        {
            var path = "/crm-associations/v1/associations";

            _client.Execute(path, new
            {
                fromObjectId = entity.Id,
                toObjectId = companyId,
                category = "HUBSPOT_DEFINED",
                definitionId = 5 // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            }, method: Method.PUT);
            entity.Associations.AssociatedCompany = new[] { companyId };
            return entity;
        }

        /// <summary>
        /// Gets a list of associations for a given deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="DealHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to get associations for</param>
        public T GetAssociations<T>(T entity) where T : DealHubSpotModel, new()
        {
            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var companyPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/5";

            var companyAssociations = _client.ExecuteList<IdListHubSpotModel>(companyPath, convertToPropertiesSchema: false);
            if (companyAssociations.Results.Any())
                entity.Associations.AssociatedCompany = companyAssociations.Results.ToArray();

            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var contactPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/3";

            var contactAssociations = _client.ExecuteList<IdListHubSpotModel>(contactPath, convertToPropertiesSchema: false);
            if (contactAssociations.Results.Any())
                entity.Associations.AssociatedContacts = contactAssociations.Results.ToArray();

            return entity;
        }
    }
}
