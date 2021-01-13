using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;

namespace FMSC.ORM.Core
{
    public class QueryEnumerator<TResult> : IEnumerator<TResult> where TResult : new()
    {
        private TResult _current = default(TResult);
        public TResult Current => _current;

        object IEnumerator.Current => _current;

        private DbDataReader Reader { get; }
        private EntityInflator Inflator { get; }
        private EntityDescription Description { get; }
        private IExceptionProcessor ExceptionProcessor { get; }

        public QueryEnumerator(DbDataReader reader, IExceptionProcessor exceptionProcessor)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Inflator = InflatorLookup.Instance.GetEntityInflator(reader);
            Description = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(TResult));
        }

        public void Dispose()
        {
            Reader.Dispose();
        }

        public bool MoveNext()
        {
            var reader = Reader;
            var inflator = Inflator;
            if (reader.Read())
            {
                TResult newDO = new TResult();
                if (newDO is ISupportInitializeFromDatastore)
                {
                    throw new NotSupportedException();
                }
                if (newDO is ISupportInitialize)
                {
                    ((ISupportInitialize)newDO).BeginInit();// allow dataobject to suspend property changed notifications or whatever
                }
                try
                {
                    inflator.ReadData(reader, newDO, Description);
                }
                catch (Exception e)
                {
                    var exceptionProcessor = ExceptionProcessor;
                    if (exceptionProcessor != null)
                    {
                        throw exceptionProcessor.ProcessException(e, null, null, null);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (newDO is ISupportInitialize)
                    {
                        ((ISupportInitialize)newDO).EndInit();
                    }
                }

                _current = newDO;
                return true;
            }
            else return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}
