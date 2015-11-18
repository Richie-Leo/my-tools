//===============================================================================
// Richie (http://www.cnblogs.com/riccc)
// December 06, 2007
//===============================================================================

using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Magic.Framework.ORM.RefelectOptimizer;

namespace Magic.Framework.ORM.Mapping
{
	public class EntityMapping
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(EntityMapping));

		private string _tableName;
		private Type _entityType;
		private IList<PropertyMapping> _columns;
		private IList<PropertyMapping> _identities;
		private IDictionary<string, PropertyMapping> _identitiesDic;
		private PropertyMapping _identityProperty = null;

		private EntityMapping()
		{
			this._columns = new List<PropertyMapping>();
			this._identities = new List<PropertyMapping>();
			this._identitiesDic = new Dictionary<string, PropertyMapping>();
		}
		private static TableAttribute GetTableAttribute(Type entityType)
		{
			if (entityType == null) return null;
			object[] objects = entityType.GetCustomAttributes(typeof(TableAttribute), true);
			if (objects == null || objects.Length <= 0) return null;
			return objects[0] as TableAttribute;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
		public static EntityMapping Create(Type entityType)
		{
			TableAttribute table = GetTableAttribute(entityType);
			if (table == null)
			{
				log.WarnFormat("No TableAttribute in class {0}", entityType.Name);
				return null;
			}

			PropertyInfo[] properties = entityType.GetProperties();
			if (properties == null || properties.Length <= 0)
			{
				log.WarnFormat("No properties in class {0}", entityType.Name);
				return null;
			}

			EntityMapping entityMapping = new EntityMapping();
			if (string.IsNullOrEmpty(table.Name) || table.Name.Trim().Length <= 0)
				entityMapping._tableName = entityType.Name;
			else
				entityMapping._tableName = table.Name;

			ArrayList getters = new ArrayList();
			ArrayList setters = new ArrayList();
			int index = 0;
			foreach (PropertyInfo property in properties)
			{
				PropertyMapping propertyMapping = PropertyMapping.Create(property, ref index);
				if (propertyMapping != null)
				{
					//这要求属性必须有get, set方法
					getters.Add(ReflectHelper.GetGetter(entityType, property.Name, null));
					setters.Add(ReflectHelper.GetSetter(entityType, property.Name, null));

					entityMapping._columns.Add(propertyMapping);
					if (propertyMapping.IsPrimary)
					{
						entityMapping._identities.Add(propertyMapping);
						entityMapping._identitiesDic.Add(propertyMapping.PropertyName, propertyMapping);
					}
					if (propertyMapping.IsSequence)
						if (entityMapping._identityProperty != null) throw new Exception(string.Format("Entity {0} can only have one sequence property", entityMapping.EntityName));
						else
							entityMapping._identityProperty = propertyMapping;
				}
			}

			if (entityMapping._columns.Count <= 0)
			{
				log.WarnFormat("No properties need to be persisted in class {0}", entityType.Name);
				return null;
			}

			ReflectionOptimizerCache.Cache(entityType, setters.ToArray(typeof(ISetter)) as ISetter[], getters.ToArray(typeof(IGetter)) as IGetter[]);

			entityMapping._entityType = entityType;
			return entityMapping;
		}
		public bool ExistsPropertyMapping(string propertyName)
		{
			for (int i = 0; i < this._columns.Count; i++)
				if (this._columns[i].PropertyName == propertyName)
					return true;
			return false;
		}
		public string TableName
		{
			get { return this._tableName; }
		}
		public Type EntityType
		{
			get { return this._entityType; }
		}
		public string EntityName
		{
			get { return this._entityType.Name; }
		}

		public IList<PropertyMapping> Properties
		{
			get { return this._columns; }
		}
		public PropertyMapping GetPropertyMapping(string propertyName)
		{
			for (int i = 0; i < this._columns.Count; i++)
				if (this._columns[i].PropertyName == propertyName)
					return this._columns[i];
			return null;
		}
		public IList<PropertyMapping> Identities
		{
			get
			{
				return this._identities;
			}
		}
		public IDictionary<string, PropertyMapping> IdentitiesDictionary
		{
			get
			{
				return this._identitiesDic;
			}
		}
		public bool ExistsIdentityProperty()
		{
			return this._identityProperty != null;
		}
		public PropertyMapping GetIdentityProperty()
		{
			return this._identityProperty;
		}

		public override bool Equals(object obj)
		{
            if (obj == null) return false;
            if (object.ReferenceEquals(this, obj)) return true;
			EntityMapping mapping = obj as EntityMapping;
			if (mapping == null) return false;
			return this._entityType == mapping.EntityType;
		}
		public override int GetHashCode()
		{
			return this._entityType.GetHashCode();
		}
	}
}
