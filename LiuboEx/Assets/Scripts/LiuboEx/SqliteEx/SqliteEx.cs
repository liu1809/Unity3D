using UnityEngine;
using System;
using System.Collections;

using Mono.Data.Sqlite;

/// <summary>
/// <para> 功能 :
/// </para>
/// <para> 作者 : 
/// </para>
/// <para> 日期 : 
/// </para>
/// </summary>
public class SqliteEx
{
    /*-------------------------------------------------------------------------------------------------------------------------------*/
    #region Private Fields

    private SqliteConnection dbConnection;

    private SqliteCommand dbCommand;

    private SqliteDataReader dbReader;

    #endregion
    /*-------------------------------------------------------------------------------------------------------------------------------*/
    #region Public Fields


    #endregion
    /*-------------------------------------------------------------------------------------------------------------------------------*/

    /*-------------------------------------------------------------------------------------------------------------------------------*/
    #region Private Functions

    #endregion
    /*-------------------------------------------------------------------------------------------------------------------------------*/
    #region Public Functions

    public SqliteEx()
    {

    }

    public SqliteEx(string connectionString)
    {
        OpenDB(connectionString);
    }

    /// <summary>  
    /// 打开数据库  
    /// </summary>  
    /// <param name="connectionString">里面已加Data Source=</param>  
    public void OpenDB(string connectionString)
    {
        try
        {
            dbConnection = new SqliteConnection("Data Source=" + connectionString);
            
            dbConnection.Open();
        }
        catch (Exception e)
        {
            string temp1 = e.ToString();
            Debug.Log(temp1);
        }
    }

    /// <summary>  
    /// 关闭数据库 
    /// </summary>  
    public void CloseDB()
    {
        if (dbCommand != null)
        {
            dbCommand = null;
        }

        if (dbReader != null)
        {
            dbReader = null;
        }

        if (dbConnection != null)
        {
            dbConnection.Close();
            dbConnection = null;
        }

        Debug.Log("数据关闭成功");
    }

    public SqliteDataReader ExecuteQuery(string sqlQuery)
    {
        try
        {
            dbCommand = dbConnection.CreateCommand();

            dbCommand.CommandText = sqlQuery;

            dbReader = dbCommand.ExecuteReader();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return dbReader;

    }

    /// <summary>  
    /// 读取整个表  
    /// </summary>  
    /// <param name="tableName"></param>  
    /// <returns></returns>
    public SqliteDataReader ReadFullTable(string tableName)
    {

        string query = "SELECT * FROM " + tableName;

        return ExecuteQuery(query);

    }
    /// <summary>  
    /// 插入一条数据，表名，对应的条目值  
    /// </summary>  
    /// <param name="tableName"></param>  
    /// <param name="values"></param>  
    /// <returns></returns>  
    public SqliteDataReader InsertInto(string tableName, string[] values)
    {

        string query = "INSERT INTO " + tableName + " VALUES (" + values[0];

        for (int i = 1; i < values.Length; ++i)
        {

            query += ", " + values[i];

        }

        query += ")";

        return ExecuteQuery(query);

    }
    /// <summary>  
    /// 更新某条信息，表名，item name，value，key，value  
    /// </summary>  
    /// <param name="tableName"></param>  
    /// <param name="cols"></param>  
    /// <param name="colsvalues"></param>  
    /// <param name="selectkey"></param>  
    /// <param name="selectvalue"></param>  
    /// <returns></returns>  
    public SqliteDataReader UpdateInto(string tableName, string[] cols, string[] colsvalues, string selectkey, string selectvalue)
    {

        string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + colsvalues[0];

        for (int i = 1; i < colsvalues.Length; ++i)
        {

            query += ", " + cols[i] + " =" + colsvalues[i];
        }

        query += " WHERE " + selectkey + " = " + selectvalue + " ";

        return ExecuteQuery(query);
    }

    /// <summary>  
    /// DELETE FROM table WHERE key=value  
    /// </summary>  
    /// <param name="tableName"></param>  
    /// <param name="cols"></param>  
    /// <param name="colsvalues"></param>  
    /// <returns></returns>  
    public SqliteDataReader Delete(string tableName, string[] cols, string[] colsvalues)
    {
        string query = "DELETE FROM " + tableName + " WHERE " + cols[0] + " = " + colsvalues[0];

        for (int i = 1; i < colsvalues.Length; ++i)
        {

            query += " or " + cols[i] + " = " + colsvalues[i];
        }
        Debug.Log(query);
        return ExecuteQuery(query);
    }

    public SqliteDataReader InsertIntoSpecific(string tableName, string[] cols, string[] values)
    {
        if (cols.Length != values.Length)
        {
            throw new Exception("columns.Length != values.Length");  
        }

        string query = "INSERT INTO " + tableName + "(" + cols[0];

        for (int i = 1; i < cols.Length; ++i)
        {
            query += ", " + cols[i];
        }

        query += ") VALUES (" + values[0];

        for (int i = 1; i < values.Length; ++i)
        {
            query += ", " + values[i];
        }

        query += ")";

        return ExecuteQuery(query);
    }

    public SqliteDataReader DeleteContents(string tableName)
    {
        string query = "DELETE FROM " + tableName;
        return ExecuteQuery(query);
    }

    /// <summary>  
    /// 创建表  
    /// </summary>  
    /// <param name="name"></param>  
    /// <param name="col"></param>  
    /// <param name="colType"></param>  
    /// <returns></returns>  
    public SqliteDataReader CreateTable(string name, string[] col, string[] colType)
    {
        if (col.Length != colType.Length)
        {
            throw new Exception("columns.Length != colType.Length");  
        }

        string query = "CREATE TABLE " + name + " (" + col[0] + " " + colType[0];

        for (int i = 1; i < col.Length; ++i)
        {
            query += ", " + col[i] + " " + colType[i];
        }

        query += ")";

        return ExecuteQuery(query);
    }

    public SqliteDataReader SelectWhere(string tableName, string[] items, string[] col, string[] operation, string[] values)
    {
        if (col.Length != operation.Length || operation.Length != values.Length)
        {
            throw new Exception("col.Length != operation.Length != values.Length");  
        }
        string query = "SELECT " + items[0];

        for (int i = 1; i < items.Length; ++i)
        {
            query += ", " + items[i];
        }
        query += " FROM " + tableName + " WHERE " + col[0] + operation[0] + "'" + values[0] + "' ";

        for (int i = 1; i < col.Length; ++i)
        {
            query += " AND " + col[i] + operation[i] + "'" + values[i] + "' ";
        }
        return ExecuteQuery(query);
    }

    #endregion
    /*-------------------------------------------------------------------------------------------------------------------------------*/
}