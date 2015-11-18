//===============================================================================
// Richie (http://www.cnblogs.com/riccc)
// December 06, 2007
//===============================================================================

using System;
using System.Reflection;
using Magic.Framework.Data;

namespace Magic.Framework.ORM.Mapping
{
    public class PropertyMapping
	{
		private ColumnAttribute _column;
		private Type _propertyType;
		private string _propertyName;
		private int _index;

		private PropertyMapping()
		{
		}
		public static PropertyMapping Create(PropertyInfo propInfo)
		{
			if (propInfo == null) return null;

			object[] objects = propInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
			if (objects == null || objects.Length <= 0) return null;
			ColumnAttribute attribute = objects[0] as ColumnAttribute;
			if (attribute == null) return null;

			PropertyMapping mapping = new PropertyMapping();
			mapping._propertyName = propInfo.Name;
			mapping._propertyType = propInfo.PropertyType;
			mapping._column = attribute;
			
			return mapping;
		}

		public Type PropertyType
		{
			get { return this._propertyType; }
		}
		public string PropertyName
		{
			get { return this._propertyName; }
		}

		public string ColumnName
		{
			get {
				if (string.IsNullOrEmpty(this._column.Name)) return this._propertyName;
				else return this._column.Name;
			}
		}
		public int Length
		{
			get
			{
				return this._column.Length;
			}
		}
		public byte Precision
		{
			get
			{
				return this._column.Precision;
			}
		}
		public byte Scale
		{
			get
			{
				return this._column.Scale;
			}
		}
		public bool IsPrimary
		{
			get
			{
				return this._column.IsPrimary;
			}
		}
		public bool Nullable
		{
			get
			{
				return this._column.Nullable;
			}
		}
		public bool Insertable
		{
			get
			{
				return this._column.Insertable;
			}
		}
		public bool Updatable
		{
			get
			{
				return this._column.Updatable;
			}
		}
		public string SequenceName
		{
			get { return this._column.SequenceName; }
		}
		public object DefaultValue
		{
			get { return this._column.DefaultValue; }
		}
		public bool IsSequence
		{
			get { return this._column.IsSequence; }
		}

		public override int GetHashCode()
		{
			return this._propertyName.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			PropertyMapping mapping = obj as PropertyMapping;
			if (mapping == null) return false;
			return this._propertyName == mapping.PropertyName;
		}
	}
}