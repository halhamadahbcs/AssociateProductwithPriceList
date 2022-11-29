using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace AssociateProductwithPriceList
{
    public class AssociateProductwithPriceList : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target"))
            {
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                if (context.MessageName == "Create")
                {
                    Entity PriceListItem = context.InputParameters["Target"] as Entity;

                    if (PriceListItem.Contains("productid") && PriceListItem.Contains("pricelevelid"))
                    {
                        EntityReferenceCollection relatedPriceListEntities = new EntityReferenceCollection();
                        relatedPriceListEntities.Add((EntityReference)PriceListItem["pricelevelid"]);

                        Relationship relationship = new Relationship("bcs_product_pricelevel");

                        service.Associate(((EntityReference)PriceListItem["productid"]).LogicalName, ((EntityReference)PriceListItem["productid"]).Id, relationship, relatedPriceListEntities);

                    }
                }
                else if(context.MessageName == "Delete")
                {
                    EntityReference PriceListItemRef = context.InputParameters["Target"] as EntityReference;

                    Entity PriceListItem = service.Retrieve(PriceListItemRef.LogicalName, PriceListItemRef.Id, new ColumnSet(new string[] {"pricelevelid", "productid"}));
                    if (PriceListItem.Contains("productid") && PriceListItem.Contains("pricelevelid"))
                    {
                        EntityReferenceCollection relatedPriceListEntities = new EntityReferenceCollection();
                        relatedPriceListEntities.Add((EntityReference)PriceListItem["pricelevelid"]);

                        Relationship relationship = new Relationship("bcs_product_pricelevel");

                        service.Disassociate(((EntityReference)PriceListItem["productid"]).LogicalName, ((EntityReference)PriceListItem["productid"]).Id, relationship, relatedPriceListEntities);
                    }
                }
            }
        }
    }
}
