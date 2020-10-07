using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace FMSC.ORM.Core
{
    public class QueryResult<TResult> : IEnumerable<TResult> where TResult : new()
    {
        public DbCommand Command { get; }
        public DbConnection Connection { get; }
        public DbTransaction Transaction { get; }
        public IExceptionProcessor ExceptionProcessor { get; }

        public QueryResult(DbConnection connection, DbCommand command, DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Transaction = transaction;
            ExceptionProcessor = exceptionProcessor;
        }

        //public QueryResult(DbConnection connection, string commandText, IEnumerable<KeyValuePair<string, object>> parameters, DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        //{
        //}

        //public QueryResult(DbConnection connection, string commandText, object[] parameters = null, DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        //{
        //}

        public IEnumerator<TResult> GetEnumerator()
        {
            var exceptionProcessor = ExceptionProcessor;
            var reader = Connection.ExecuteReader(Command, Transaction, exceptionProcessor);
            return new QueryEnumerator<TResult>(reader, exceptionProcessor);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}