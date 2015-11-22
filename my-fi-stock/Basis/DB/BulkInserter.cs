using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Basis.DB
{
	public class BulkInserter<T>
	{
		private Database _db;
		private string _table;
		private string[] _columns;
		private int _batchSize;
		private List<object[]> _rows;
		
		public BulkInserter(Database db, string table, string[] columns, int batchSize) {
			this._db = db;
			if(string.IsNullOrEmpty(table) || table.Trim().Length<=0)
				throw new DatabaseException("批量执行数据库插入操作，必须提供表名：参数table为空");
			this._table = table;
			if(columns==null || columns.Length<=0)
				throw new DatabaseException("批量执行数据库插入操作，必须提供需要插入的列名：参数columns为空");
			this._columns = columns;
			this._batchSize = batchSize <=0 ? 50 : batchSize;
			this._rows = new List<object[]>(batchSize);
		}
		
		public virtual BulkInserter<T> Push(T entity){
			return this;
		}
		
		protected BulkInserter<T> Push(object[] row){
			if(row==null || row.Length<=0) return this;
			if(this._columns.Length != row.Length)
				throw new DatabaseException(string.Format("批量执行数据库插入操作，值 数量({0})必须等于 字段 数量({1})"
				                                          , row.Length, this._columns.Length));
			this._rows.Add(row);
			if(this._rows.Count>=this._batchSize) this.Flush();
			return this;
		}
		
		public BulkInserter<T> Flush(){
			if(this._rows==null || this._rows.Count<=0) return this;
			
			StringBuilder sql = new StringBuilder();
			sql.Append("insert into `").Append(this._table).Append("`(");
			for(int i=0; i<this._columns.Length; i++){
				sql.Append('`').Append(this._columns[i]).Append('`');
				if(i != this._columns.Length-1) sql.Append(',');
			}
			sql.Append(") values ");
			
			for(int i=0; i<this._rows.Count; i++) {
				sql.Append("\n  (");
				for(int j=0; j<this._rows[i].Length; j++){
					sql.Append(Database.SQLFieldStringValue(this._rows[i][j]));
					if(j != this._rows[i].Length-1) sql.Append(',');
				}
				sql.Append(')');
				if(i != this._rows.Count-1) sql.Append(',');
			}
			
			this._db.ExecNonQuery(sql.ToString(), null, null);
			
			this._rows.Clear();
			return this;
		}
	}
}
