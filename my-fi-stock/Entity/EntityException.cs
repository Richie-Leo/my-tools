using System;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// 实体操作时的各种异常
	/// </summary>
	public class EntityException : Exception
	{
		public EntityException(string message) : base(message) {}
		
		public EntityException(string message, Exception ex) : base(message, ex){}
	}
}