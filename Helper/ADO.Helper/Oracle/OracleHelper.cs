﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OracleClient;
using ADO.Helper.TXT;

namespace ADO.Helper.Oracle
{
    /// <summary>
    /// Oracle数据库帮助类
    /// 创建日期:2016年12月14日
    /// </summary>
    public class OracleHelper
    {
        /// <summary>
        /// Oracle连接字符串
        /// </summary>
        public static string strOracleConnection { get; set; }

        /// <summary>
        /// 表示一个到 Oracle 数据库的打开的连接
        /// </summary>
        OracleConnection Connection = null;

        /// <summary>
        /// 表示要在 Oracle 数据库中处理的 Transact-SQL 事务
        /// </summary>
        OracleTransaction Transaction = null;

        /// <summary>
        /// 直接传给帮助类Oracle连接字符串
        /// </summary>
        /// <param name="OracleConnection">Oracle连接字符串</param>
        public void OracleConnectionString(string OracleConnection)
        {
            strOracleConnection = OracleConnection;
        }

        /// <summary>
        /// 传给帮助类Oracle连接字符串需要的信息
        /// </summary>
        /// <param name="Source">IP地址和数据库名(可选)</param>
        /// <param name="Id">用户名</param>
        /// <param name="Password">密码</param>
        public void OracleConnectionString(string Source, string Id, string Password)
        {
            strOracleConnection = string.Format("data source={0};user id={1};password={2};", Source, Id, Password);
        }

        /// <summary>
        /// 得到数据库连接字符串
        /// </summary>
        /// <returns>数据库连接字符串</returns>
        public string GetConnectionString()
        {
            return strOracleConnection;
        }

        /// <summary>
        /// 使用所指定的属性设置打开数据库连接
        /// </summary>
        /// <returns>成功返回0,失败返回-1</returns>
        public int Open()
        {
            try
            {
                Connection = new OracleConnection(strOracleConnection);
                Connection.Open();
                if (Connection.State == ConnectionState.Open)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 关闭与数据库的连接
        /// </summary>
        /// <returns>成功返回0,失败返回-1</returns>
        public int Close()
        {
            try
            {
                Connection.Close();
                if (Connection.State == ConnectionState.Closed)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>成功返回0,失败返回-1</returns>
        public int BeginTransaction()
        {
            try
            {
                Transaction = Connection.BeginTransaction();
                return 0;
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns>成功返回0,失败返回-1</returns>
        public int CommitTransaction()
        {
            try
            {
                if (Transaction != null)
                {
                    Transaction.Commit();
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns>成功返回0,失败返回-1</returns>
        public int RollbackTransaction()
        {
            try
            {
                if (Transaction != null)
                {
                    Transaction.Rollback();
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 创建数据库表
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="strNameType">数据库表字段名和字段类型</param>
        /// <returns>成功返回0,失败返回-1(创建数据库表无法返回行数)</returns>
        public int CreateTable(string strTableName, string strNameType)
        {
            try
            {
                if (string.IsNullOrEmpty(strTableName) || string.IsNullOrEmpty(strNameType)) return -1;
                string sqlCreateTable = string.Format("CREATE TABLE {0} ( {1} )", strTableName, strNameType);
                sqlCreateTable = ADO.Helper.DatabaseConversion.SqlProcessing.RemoveIllegal(sqlCreateTable);
                OracleCommand Command = new OracleCommand(sqlCreateTable, Connection);
                if (Transaction != null)
                {
                    Command.Transaction = Transaction;
                }
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 创建数据库表(Dictionary储存列名和列类型)
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="dicFieldNameType">数据库表字段名和字段类型</param>
        /// <returns>成功返回0,失败返回-1(创建数据库表无法返回行数)</returns>
        public int CreateTable(string strTableName, Dictionary<string, string> dicFieldNameType)
        {
            try
            {
                if (string.IsNullOrEmpty(strTableName) || dicFieldNameType.Count < 1) return -1;
                string strNameType = "";
                foreach (var varFieldNameType in dicFieldNameType)
                {
                    string strKei = varFieldNameType.Key;
                    string strValue = varFieldNameType.Value;
                    strNameType += string.Format("{0} {1},", strKei, strValue);
                }
                strNameType = strNameType.Substring(0, strNameType.Length - 1);
                string sqlCreateTable = string.Format("CREATE TABLE {0} ( {1} )", strTableName, strNameType);
                sqlCreateTable = ADO.Helper.DatabaseConversion.SqlProcessing.RemoveIllegal(sqlCreateTable);
                OracleCommand Command = new OracleCommand(sqlCreateTable, Connection);
                if (Transaction != null)
                {
                    Command.Transaction = Transaction;
                }
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 删除数据库表
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <returns>成功返回0,失败返回-1(删除据库表无法返回行数)</returns>
        public int DropTable(string strTableName)
        {
            try
            {
                if (string.IsNullOrEmpty(strTableName)) return -1;
                string sqlCreateTable = string.Format("DROP TABLE {0}", strTableName);
                OracleCommand Command = new OracleCommand(sqlCreateTable, Connection);
                if (Transaction != null)
                {
                    Command.Transaction = Transaction;
                }
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 删除数据库表中的内容
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <returns>成功返回受影响的行数,失败返回-1</returns>
        public int DeleteTableData(string strTableName)
        {
            int intNumberIfAffectedRows = 0;
            try
            {
                if (string.IsNullOrEmpty(strTableName)) return -1;
                string sqlCreateTable = string.Format("DELETE FROM {0}", strTableName);
                OracleCommand Command = new OracleCommand(sqlCreateTable, Connection);
                if (Transaction != null)
                {
                    Command.Transaction = Transaction;
                }
                intNumberIfAffectedRows = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 得到数据库中所有表名
        /// </summary>
        /// <returns>Oracle中所有数据库名(listString)</returns>
        public List<string> GetAllTableName()
        {
            List<string> listAllTable = new List<string>();
            try
            {
                string sqlAllTable = string.Format("SELECT TABLE_NAME FROM USER_TABLES");
                OracleDataAdapter DataAdapter = new OracleDataAdapter(sqlAllTable, Connection);
                if (Transaction != null)
                {
                    DataAdapter.SelectCommand.Transaction = Transaction;
                }
                DataTable dtAllTable = new DataTable();
                DataAdapter.Fill(dtAllTable);
                if (dtAllTable.Rows.Count < 1) return null;
                foreach (DataRow drTableName in dtAllTable.Rows)
                {
                    listAllTable.Add(drTableName["TABLE_NAME"].ToString());
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
            }
            return listAllTable;
        }

        /// <summary>
        /// 得到指定表中所有字段名
        /// </summary>
        /// <param name="strTableName">指定表名</param>
        /// <returns>表中所有字段名(listString)</returns>
        public List<string> GetAllFieldName(string strTableName)
        {
            List<string> listAllTableField = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(strTableName)) return null;
                string sqlAllTable = string.Format("SELECT COLUMN_NAME FROM COLS WHERE TABLE_NAME='{0}'", strTableName);
                OracleDataAdapter DataAdapter = new OracleDataAdapter(sqlAllTable, Connection);
                if (Transaction != null)
                {
                    DataAdapter.SelectCommand.Transaction = Transaction;
                }
                DataTable dtAllTable = new DataTable();
                DataAdapter.Fill(dtAllTable);
                if (dtAllTable.Rows.Count < 1) return null;
                foreach (DataRow drTableName in dtAllTable.Rows)
                {
                    listAllTableField.Add(drTableName["COLUMN_NAME"].ToString());
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
            }
            return listAllTableField;
        }

        /// <summary>
        /// 得到指定表中所有字段名和字段类型
        /// </summary>
        /// <param name="strTableName">指定表名</param>
        /// <returns>表中所有字段名和字段类型(Dictionary)</returns>
        public Dictionary<string, string> GetAllFieldNameType(string strTableName)
        {
            Dictionary<string, string> dicAllTableField = new Dictionary<string, string>();
            try
            {
                if (string.IsNullOrEmpty(strTableName)) return null;
                string sqlAllTable = string.Format("SELECT TABLE_NAME,COLUMN_NAME,DATA_TYPE,DATA_LENGTH FROM COLS WHERE TABLE_NAME='{0}'", strTableName.ToUpper());
                OracleDataAdapter DataAdapter = new OracleDataAdapter(sqlAllTable, Connection);
                if (Transaction != null)
                {
                    DataAdapter.SelectCommand.Transaction = Transaction;
                }
                DataTable dtAllTable = new DataTable();
                DataAdapter.Fill(dtAllTable);
                if (dtAllTable.Rows.Count < 1) return null;
                foreach (DataRow drTableName in dtAllTable.Rows)
                {
                    string strDataType = "";
                    if (string.IsNullOrEmpty(drTableName["DATA_LENGTH"].ToString()))
                    {
                        strDataType += string.Format("{0}", drTableName["DATA_TYPE"].ToString().ToUpper());
                    }
                    else
                    {
                        strDataType += string.Format("{0}({1})", drTableName["DATA_TYPE"].ToString().ToUpper(), drTableName["DATA_LENGTH"].ToString());
                    }
                    dicAllTableField.Add(drTableName["COLUMN_NAME"].ToString(), strDataType);
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
            }
            return dicAllTableField;
        }

        /// <summary>
        /// 执行非查询SQL语句
        /// </summary>
        /// <param name="sqlExecuteNonQuery">非查询SQL</param>
        /// <returns>成功返回受影响的行数,失败或非数据操作成功返回-1</returns>
        public int ExecuteNonQuery(string sqlExecuteNonQuery)
        {
            int intNumberIfAffectedRows = 0;
            try
            {
                if (string.IsNullOrEmpty(sqlExecuteNonQuery))
                {
                    return -1;
                }
                OracleCommand Command = new OracleCommand(sqlExecuteNonQuery, Connection);
                if (Transaction != null)
                {
                    Command.Transaction = Transaction;
                }
                intNumberIfAffectedRows = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 执行非查询SQL语句(批量执行)
        /// </summary>
        /// <param name="listExecuteNonQuery">SQL语句集合</param>
        /// <returns>成功返回执行条数,失败返回-1</returns>
        public int ExecuteNonQuery(List<string> listExecuteNonQuery)
        {
            int intNumberIfAffectedRows = 0;
            try
            {
                if (listExecuteNonQuery.Count < 1)
                {
                    return -1;
                }
                foreach (string sqlExecuteNonQuery in listExecuteNonQuery)
                {
                    OracleCommand Command = new OracleCommand(sqlExecuteNonQuery, Connection);
                    if (Transaction != null)
                    {
                        Command.Transaction = Transaction;
                    }
                    if (Command.ExecuteNonQuery() > 0)
                    {
                        intNumberIfAffectedRows++;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 执行查询SQL语句(存到DataSet)
        /// </summary>
        /// <param name="sqlSelect">查询SQL</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string sqlSelect)
        {
            DataSet dsSelect = new DataSet();
            try
            {
                if (string.IsNullOrEmpty(sqlSelect)) return null;
                OracleDataAdapter DataAdapter = new OracleDataAdapter(sqlSelect, Connection);
                if (Transaction != null)
                {
                    DataAdapter.SelectCommand.Transaction = Transaction;
                }
                DataAdapter.Fill(dsSelect);
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
            }
            return dsSelect;
        }

        /// <summary>
        /// 执行查询SQL语句(存到DataSet)(多表)
        /// </summary>
        /// <param name="listSelect">查询SQL集合</param>
        /// <returns>DataSet(多表)</returns>
        public DataSet GetDataSet(List<string> listSelect)
        {
            DataSet dsSelect = new DataSet();
            string strTableName = string.Format("Table_");
            int intTableNumber = 0;
            try
            {
                if (listSelect.Count < 1) return null;
                foreach (string sqlSelect in listSelect)
                {
                    OracleDataAdapter DataAdapter = new OracleDataAdapter(sqlSelect, Connection);
                    if (Transaction != null)
                    {
                        DataAdapter.SelectCommand.Transaction = Transaction;
                    }
                    DataAdapter.Fill(dsSelect, strTableName + intTableNumber++);
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
            }
            return dsSelect;
        }

        /// <summary>
        /// 执行查询SQL语句(存到DataTable)
        /// </summary>
        /// <param name="sqlSelect">查询SQL</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sqlSelect)
        {
            DataTable dtSelect = new DataTable();
            try
            {
                if (string.IsNullOrEmpty(sqlSelect)) return null;
                OracleDataAdapter DataAdapter = new OracleDataAdapter(sqlSelect, Connection);
                if (Transaction != null)
                {
                    DataAdapter.SelectCommand.Transaction = Transaction;
                }
                DataAdapter.Fill(dtSelect);
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
            }
            return dtSelect;
        }

        /// <summary>
        /// 保存数据(追加DataTable数据到指定数据库表)
        /// 根据字段名称查找DataTable中的数据并写入数据库
        /// 自动获取目标数据库表的字段名和类型
        /// 主键字段数据冲突或者为空会导致保存失败
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="dtSourceData">源数据DataTable</param>
        /// <returns>成功返回执行条数,失败返回-1</returns>
        public int SaveData(string strTableName, DataTable dtSourceData)
        {
            int intNumberIfAffectedRows = 0;
            List<string> listExecuteNonQuery = new List<string>();
            Dictionary<string, string> dicFieldNameType = GetAllFieldNameType(strTableName);
            try
            {
                if (string.IsNullOrEmpty(strTableName) || dtSourceData.Rows.Count < 1) return -1;
                foreach (DataRow drSourceData in dtSourceData.Rows)
                {
                    string sqlInsertInto = string.Format("INSERT INTO {0} ", strTableName);
                    string sqlFieldName = string.Format("(");
                    string sqlFieldValue = string.Format("(");
                    foreach (var varFieldNameType in dicFieldNameType)
                    {
                        string strKei = varFieldNameType.Key;
                        string strValue = varFieldNameType.Value.ToUpper();
                        //二进制格式或类型为空或DataTable中不包含指定列退出循环(不支持二进制保存)
                        if (strValue.IndexOf("RAW") > -1 || strValue.IndexOf("BLOB") > -1 || strValue.IndexOf("CLOB") > -1 || strValue.IndexOf("NROWID") > -1 || string.IsNullOrEmpty(strValue) || !dtSourceData.Columns.Contains(strKei) || drSourceData.IsNull(strKei))
                        {
                            continue;
                        }
                        //拼写SQL,如果是数值型单独处理(不加'')
                        if (strValue.IndexOf("INT") > -1 || strValue.IndexOf("FLOAT") > -1 || strValue.IndexOf("NUMBER") > -1 || strValue.IndexOf("DOUBLE") > -1 || strValue.IndexOf("DECIMAL") > -1)
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", string.IsNullOrEmpty(drSourceData[strKei].ToString()) ? "null" : drSourceData[strKei].ToString());
                        }
                        else
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("'{0}',", drSourceData[strKei].ToString().Replace("\'", "\'\'"));
                        }
                    }
                    sqlFieldName = sqlFieldName.Substring(0, sqlFieldName.Length - 1);
                    sqlFieldName += string.Format(")");
                    sqlFieldValue = sqlFieldValue.Substring(0, sqlFieldValue.Length - 1);
                    sqlFieldValue += string.Format(")");
                    sqlInsertInto += string.Format("{0} VALUES {1}", sqlFieldName, sqlFieldValue);
                    listExecuteNonQuery.Add(sqlInsertInto);
                }
                intNumberIfAffectedRows = ExecuteNonQuery(listExecuteNonQuery);
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 保存数据(追加DataTable数据到指定数据库表)
        /// 根据字段名称查找DataTable中的数据并写入数据库
        /// 自动获取目标数据库表的字段名和类型
        /// 主键字段数据冲突或者为空会导致保存失败
        /// 根据目标与源的字段名对应关系读取DataTable中的数据
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="dtSourceData">源数据DataTable</param>
        /// <param name="dicCorrespondenceBetween">目标与源的字段名对应关系(目标字段名/源字段名)</param>
        /// <returns>成功返回执行条数,失败返回-1</returns>
        public int SaveData(string strTableName, DataTable dtSourceData, Dictionary<string, string> dicCorrespondenceBetween)
        {
            int intNumberIfAffectedRows = 0;
            List<string> listExecuteNonQuery = new List<string>();
            Dictionary<string, string> dicFieldNameType = GetAllFieldNameType(strTableName);
            try
            {
                if (string.IsNullOrEmpty(strTableName) || dtSourceData.Rows.Count < 1) return -1;
                foreach (DataRow drSourceData in dtSourceData.Rows)
                {
                    string sqlInsertInto = string.Format("INSERT INTO {0} ", strTableName);
                    string sqlFieldName = string.Format("(");
                    string sqlFieldValue = string.Format("(");
                    foreach (var varFieldNameType in dicFieldNameType)
                    {
                        //目标(数据库)字段名和字段类型
                        string strKei = varFieldNameType.Key;
                        string strValue = varFieldNameType.Value.ToUpper();
                        //源(数据库)字段名,取数据用这个字段名取
                        string strSourceFieldName = varFieldNameType.Key;
                        if (dicCorrespondenceBetween != null)
                        {
                            strSourceFieldName = dicCorrespondenceBetween[strKei].ToString();
                        }
                        //二进制格式或类型为空或DataTable中不包含指定列退出循环(不支持二进制保存)
                        if (strValue.IndexOf("BINARY") > -1 || strValue.IndexOf("IMAGE") > -1 || string.IsNullOrEmpty(strValue) || !dtSourceData.Columns.Contains(strSourceFieldName) || drSourceData.IsNull(strSourceFieldName))
                        {
                            continue;
                        }
                        //拼写SQL,如果是数值型单独处理(不加'')
                        if (strValue.IndexOf("INT") > -1 || strValue.IndexOf("FLOAT") > -1 || strValue.IndexOf("SMALLMONEY") > -1 || strValue.IndexOf("MONEY") > -1 || strValue.IndexOf("DECIMAL") > -1 || strValue.IndexOf("NUMERIC") > -1 || strValue.IndexOf("REAL") > -1)
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", string.IsNullOrEmpty(drSourceData[strSourceFieldName].ToString()) ? "null" : drSourceData[strSourceFieldName].ToString());
                        }
                        else
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("'{0}',", drSourceData[strSourceFieldName].ToString().Replace("\'", "\'\'"));
                        }
                    }
                    sqlFieldName = sqlFieldName.Substring(0, sqlFieldName.Length - 1);
                    sqlFieldName += string.Format(")");
                    sqlFieldValue = sqlFieldValue.Substring(0, sqlFieldValue.Length - 1);
                    sqlFieldValue += string.Format(")");
                    sqlInsertInto += string.Format("{0} VALUES {1}", sqlFieldName, sqlFieldValue);
                    listExecuteNonQuery.Add(sqlInsertInto);
                }
                intNumberIfAffectedRows = ExecuteNonQuery(listExecuteNonQuery);
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 保存数据(追加DataTable数据到指定数据库表)
        /// 根据字段名称查找DataTable中的数据并写入数据库
        /// 手动传给目标数据库表的字段名和类型
        /// 主键字段数据冲突或者为空会导致保存失败
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="dicFieldNameType">目标数据字段名和类型</param>
        /// <param name="dtSourceData">源数据DataTable</param>
        /// <returns>成功返回执行条数,失败返回-1</returns>
        public int SaveData(string strTableName, Dictionary<string, string> dicFieldNameType, DataTable dtSourceData)
        {
            int intNumberIfAffectedRows = 0;
            List<string> listExecuteNonQuery = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(strTableName) || dicFieldNameType.Count < 1 || dtSourceData.Rows.Count < 1) return -1;
                foreach (DataRow drSourceData in dtSourceData.Rows)
                {
                    string sqlInsertInto = string.Format("INSERT INTO {0} ", strTableName);
                    string sqlFieldName = string.Format("(");
                    string sqlFieldValue = string.Format("(");
                    foreach (var varFieldNameType in dicFieldNameType)
                    {
                        string strKei = varFieldNameType.Key;
                        string strValue = varFieldNameType.Value.ToUpper();
                        //二进制格式或类型为空或DataTable中不包含指定列退出循环(不支持二进制保存)
                        if (strValue.IndexOf("RAW") > -1 || strValue.IndexOf("BLOB") > -1 || strValue.IndexOf("CLOB") > -1 || strValue.IndexOf("NROWID") > -1 || string.IsNullOrEmpty(strValue) || !dtSourceData.Columns.Contains(strKei) || drSourceData.IsNull(strKei))
                        {
                            continue;
                        }
                        //拼写SQL,如果是数值型单独处理(不加'')
                        if (strValue.IndexOf("INT") > -1 || strValue.IndexOf("FLOAT") > -1 || strValue.IndexOf("NUMBER") > -1 || strValue.IndexOf("DOUBLE") > -1 || strValue.IndexOf("DECIMAL") > -1)
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", string.IsNullOrEmpty(drSourceData[strKei].ToString()) ? "null" : drSourceData[strKei].ToString());
                        }
                        else
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("'{0}',", drSourceData[strKei].ToString().Replace("\'", "\'\'"));
                        }
                    }
                    sqlFieldName = sqlFieldName.Substring(0, sqlFieldName.Length - 1);
                    sqlFieldName += string.Format(")");
                    sqlFieldValue = sqlFieldValue.Substring(0, sqlFieldValue.Length - 1);
                    sqlFieldValue += string.Format(")");
                    sqlInsertInto += string.Format("{0} VALUES {1}", sqlFieldName, sqlFieldValue);
                    listExecuteNonQuery.Add(sqlInsertInto);
                }
                intNumberIfAffectedRows = ExecuteNonQuery(listExecuteNonQuery);
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 保存数据(追加DataTable数据到指定数据库表)
        /// 根据字段名称查找DataTable中的数据并写入数据库
        /// 手动传给目标数据库表的字段名和类型
        /// 主键字段数据冲突或者为空会导致保存失败
        /// 根据目标与源的字段名对应关系读取DataTable中的数据
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="dicFieldNameType">目标数据字段名和类型</param>
        /// <param name="dtSourceData">源数据DataTable</param>
        /// <param name="dicCorrespondenceBetween">目标与源的字段名对应关系(目标字段名/源字段名)</param>
        /// <returns>成功返回执行条数,失败返回-1</returns>
        public int SaveData(string strTableName, Dictionary<string, string> dicFieldNameType, DataTable dtSourceData, Dictionary<string, string> dicCorrespondenceBetween)
        {
            int intNumberIfAffectedRows = 0;
            List<string> listExecuteNonQuery = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(strTableName) || dicFieldNameType.Count < 1 || dtSourceData.Rows.Count < 1) return -1;
                foreach (DataRow drSourceData in dtSourceData.Rows)
                {
                    string sqlInsertInto = string.Format("INSERT INTO {0} ", strTableName);
                    string sqlFieldName = string.Format("(");
                    string sqlFieldValue = string.Format("(");
                    foreach (var varFieldNameType in dicFieldNameType)
                    {
                        //目标(数据库)字段名和字段类型
                        string strKei = varFieldNameType.Key;
                        string strValue = varFieldNameType.Value.ToUpper();
                        //源(数据库)字段名,取数据用这个字段名取
                        string strSourceFieldName = varFieldNameType.Key;
                        if (dicCorrespondenceBetween != null)
                        {
                            strSourceFieldName = dicCorrespondenceBetween[strKei].ToString();
                        }
                        //二进制格式或类型为空或DataTable中不包含指定列退出循环(不支持二进制保存)
                        if (strValue.IndexOf("BINARY") > -1 || strValue.IndexOf("IMAGE") > -1 || string.IsNullOrEmpty(strValue) || !dtSourceData.Columns.Contains(strSourceFieldName) || drSourceData.IsNull(strSourceFieldName))
                        {
                            continue;
                        }
                        //拼写SQL,如果是数值型单独处理(不加'')
                        if (strValue.IndexOf("INT") > -1 || strValue.IndexOf("FLOAT") > -1 || strValue.IndexOf("SMALLMONEY") > -1 || strValue.IndexOf("MONEY") > -1 || strValue.IndexOf("DECIMAL") > -1 || strValue.IndexOf("NUMERIC") > -1 || strValue.IndexOf("REAL") > -1)
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", string.IsNullOrEmpty(drSourceData[strSourceFieldName].ToString()) ? "null" : drSourceData[strSourceFieldName].ToString());
                        }
                        else
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("'{0}',", drSourceData[strSourceFieldName].ToString().Replace("\'", "\'\'"));
                        }
                    }
                    sqlFieldName = sqlFieldName.Substring(0, sqlFieldName.Length - 1);
                    sqlFieldName += string.Format(")");
                    sqlFieldValue = sqlFieldValue.Substring(0, sqlFieldValue.Length - 1);
                    sqlFieldValue += string.Format(")");
                    sqlInsertInto += string.Format("{0} VALUES {1}", sqlFieldName, sqlFieldValue);
                    listExecuteNonQuery.Add(sqlInsertInto);
                }
                intNumberIfAffectedRows = ExecuteNonQuery(listExecuteNonQuery);
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 保存带有二进制字段的数据(追加DataTable数据到指定数据库表)
        /// 根据字段名称查找DataTable中的数据并写入数据库
        /// 自动获取目标数据库表的字段名和类型
        /// 主键字段数据冲突或者为空会导致保存失败
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="dtSourceData">源数据DataTable</param>
        /// <returns>成功返回执行条数,失败返回-1</returns>
        public int SaveByteData(string strTableName, DataTable dtSourceData)
        {
            int intNumberIfAffectedRows = 0;
            Dictionary<string, string> dicFieldNameType = GetAllFieldNameType(strTableName);
            try
            {
                if (string.IsNullOrEmpty(strTableName) || dtSourceData.Rows.Count < 1) return -1;
                foreach (DataRow drSourceData in dtSourceData.Rows)
                {
                    List<string> listByteDataFieldName = new List<string>();
                    int intByteDataNumber = 0;
                    string sqlInsertInto = string.Format("INSERT INTO {0} ", strTableName);
                    string sqlFieldName = string.Format("(");
                    string sqlFieldValue = string.Format("(");
                    foreach (var varFieldNameType in dicFieldNameType)
                    {
                        string strKei = varFieldNameType.Key;
                        string strValue = varFieldNameType.Value.ToUpper();
                        if (string.IsNullOrEmpty(strValue) || !dtSourceData.Columns.Contains(strKei) || drSourceData.IsNull(strKei))
                        {
                            continue;
                        }
                        if (strValue.IndexOf("RAW") > -1 || strValue.IndexOf("BLOB") > -1 || strValue.IndexOf("CLOB") > -1 || strValue.IndexOf("NROWID") > -1)
                        {
                            listByteDataFieldName.Add(strKei);
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", ":file_" + intByteDataNumber++);
                        }
                        //拼写SQL,如果是数值型单独处理(不加'')
                        else if (strValue.IndexOf("INT") > -1 || strValue.IndexOf("FLOAT") > -1 || strValue.IndexOf("NUMBER") > -1 || strValue.IndexOf("DOUBLE") > -1 || strValue.IndexOf("DECIMAL") > -1)
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", string.IsNullOrEmpty(drSourceData[strKei].ToString()) ? "null" : drSourceData[strKei].ToString());
                        }
                        else
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("'{0}',", drSourceData[strKei].ToString().Replace("\'", "\'\'"));
                        }
                    }
                    sqlFieldName = sqlFieldName.Substring(0, sqlFieldName.Length - 1);
                    sqlFieldName += string.Format(")");
                    sqlFieldValue = sqlFieldValue.Substring(0, sqlFieldValue.Length - 1);
                    sqlFieldValue += string.Format(")");
                    sqlInsertInto += string.Format("{0} VALUES {1}", sqlFieldName, sqlFieldValue);
                    try
                    {
                        //带有二进制字段的数据每条数据保存一次
                        OracleCommand Command = new OracleCommand(sqlInsertInto, Connection);
                        foreach (string strByteDataFieldName in listByteDataFieldName)
                        {
                            int intByteNumber = 0;
                            byte[] byteFile = (byte[])drSourceData[strByteDataFieldName];
                            string strSqlFileName = string.Format("{0}", "file_" + intByteNumber++);
                            Command.Parameters.Add(strSqlFileName, OracleType.Blob, byteFile.Length);
                            Command.Parameters[strSqlFileName].Value = byteFile;
                        }
                        if (Transaction != null)
                        {
                            Command.Transaction = Transaction;
                        }
                        if (Command.ExecuteNonQuery() > 0)
                        {
                            intNumberIfAffectedRows++;
                        }
                    }
                    catch (Exception ex)
                    {
                        TXTHelper.Logs(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 保存带有二进制字段的数据(追加DataTable数据到指定数据库表)
        /// 根据字段名称查找DataTable中的数据并写入数据库
        /// 自动获取目标数据库表的字段名和类型
        /// 主键字段数据冲突或者为空会导致保存失败
        /// 根据目标与源的字段名对应关系读取DataTable中的数据
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="dtSourceData">源数据DataTable</param>
        /// <param name="dicCorrespondenceBetween">目标与源的字段名对应关系(目标字段名/源字段名)</param>
        /// <returns>成功返回执行条数,失败返回-1</returns>
        public int SaveByteData(string strTableName, DataTable dtSourceData, Dictionary<string, string> dicCorrespondenceBetween)
        {
            int intNumberIfAffectedRows = 0;
            Dictionary<string, string> dicFieldNameType = GetAllFieldNameType(strTableName);
            try
            {
                if (string.IsNullOrEmpty(strTableName) || dtSourceData.Rows.Count < 1) return -1;
                foreach (DataRow drSourceData in dtSourceData.Rows)
                {
                    List<string> listByteDataFieldName = new List<string>();
                    int intByteDataNumber = 0;
                    string sqlInsertInto = string.Format("INSERT INTO {0} ", strTableName);
                    string sqlFieldName = string.Format("(");
                    string sqlFieldValue = string.Format("(");
                    foreach (var varFieldNameType in dicFieldNameType)
                    {
                        //目标(数据库)字段名和字段类型
                        string strKei = varFieldNameType.Key;
                        string strValue = varFieldNameType.Value.ToUpper();
                        //源(数据库)字段名,取数据用这个字段名取
                        string strSourceFieldName = varFieldNameType.Key;
                        if (dicCorrespondenceBetween != null)
                        {
                            strSourceFieldName = dicCorrespondenceBetween[strKei].ToString();
                        }
                        if (string.IsNullOrEmpty(strValue) || !dtSourceData.Columns.Contains(strSourceFieldName) || drSourceData.IsNull(strSourceFieldName))
                        {
                            continue;
                        }
                        if (strValue.IndexOf("BINARY") > -1 || strValue.IndexOf("IMAGE") > -1)
                        {
                            listByteDataFieldName.Add(strSourceFieldName);
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", "@file_" + intByteDataNumber++);
                        }
                        //拼写SQL,如果是数值型单独处理(不加'')
                        else if (strValue.IndexOf("INT") > -1 || strValue.IndexOf("FLOAT") > -1 || strValue.IndexOf("SMALLMONEY") > -1 || strValue.IndexOf("MONEY") > -1 || strValue.IndexOf("DECIMAL") > -1 || strValue.IndexOf("NUMERIC") > -1 || strValue.IndexOf("REAL") > -1)
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", string.IsNullOrEmpty(drSourceData[strSourceFieldName].ToString()) ? "null" : drSourceData[strSourceFieldName].ToString());
                        }
                        else
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("'{0}',", drSourceData[strSourceFieldName].ToString().Replace("\'", "\'\'"));
                        }
                    }
                    sqlFieldName = sqlFieldName.Substring(0, sqlFieldName.Length - 1);
                    sqlFieldName += string.Format(")");
                    sqlFieldValue = sqlFieldValue.Substring(0, sqlFieldValue.Length - 1);
                    sqlFieldValue += string.Format(")");
                    sqlInsertInto += string.Format("{0} VALUES {1}", sqlFieldName, sqlFieldValue);
                    try
                    {
                        //带有二进制字段的数据每条数据保存一次
                        OracleCommand Command = new OracleCommand(sqlInsertInto, Connection);
                        foreach (string strByteDataFieldName in listByteDataFieldName)
                        {
                            int intByteNumber = 0;
                            byte[] byteFile = (byte[])drSourceData[strByteDataFieldName];
                            string strSqlFileName = string.Format("{0}", "file_" + intByteNumber++);
                            Command.Parameters.Add(strSqlFileName, OracleType.Blob, byteFile.Length);
                            Command.Parameters[strSqlFileName].Value = byteFile;
                        }
                        if (Transaction != null)
                        {
                            Command.Transaction = Transaction;
                        }
                        if (Command.ExecuteNonQuery() > 0)
                        {
                            intNumberIfAffectedRows++;
                        }
                    }
                    catch (Exception ex)
                    {
                        TXTHelper.Logs(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 保存带有二进制字段的数据(追加DataTable数据到指定数据库表)
        /// 根据字段名称查找DataTable中的数据并写入数据库
        /// 自动获取目标数据库表的字段名和类型
        /// 主键字段数据冲突或者为空会导致保存失败
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="dicFieldNameType">目标数据字段名和类型</param>
        /// <param name="dtSourceData">源数据DataTable</param>
        /// <returns>成功返回执行条数,失败返回-1</returns>
        public int SaveByteData(string strTableName, Dictionary<string, string> dicFieldNameType, DataTable dtSourceData)
        {
            int intNumberIfAffectedRows = 0;
            try
            {
                if (string.IsNullOrEmpty(strTableName) || dicFieldNameType.Count < 1 || dtSourceData.Rows.Count < 1) return -1;
                foreach (DataRow drSourceData in dtSourceData.Rows)
                {
                    List<string> listByteDataFieldName = new List<string>();
                    int intByteDataNumber = 0;
                    string sqlInsertInto = string.Format("INSERT INTO {0} ", strTableName);
                    string sqlFieldName = string.Format("(");
                    string sqlFieldValue = string.Format("(");
                    foreach (var varFieldNameType in dicFieldNameType)
                    {
                        string strKei = varFieldNameType.Key;
                        string strValue = varFieldNameType.Value.ToUpper();
                        if (string.IsNullOrEmpty(strValue) || !dtSourceData.Columns.Contains(strKei) || drSourceData.IsNull(strKei))
                        {
                            continue;
                        }
                        if (strValue.IndexOf("RAW") > -1 || strValue.IndexOf("BLOB") > -1 || strValue.IndexOf("CLOB") > -1 || strValue.IndexOf("NROWID") > -1)
                        {
                            listByteDataFieldName.Add(strKei);
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", ":file_" + intByteDataNumber++);
                        }
                        //拼写SQL,如果是数值型单独处理(不加'')
                        else if (strValue.IndexOf("INT") > -1 || strValue.IndexOf("FLOAT") > -1 || strValue.IndexOf("NUMBER") > -1 || strValue.IndexOf("DOUBLE") > -1 || strValue.IndexOf("DECIMAL") > -1)
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", string.IsNullOrEmpty(drSourceData[strKei].ToString()) ? "null" : drSourceData[strKei].ToString());
                        }
                        else
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("'{0}',", drSourceData[strKei].ToString().Replace("\'", "\'\'"));
                        }
                    }
                    sqlFieldName = sqlFieldName.Substring(0, sqlFieldName.Length - 1);
                    sqlFieldName += string.Format(")");
                    sqlFieldValue = sqlFieldValue.Substring(0, sqlFieldValue.Length - 1);
                    sqlFieldValue += string.Format(")");
                    sqlInsertInto += string.Format("{0} VALUES {1}", sqlFieldName, sqlFieldValue);
                    try
                    {
                        //带有二进制字段的数据每条数据保存一次
                        OracleCommand Command = new OracleCommand(sqlInsertInto, Connection);
                        foreach (string strByteDataFieldName in listByteDataFieldName)
                        {
                            int intByteNumber = 0;
                            byte[] byteFile = (byte[])drSourceData[strByteDataFieldName];
                            string strSqlFileName = string.Format("{0}", "file_" + intByteNumber++);
                            Command.Parameters.Add(strSqlFileName, OracleType.Blob, byteFile.Length);
                            Command.Parameters[strSqlFileName].Value = byteFile;
                        }
                        if (Transaction != null)
                        {
                            Command.Transaction = Transaction;
                        }
                        if (Command.ExecuteNonQuery() > 0)
                        {
                            intNumberIfAffectedRows++;
                        }
                    }
                    catch (Exception ex)
                    {
                        TXTHelper.Logs(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }

        /// <summary>
        /// 保存带有二进制字段的数据(追加DataTable数据到指定数据库表)
        /// 根据字段名称查找DataTable中的数据并写入数据库
        /// 自动获取目标数据库表的字段名和类型
        /// 主键字段数据冲突或者为空会导致保存失败
        /// 根据目标与源的字段名对应关系读取DataTable中的数据
        /// </summary>
        /// <param name="strTableName">数据库表名</param>
        /// <param name="dicFieldNameType">目标数据字段名和类型</param>
        /// <param name="dtSourceData">源数据DataTable</param>
        /// <param name="dicCorrespondenceBetween">目标与源的字段名对应关系(目标字段名/源字段名)</param>
        /// <returns>成功返回执行条数,失败返回-1</returns>
        public int SaveByteData(string strTableName, Dictionary<string, string> dicFieldNameType, DataTable dtSourceData, Dictionary<string, string> dicCorrespondenceBetween)
        {
            int intNumberIfAffectedRows = 0;
            try
            {
                if (string.IsNullOrEmpty(strTableName) || dicFieldNameType.Count < 1 || dtSourceData.Rows.Count < 1) return -1;
                foreach (DataRow drSourceData in dtSourceData.Rows)
                {
                    List<string> listByteDataFieldName = new List<string>();
                    int intByteDataNumber = 0;
                    string sqlInsertInto = string.Format("INSERT INTO {0} ", strTableName);
                    string sqlFieldName = string.Format("(");
                    string sqlFieldValue = string.Format("(");
                    foreach (var varFieldNameType in dicFieldNameType)
                    {
                        //目标(数据库)字段名和字段类型
                        string strKei = varFieldNameType.Key;
                        string strValue = varFieldNameType.Value.ToUpper();
                        //源(数据库)字段名,取数据用这个字段名取
                        string strSourceFieldName = varFieldNameType.Key;
                        if (dicCorrespondenceBetween != null)
                        {
                            strSourceFieldName = dicCorrespondenceBetween[strKei].ToString();
                        }
                        if (string.IsNullOrEmpty(strValue) || !dtSourceData.Columns.Contains(strSourceFieldName) || drSourceData.IsNull(strSourceFieldName))
                        {
                            continue;
                        }
                        if (strValue.IndexOf("BINARY") > -1 || strValue.IndexOf("IMAGE") > -1)
                        {
                            listByteDataFieldName.Add(strSourceFieldName);
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", "@file_" + intByteDataNumber++);
                        }
                        //拼写SQL,如果是数值型单独处理(不加'')
                        else if (strValue.IndexOf("INT") > -1 || strValue.IndexOf("FLOAT") > -1 || strValue.IndexOf("SMALLMONEY") > -1 || strValue.IndexOf("MONEY") > -1 || strValue.IndexOf("DECIMAL") > -1 || strValue.IndexOf("NUMERIC") > -1 || strValue.IndexOf("REAL") > -1)
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("{0},", string.IsNullOrEmpty(drSourceData[strSourceFieldName].ToString()) ? "null" : drSourceData[strSourceFieldName].ToString());
                        }
                        else
                        {
                            sqlFieldName += string.Format("{0},", strKei);
                            sqlFieldValue += string.Format("'{0}',", drSourceData[strSourceFieldName].ToString().Replace("\'", "\'\'"));
                        }
                    }
                    sqlFieldName = sqlFieldName.Substring(0, sqlFieldName.Length - 1);
                    sqlFieldName += string.Format(")");
                    sqlFieldValue = sqlFieldValue.Substring(0, sqlFieldValue.Length - 1);
                    sqlFieldValue += string.Format(")");
                    sqlInsertInto += string.Format("{0} VALUES {1}", sqlFieldName, sqlFieldValue);
                    try
                    {
                        //带有二进制字段的数据每条数据保存一次
                        OracleCommand Command = new OracleCommand(sqlInsertInto, Connection);
                        foreach (string strByteDataFieldName in listByteDataFieldName)
                        {
                            int intByteNumber = 0;
                            byte[] byteFile = (byte[])drSourceData[strByteDataFieldName];
                            string strSqlFileName = string.Format("{0}", "file_" + intByteNumber++);
                            Command.Parameters.Add(strSqlFileName, OracleType.Blob, byteFile.Length);
                            Command.Parameters[strSqlFileName].Value = byteFile;
                        }
                        if (Transaction != null)
                        {
                            Command.Transaction = Transaction;
                        }
                        if (Command.ExecuteNonQuery() > 0)
                        {
                            intNumberIfAffectedRows++;
                        }
                    }
                    catch (Exception ex)
                    {
                        TXTHelper.Logs(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                TXTHelper.Logs(ex.ToString());
                return -1;
            }
            return intNumberIfAffectedRows;
        }
    }
}
