//===============================================================================
// Richie (http://www.cnblogs.com/riccc)
// December 06, 2007
//===============================================================================

using System;

namespace Pandora.Basis.Entity.Mapping
{
	/// <summary>
	/// 
	/// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class TableAttribute : Attribute
	{
		private string _tableName;

        /// <summary>
        /// 
        /// </summary>
		public TableAttribute()
		{
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
		public TableAttribute(string name)
		{
			this._tableName = name;
		}

        /// <summary>
        /// 
        /// </summary>
		public string Name
		{
			get { return this._tableName; }
		}
	}
}
