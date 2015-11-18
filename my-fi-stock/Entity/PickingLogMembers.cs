using System;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	public partial class PickingLog
	{
		public void Create(Database db){
			db.ExecNonQuery(
				string.Format(
					"insert into {0} ({1},{2},{3},{4},{5}) values(?strategy, ?stime, ?etime, ?mode, ?param)"
					, Mapper.TableName, Mapper.Strategy, Mapper.StartTime, Mapper.EndTime, Mapper.PickingMode, Mapper.Params
				)
				, new string[] {"strategy","stime","etime", "mode", "param"}
				, new object[] {this.Strategy, this.StartTime, this.EndTime, this.PickingMode, this.Params});
			this.PickId = Convert.ToInt32(db.ExecScalar("select last_insert_id()", null, null));
		}
		
		public void UpdateEndTime(Database db){
			db.ExecNonQuery(
				string.Format("update {0} set {1}=?time where {2}=?id", Mapper.TableName, Mapper.EndTime, Mapper.PickId)
			    	, new string[]{ "time", "id" }
			    	, new object[]{ this.EndTime, this.PickId });
		}
	}
}