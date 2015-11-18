using System;
using System.Collections;

namespace Pandora.Invest.MThread
{
	public class MThreadContext
	{
		private IDictionary _context = new Hashtable();
		
		public object Get(string name){
			if(!this._context.Contains(name)) return null;
			return this._context[name];
		}
		
		public void Put(string name, object value){
			if(this._context.Contains(name))
				this._context[name] = value;
			else
				this._context.Add(name, value);
		}
		
		public bool Contains(string name){
			return this._context.Contains(name);
		}
		
		public void Remove(string name){
			this._context.Remove(name);
		}
	}
}