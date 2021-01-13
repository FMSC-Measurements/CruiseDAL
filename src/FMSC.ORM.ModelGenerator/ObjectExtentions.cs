using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public static class ObjectExtentions
    {
        public static TReturn Try<TTarget, TReturn>(this TTarget target, Func<TTarget, TReturn> @try, Func<TTarget, Exception, TReturn> @catch)
        {
            try
            {
                return @try(target);
            }
            catch (Exception e)
            {
                return @catch(target, e);
            }
        }
    }
}
