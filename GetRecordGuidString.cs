using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace GetRecordGuidString
{
    public class GetRecordGuidString : CodeActivity
    {
        //For Primary Record
        [Input("Primary Record?")]
        public InArgument<bool> isBase { get; set; }

        //For Entity Reference
        [Input("Column Name if Entity Reference")]
        public InArgument<string> columnname { get; set; }

        // Result - the GUID 
        [Output("res")]
        public OutArgument<string> res { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            try { 
            //Build the connection
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (isBase.Get<bool>(executionContext)) //Just return the entitiesID
                {
                    res.Set(executionContext, context.PrimaryEntityId.ToString());
                } 

            else  //It's an EntityReference
                { 
                    string entityref = columnname.Get<string>(executionContext);          
                    Entity entity = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(entityref));
                    string result = "";
                    if (entity.Attributes.Contains(entityref))
                    {
                        if (entity.Attributes[entityref] != null)
                        {
                            result = (entity.Attributes[entityref] as EntityReference).Id.ToString();
                        }
                    }                  
                    res.Set(executionContext, result);
                 }
            }
            catch
            {
                res.Set(executionContext, "error");
            }

        } 
    } 
}

