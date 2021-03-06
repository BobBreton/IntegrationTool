﻿using IntegrationTool.DataMappingControl;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.CrmWrapper
{
    public class JoinResolver
    {
        private IOrganizationService service;
        private EntityMetadata relatedEntityMetadata;
        private List<DataMapping> relationMappings;
        private Dictionary<string, AttributeMetadata> attributeMetadataDictionary = new Dictionary<string, AttributeMetadata>();

        public JoinResolver(IOrganizationService service, EntityMetadata relatedEntityMetadata, List<DataMapping> relationMappings)
        {
            this.service = service;
            this.relatedEntityMetadata = relatedEntityMetadata;
            this.relationMappings = relationMappings;

            foreach (DataMapping relMapping in relationMappings)
            {
                string str = relMapping.Target;
                AttributeMetadata attributeMetadata = relatedEntityMetadata.Attributes.Where(t => t.LogicalName == relMapping.Target).First();
                attributeMetadataDictionary.Add(attributeMetadata.LogicalName, attributeMetadata);
            }
        }

        public Dictionary<string, Guid[]> BuildMassResolverIndex()
        {
            ColumnSet columnSet = new ColumnSet(new string[] { relatedEntityMetadata.PrimaryIdAttribute });
            List<ConditionExpression> conditions = new List<ConditionExpression>();

            foreach (var relMapping in relationMappings)
            {
                conditions.Add(new ConditionExpression(relMapping.Target, ConditionOperator.NotNull));

                if (!columnSet.Columns.Contains(relMapping.Target))
                {
                    columnSet.Columns.Add(relMapping.Target);
                }
            }

            DataCollection<Entity> existingEntities = Crm2013Wrapper.Crm2013Wrapper.RetrieveMultiple(service, relatedEntityMetadata.LogicalName, columnSet, conditions);

            Dictionary<string, Guid[]> keyDictionary = BuildExistingCheckDictionary(existingEntities, attributeMetadataDictionary);

            return keyDictionary;
        }

        public Dictionary<string, Guid[]> BuildExistingCheckDictionary(DataCollection<Entity> entities, Dictionary<string, AttributeMetadata> attributeMetadataDictionary)
        {
            Dictionary<string, Guid[]> keyDictionary = new Dictionary<string, Guid[]>();
            foreach (Entity entity in entities)
            {

                string strKey = BuildExistingCheckKey(entity, attributeMetadataDictionary);
                if (keyDictionary.ContainsKey(strKey) == false)
                {
                    keyDictionary.Add(strKey, new Guid[] { entity.Id });
                }
                else
                {
                    List<Guid> Ids = keyDictionary[strKey].ToList();
                    Ids.Add(entity.Id);
                    keyDictionary[strKey] = Ids.ToArray();
                }
            }

            return keyDictionary;
        }

        public static string BuildExistingCheckKey(Entity entity, Dictionary<string, AttributeMetadata> attributeMetadataDictionary)
        {
            string strKey = "";
            foreach (var primaryKey in attributeMetadataDictionary)
            {
                if (entity.Contains(primaryKey.Key) == false)
                    throw new Exception("Could not find all attributes for primary key");
                strKey += Crm2013Wrapper.Crm2013Wrapper.GetAttributeIdentifierStringValue(entity, primaryKey.Value) + "##";
            }

            return strKey;
        }

        public static string BuildExistingCheckKey(object[] dataObject, List<DataMappingControl.DataMapping> mapping, DataMetadata dataMetadata)
        {
            string[] keyvalues = new string[mapping.Count];
            for (int iMapping = 0; iMapping < mapping.Count; iMapping++)
            {
                string source = mapping[iMapping].Source;
                keyvalues[iMapping] = dataObject[dataMetadata[source].ColumnIndex].ToString();
            }

            string key = string.Empty;
            for (int i = 0; i < keyvalues.Length; i++)
            {
                key += keyvalues[i] + "##";
            }

            return key;
        }

        public Dictionary<string, Guid[]> OneByOneResolver(Entity[] sourceEntities)
        {
            Dictionary<string, Guid[]> keyDictionary = new Dictionary<string, Guid[]>();


            return keyDictionary;
        }

        
    }
}
