//===============================================================================
// Richie (http://www.cnblogs.com/riccc)
// December 06, 2007
//===============================================================================

using System;

namespace Pandora.Basis.Entity.Mapping
{
	/// <summary>
	/// Database column information
	/// </summary>
	[Serializable, AttributeUsage(AttributeTargets.Property)]
	public sealed class ColumnAttribute : Attribute
	{
		private string _columnName = string.Empty;
		private int _length = 0;
		private byte _precision = 0;
		private byte _scale = 0;
		private bool _isPrimary = false;
		private bool _nullable = true;
		private bool _insertable = true;
		private bool _updatable = true;
		private string _sequenceName;
		private object _defaultValue = null;
		private bool _isSequence = false;

        /// <summary>
        /// 
        /// </summary>
		public ColumnAttribute()
		{
		}

        /// <summary>
        /// The database column name
        /// </summary>
		public string Name
		{
			get
			{
				return this._columnName;
			}
			set { this._columnName = value; }
		}
		
        /// <summary>
        /// The length of the field
        /// </summary>
		public int Length
		{
			get
			{
				return this._length;
			}
			set { this._length = value; }
		}
        /// <summary>
        /// Precision of number fields
        /// </summary>
		public byte Precision
		{
			get
			{
				return this._precision;
			}
			set { this._precision = value; }
		}
        /// <summary>
        /// 
        /// </summary>
		public byte Scale
		{
			get
			{
				return this._scale;
			}
			set { this._scale = value; }
		}

        /// <summary>
        /// 
        /// </summary>
		public bool IsPrimary
		{
			get
			{
				return this._isPrimary;
			}
			set { this._isPrimary = value; }
		}
        /// <summary>
        /// 
        /// </summary>
		public bool Nullable
		{
			get
			{
				return this._nullable;
			}
			set { this._nullable = value; }
		}
        /// <summary>
        /// 
        /// </summary>
		public bool Insertable
		{
			get
			{
				return this._insertable;
			}
			set { this._insertable = value; }
		}
        /// <summary>
        /// 
        /// </summary>
		public bool Updatable
		{
			get
			{
				return this._updatable;
			}
			set { this._updatable = value; }
		}
        /// <summary>
        /// Sequence name for Orcale sequence
        /// </summary>
		public string SequenceName
		{
			get { return this._sequenceName; }
			set { this._sequenceName = value; }
		}
        /// <summary>
        /// For future purpose
        /// </summary>
		public object DefaultValue
		{
			get { return this._defaultValue; }
		}
        /// <summary>
        /// Is this field using the Identity(SQL Server) or Sequence(Oracle) ?
        /// </summary>
		public bool IsSequence
		{
			get { return this._isSequence; }
			set { this._isSequence = value; }
		}
	}
}
