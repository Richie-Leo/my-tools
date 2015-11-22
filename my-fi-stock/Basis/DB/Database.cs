using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace Pandora.Basis.DB
{
	public class DatabaseException : Exception
	{
		public DatabaseException(string message) : base(message) { }
		public DatabaseException(string message, Exception ex) : base(message, ex) { }
	}

	public class Database
	{
		private string _connectionString;
		private MySqlConnection _connection;
		private MySqlTransaction _transaction;
		
		public Database(string connnectionString)
		{
			this._connectionString = connnectionString;
			this._connection = new MySqlConnection(this._connectionString);
		}
		
		public Database Open(){
			try{
				this._connection.Open();
				return this;
			}catch(Exception ex){
				throw new DatabaseException("Can not connect to database：\n" + ex.Message, ex);
			}
		}
		
		public bool IsOpen(){
			return this._connection!=null && this._connection.State == ConnectionState.Open;
		}
		
		public Database Close(){
			try{
				this._connection.Close();
				return this;
			}catch(Exception ex){
				throw new DatabaseException("Can not close database connection：\n" + ex.Message, ex);
			}
		}
		
		public Database BeginTransaction(){
			try{
				this._transaction = this._connection.BeginTransaction(IsolationLevel.ReadCommitted);
				return this;
			}catch(Exception ex){
				throw new DatabaseException("Can not begin database transaction：\n" + ex.Message, ex);
			}
		}
		
		public Database CommitTransaction(){
			try{
				if(this._transaction==null) return this;
				this._transaction.Commit();
				this._transaction = null;
				return this;
			}catch(Exception ex){
				throw new DatabaseException("Can not commit database transaction：\n" + ex.Message, ex);
			}
		}
		
		public Database RollbackTransaction(){
			try{
				if(this._transaction==null) return this;
				this._transaction.Rollback();
				this._transaction = null;
				return this;
			}catch(Exception ex){
				throw new DatabaseException("Can not rollback database transaction：\n" + ex.Message, ex);
			}
		}
		
		public int ExecNonQuery(string sql, string[] paramNames, object[] paramValues){
			try{
				return this.BuildCommand(sql, paramNames, paramValues).ExecuteNonQuery();
			}catch(Exception ex){
				throw new DatabaseException("Can not execute sql command：\n" + ex.Message, ex);
			}
		}
		
		public object ExecScalar(string sql, string[] paramNames, object[] paramValues){
			try{
				return this.BuildCommand(sql, paramNames, paramValues).ExecuteScalar();
			}catch(Exception ex){
				throw new DatabaseException("Can not execute sql command：\n" + ex.Message, ex);
			}
		}
		
		public DataSet ExecDataSet(string sql, string[] paramNames, object[] paramValues){
			MySqlCommand cmd = this.BuildCommand(sql, paramNames, paramValues);
			MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
			DataSet ds = new DataSet();
			adapter.Fill(ds);
			cmd.Dispose();
			adapter.Dispose();
			return ds;
		}
		
		public BulkUpdate CreateBulkUpdate(string table, string[] valueColumns, string[] idColumns, int batchSize){
			return new BulkUpdate(this, table, valueColumns, idColumns, batchSize);
		}
		
		private MySqlCommand BuildCommand(string sql, string[] paramNames, object[] paramValues){
			MySqlCommand cmd = new MySqlCommand();
			cmd.Connection = this._connection;
			cmd.CommandText = sql;
			if(paramNames!=null && paramValues!=null){
				for(int i=0; i<paramNames.Length; i++){
					cmd.Parameters.AddWithValue(paramNames[i].StartsWith("?", StringComparison.Ordinal) ? "" : "?" + paramNames[i]
					                            , paramValues[i]);
				}
			}
			return cmd;
		}
		
		public static string SQLFieldStringValue(Object obj){
			if(obj==null) return "null";
			if(obj.GetType().Equals(typeof(DateTime)))
				return "'" + ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss") + "'";
			if(obj is string)
				return "'" + obj.ToString() + "'";
			if(obj.GetType().Equals(typeof(decimal)))
				return ((decimal)obj).ToString("0.00");
			return obj.ToString();
		}
	}
	
	public class BulkUpdate{
		private Database _db;
		private string _table;
		private string[] _valueColumns;
		private string[] _idColumns;
		private int _batchSize;
		private List<object[]> _valueRows;
		private List<object[]> _idRows;
		
		internal BulkUpdate(Database db, string table, string[] valueColumns, string[] idColumns, int batchSize) {
			this._db = db;
			if(string.IsNullOrEmpty(table) || table.Trim().Length<=0)
				throw new DatabaseException("Parameter 'table' is empty or null in BulkUpdate");
			this._table = table;
			if(valueColumns==null || valueColumns.Length<=0)
				throw new DatabaseException("Parameter 'valueColumns' is empty or null in BulkUpdate");
			this._valueColumns = valueColumns;
			if(idColumns==null || idColumns.Length<=0)
				throw new DatabaseException("Parameter 'idColumns' is empty or null in BulkUpdate");
			this._idColumns = idColumns;
			this._batchSize = batchSize <=0 ? 50 : batchSize;
			this._valueRows = new List<object[]>(batchSize);
			this._idRows = new List<object[]>(batchSize);
		}
		
		public BulkUpdate PushValues(object[] valueRow, object[] idRow){
			if(valueRow==null || valueRow.Length<=0 || idRow==null || idRow.Length<=0) return this;
			if(this._valueColumns.Length != valueRow.Length || this._idColumns.Length != idRow.Length)
				throw new DatabaseException(string.Format("批量执行数据库更新操作，值 数量({0})必须等于 值字段 数量({1})并且 ID 数量({2})必须等于 ID字段 数量({3})"
					, valueRow.Length, this._valueColumns.Length, idRow.Length, this._idColumns.Length));
			this._valueRows.Add(valueRow);
			this._idRows.Add(idRow);
			if(this._valueRows.Count>=this._batchSize) this.Flush();
			return this;
		}
		
		public BulkUpdate Flush(){
			if(this._valueRows==null || this._valueRows.Count<=0) return this;
			
			StringBuilder sql = new StringBuilder();
			for(int i=0; i<this._valueRows.Count; i++) {
				sql.Append("update `").Append(this._table).Append("` set");
				for(int j=0; j<this._valueColumns.Length; j++){
					if(j!=0) sql.Append(',');
					sql.Append(" `").Append(this._valueColumns[j]).Append("`=")
						.Append(Database.SQLFieldStringValue(this._valueRows[i][j]));
				}
				sql.Append(" where");
				for(int j=0; j<this._idColumns.Length; j++){
					if(j!=0) sql.Append("and");
					sql.Append(" `").Append(this._idColumns[j]).Append("`=")
						.Append(Database.SQLFieldStringValue(this._idRows[i][j]));
				}
				sql.Append(";\n");
			}
			
			this._db.ExecNonQuery(sql.ToString(), null, null);
			
			this._valueRows.Clear();
			this._idRows.Clear();
			return this;
		}
	}
}
