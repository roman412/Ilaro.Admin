﻿using Ilaro.Admin.Services.Interfaces;
using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.EntitiesFilters;
using System.Data.SqlClient;
using System.Data.Entity;
using Ilaro.Admin.Commons;
using System.Data;
using System.Web.Mvc;
using Ilaro.Admin.Commons.FileUpload;
using Ilaro.Admin.Commons.Notificator;
using System.Data.Common;
using Massive;
using System.Dynamic;
using System.Collections.Specialized;

namespace Ilaro.Admin.Services
{
	public class EntityService : BaseService, IEntityService
	{
		public EntityService(Notificator notificator)
			: base(notificator)
		{
		}

		public PagedRecordsViewModel GetRecords(EntityViewModel entity, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection)
		{
			var skip = (page - 1) * take;

			var search = new EntitySearch { Query = searchQuery, Properties = entity.SearchProperties };
			order = order.IsNullOrEmpty() ? entity.Key.Name : order;
			orderDirection = orderDirection.IsNullOrEmpty() ? "ASC" : orderDirection.ToUpper();
			var orderBy = order + " " + orderDirection;
			var columns = string.Join(",", entity.DisplayColumns.Select(x => x.Name));
			var where = ConvertFiltersToSQL(filters, search);

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.Name);

			var result = table.Paged(columns: columns, where: where, orderBy: orderBy, currentPage: page, pageSize: take);

			var data = new List<DataRowViewModel>();
			foreach (var item in result.Items)
			{
				var dict = (IDictionary<String, Object>)item;
				var row = new DataRowViewModel();
				row.KeyValue = dict[entity.Key.Name].ToStringSafe();
				row.LinkKeyValue = dict[entity.LinkKey.Name].ToStringSafe();
				foreach (var property in entity.DisplayColumns)
				{
					row.Values.Add(new CellValueViewModel
					{
						Value = dict[property.Name].ToStringSafe(),
						Property = property
					});
				}
				data.Add(row);
			}

			return new PagedRecordsViewModel
			{
				TotalItems = result.TotalRecords,
				TotalPages = result.TotalPages,
				Records = data
			};
		}

		private string ConvertFiltersToSQL(IList<IEntityFilter> filters, EntitySearch search, string alias = "")
		{
			var activeFilters = filters.Where(x => !x.Value.IsNullOrEmpty());
			if (!activeFilters.Any() && !search.IsActive)
			{
				return null;
			}

			if (!alias.IsNullOrEmpty())
			{
				alias += ".";
			}

			var conditions = String.Empty;
			foreach (var filter in activeFilters)
			{
				var condition = filter.GetSQLCondition(alias);

				if (!condition.IsNullOrEmpty())
				{
					conditions += string.Format(" {0} AND", filter.GetSQLCondition(alias));
				}
			}

			if (search.IsActive)
			{
				var searchCondition = String.Empty;
				foreach (var property in search.Properties)
				{
					var query = search.Query.TrimStart('>', '<');
					var temp = 0.0m;
					if (property.PropertyType.In(typeof(string)))
					{
						searchCondition += string.Format(" {0}[{1}] LIKE '%{2}%' OR", alias, property.Name, search.Query);
					}
					else if (decimal.TryParse(query, out temp))
					{
						var sign = "=";
						if (search.Query.StartsWith(">"))
						{
							sign = ">=";
						}
						else if (search.Query.StartsWith("<"))
						{
							sign = "<=";
						}

						searchCondition += string.Format(" {0}[{1}] {3} {2} OR", alias, property.Name, query.Replace(",", "."), sign);
					}
				}

				if (!searchCondition.IsNullOrEmpty())
				{
					conditions += " (" + searchCondition.TrimStart(' ').TrimEnd("OR".ToCharArray()) + ")";
				}
			}

			if (conditions.IsNullOrEmpty())
			{
				return null;
			}

			return " WHERE" + conditions.TrimEnd("AND".ToCharArray());
		}

		public IList<ColumnViewModel> PrepareColumns(EntityViewModel entity, string order, string orderDirection)
		{
			if (orderDirection == "asc")
			{
				orderDirection = "up";
			}
			else
			{
				orderDirection = "down";
			}

			order = order.ToLower();

			return entity.DisplayColumns.Select(x => new ColumnViewModel
			{
				Name = x.Name,
				DisplayName = x.DisplayName,
				Description = x.Description,
				SortDirection = x.Name.ToLower() == order ? orderDirection : String.Empty
			}).ToList();
		}

		public object Create(EntityViewModel entity)
		{
			var existingItem = GetEntity(entity, entity.Key.Value);
			if (existingItem != null)
			{
				Error("Already exist");
				return null;
			}

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.Name);

			var expando = new ExpandoObject();
			var filler = expando as IDictionary<String, object>;
			foreach (var property in entity.Properties.Where(x => !x.IsKey && !x.IsForeignKey))
			{
				filler[property.Name] = property.Value;
			}
			var item = table.Insert(expando);

			ClearProperties(entity);

			return item;
		}

		public object Edit(EntityViewModel entity)
		{
			if (entity.Key.Value == null)
			{
				Error("Key is null");
				return null;
			}

			var existingItem = GetEntity(entity, entity.Key.Value.ToString());
			if (existingItem == null)
			{
				Error("Not exist");
				return null;
			}

			FillEntity(existingItem, entity);

			//context.SaveChanges();

			ClearProperties(entity);

			return existingItem;
		}

		public void ClearProperties(EntityViewModel entity)
		{
			foreach (var property in entity.Properties)
			{
				property.Value = null;
			}
		}

		public bool ValidateEntity(EntityViewModel entity, ModelStateDictionary ModelState)
		{
			var request = HttpContext.Current.Request;
			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.File))
			{
				var file = request.Files["field." + property.Name];
				var result = FileUpload.Validate(file, property.ImageOptions.MaxFileSize, property.ImageOptions.AllowedFileExtensions, !property.IsRequired);

				if (result != FileUploadValidationResult.Valid)
				{
					return false;
				}
			}
			return true;
		}

		private void FillEntity(object item, EntityViewModel entity)
		{
			var request = HttpContext.Current.Request;
			foreach (var property in entity.CreateProperties())
			{
				if (property.DataType == DataType.File)
				{
					var file = request.Files["field." + property.Name];
					var fileName = String.Empty;
					if (property.ImageOptions.NameCreation == NameCreation.UserInput)
					{
						fileName = "test";
					}
					FileUpload.SaveImage(file, property.ImageOptions.MaxFileSize, property.ImageOptions.AllowedFileExtensions, out fileName, property.ImageOptions.NameCreation, property.ImageOptions.Settings);

					property.Value = fileName;
				}

				var propertyInfo = entity.Type.GetProperty(property.Name);
				propertyInfo.SetValue(item, property.Value, null);
			}
		}

		public void FillEntity(EntityViewModel entity, FormCollection collection)
		{
			foreach (var property in entity.Properties)
			{
				var value = collection.GetValue("property." + property.Name);
				if (value != null)
				{
					property.Value = value.ConvertTo(property.PropertyType);
				}
			}
		}

		public void FillEntity(EntityViewModel entity, string key)
		{
			var item = GetEntity(entity, key);
			if (item == null)
			{
				Error("Not exist");
				return;
			}

			//foreach (var property in entity.CreateProperties(false))
			//{
			//	var propertyInfo = entity.Type.GetProperty(property.Name);
			//	property.Value = propertyInfo.GetValue(item, null);
			//}

			//entity.Key.Value = key;
		}

		private object GetEntity(EntityViewModel entity, string key)
		{
			var keyObject = GetKeyObject(entity, key);

			return GetEntity(entity, keyObject);
		}

		private object GetEntity(EntityViewModel entity, object key)
		{
			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.Name);

			var result = table.Single(key);

			return result;
		}

		private object GetKeyObject(EntityViewModel entity, string key)
		{
			var keyType = entity.Key.PropertyType;
			if (keyType.In(typeof(int), typeof(short), typeof(long)))
			{
				return long.Parse(key);
			}
			else if (keyType.In(typeof(Guid)))
			{
				return Guid.Parse(key);
			}
			else
			{
				return key;
			}
		}

		public bool Delete(EntityViewModel entity, string key)
		{
			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.Name);

			var keyObject = GetKeyObject(entity, key);

			var result = table.Delete(keyObject);

			if (result < 1)
			{
				Error("Not exist");
				return false;
			}

			return true;
		}

		public IList<IEntityFilter> PrepareFilters(EntityViewModel entity, HttpRequestBase request)
		{
			var filters = new List<IEntityFilter>();

			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.Bool))
			{
				var value = request[property.Name];

				var filter = new BoolEntityFilter();
				filter.Initialize(property, value);
				filters.Add(filter);
			}

			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.Enum))
			{
				var value = request[property.Name];

				var filter = new EnumEntityFilter();
				filter.Initialize(property, value);
				filters.Add(filter);
			}

			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.DateTime))
			{
				var value = request[property.Name];

				var filter = new DateTimeEntityFilter();
				filter.Initialize(property, value);
				filters.Add(filter);
			}

			return filters;
		}

		public object GetKeyValue(EntityViewModel entity, object savedItem)
		{
			// it should be always a ExpandoObject, but just in caase
			if (savedItem is ExpandoObject)
			{
				return ((dynamic)savedItem).ID;
			}

			var propertyInfo = entity.Type.GetProperty(entity.Key.Name);
			return propertyInfo.GetValue(savedItem, null);
		}

		public IList<GroupPropertiesViewModel> PrepareGroups(EntityViewModel entity, bool getKey = true)
		{
			var groupsDict = entity.CreateProperties(getKey).GroupBy(x => x.GroupName).ToDictionary(x => x.Key);

			var groups = new List<GroupPropertiesViewModel>();
			if (entity.Groups.IsNullOrEmpty())
			{
				foreach (var group in groupsDict)
				{
					groups.Add(new GroupPropertiesViewModel
					{
						GroupName = group.Key,
						Properties = group.Value.ToList()
					});
				}
			}
			else
			{
				foreach (var groupName in entity.Groups)
				{
					var trimedGroupName = groupName.TrimEnd('*');
					if (groupsDict.ContainsKey(trimedGroupName ?? "Others"))
					{
						var group = groupsDict[trimedGroupName];

						groups.Add(new GroupPropertiesViewModel
						{
							GroupName = group.Key,
							Properties = group.ToList(),
							IsCollapsed = groupName.EndsWith("*")
						});
					}
				}
			}

			return groups;
		}
	}
}