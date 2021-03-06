﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Linq;
using NJsonSchema;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Core.GenerateJsonSchema
{
    public static class JsonSchemaExtensions
    {
        public static JsonSchema4 BuildJsonSchema(this Schema schema, PartitionResolver partitionResolver, Func<string, JsonSchema4, JsonSchema4> schemaResolver)
        {
            Guard.NotNull(schemaResolver, nameof(schemaResolver));
            Guard.NotNull(partitionResolver, nameof(partitionResolver));

            var schemaName = schema.Name.ToPascalCase();

            var jsonTypeVisitor = new JsonTypeVisitor(schemaResolver);
            var jsonSchema = new JsonSchema4 { Type = JsonObjectType.Object };

            foreach (var field in schema.Fields.Where(x => !x.IsHidden))
            {
                var partitionProperty = CreateProperty(field);
                var partitionObject = new JsonSchema4 { Type = JsonObjectType.Object, AllowAdditionalProperties = false };
                var partition = partitionResolver(field.Partitioning);

                foreach (var partitionItem in partition)
                {
                    var partitionItemProperty = field.Accept(jsonTypeVisitor);

                    partitionItemProperty.Description = partitionItem.Name;
                    partitionObject.Properties.Add(partitionItem.Key, partitionItemProperty);
                }

                partitionProperty.Reference = schemaResolver($"{schemaName}{field.Name.ToPascalCase()}Property", partitionObject);

                jsonSchema.Properties.Add(field.Name, partitionProperty);
            }

            return jsonSchema;
        }

        public static JsonProperty CreateProperty(Field field)
        {
            var jsonProperty = new JsonProperty { IsRequired = field.RawProperties.IsRequired, Type = JsonObjectType.Object };

            if (!string.IsNullOrWhiteSpace(field.RawProperties.Hints))
            {
                jsonProperty.Description = field.RawProperties.Hints;
            }
            else
            {
                jsonProperty.Description = field.Name;
            }

            if (!string.IsNullOrWhiteSpace(field.RawProperties.Hints))
            {
                jsonProperty.Description += $" ({field.RawProperties.Hints}).";
            }

            return jsonProperty;
        }
    }
}
