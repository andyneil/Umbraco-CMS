﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Examine;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Mapping;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Models.Mapping
{
    internal class EntityModelMapper : MapperConfiguration
    {
        public override void ConfigureMappings(IConfiguration config, ApplicationContext applicationContext)
        {
            config.CreateMap<UmbracoEntity, EntityBasic>()
                  .ForMember(basic => basic.Icon, expression => expression.MapFrom(entity => entity.ContentTypeIcon));

            config.CreateMap<SearchResult, EntityBasic>()
                //default to document icon
                  .ForMember(x => x.Icon, expression => expression.UseValue("icon-document"))
                  .ForMember(x => x.Id, expression => expression.MapFrom(result => result.Id))
                  .AfterMap((result, basic) =>
                      {
                          basic.Name = result.Fields.ContainsKey("nodeName") ? result.Fields["nodeName"] : "[no name]";
                          if (result.Fields.ContainsKey("__NodeKey"))
                          {
                              Guid key;
                              if (Guid.TryParse(result.Fields["__NodeKey"], out key))
                              {
                                  basic.Key = key;
                              }
                          }
                          if (result.Fields.ContainsKey("ParentID"))
                          {
                              int parentId;
                              if (int.TryParse(result.Fields["ParentID"], out parentId))
                              {
                                  basic.ParentId = parentId;
                              }
                              else
                              {
                                  basic.ParentId = -1;
                              }
                          }
                          basic.Path = result.Fields.ContainsKey("__Path") ? result.Fields["__Path"] : "";
                      });

            config.CreateMap<ISearchResults, IEnumerable<EntityBasic>>()
                  .ConvertUsing(results => results.Select(Mapper.Map<EntityBasic>).ToList());
        }
    }
}