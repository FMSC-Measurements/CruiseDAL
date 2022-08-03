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

#if !NET35_CF
    public class QueryEnumerator<TResult1, TResult2> : IEnumerator<Tuple<TResult1, TResult2>> 
        where TResult1 : new () 
        where TResult2 : new()
    {
        private Tuple<TResult1, TResult2> _current = default(Tuple<TResult1, TResult2>);
        public Tuple<TResult1, TResult2> Current => _current;

        object IEnumerator.Current => _current;

        private DbDataReader Reader { get; }
        private EntityInflator Inflator { get; }
        private EntityDescription[] Description { get; } = new EntityDescription[2];
        private IExceptionProcessor ExceptionProcessor { get; }

        public QueryEnumerator(DbDataReader reader, IExceptionProcessor exceptionProcessor)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Inflator = InflatorLookup.Instance.GetEntityInflator(reader);
            Description[0] = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(TResult1));
            Description[1] = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(TResult2));
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
                var result1 = new TResult1();
                var result2 = new TResult2();

                if (result1 is ISupportInitializeFromDatastore)
                {
                    throw new NotSupportedException();
                }
                if (result1 is ISupportInitialize)
                {
                    ((ISupportInitialize)result1).BeginInit();// allow dataobject to suspend property changed notifications or whatever
                }

                if (result2 is ISupportInitializeFromDatastore)
                {
                    throw new NotSupportedException();
                }
                if (result2 is ISupportInitialize)
                {
                    ((ISupportInitialize)result2).BeginInit();// allow dataobject to suspend property changed notifications or whatever
                }

                try
                {
                    inflator.ReadData(reader, result1, Description[0]);
                    inflator.ReadData(reader, result2, Description[1]);
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
                    if (result1 is ISupportInitialize)
                    {
                        ((ISupportInitialize)result1).EndInit();
                    }
                    if (result2 is ISupportInitialize)
                    {
                        ((ISupportInitialize)result2).EndInit();
                    }
                }

                _current = new Tuple<TResult1, TResult2>(result1, result2); ;
                return true;
            }
            else return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
#endif
}
