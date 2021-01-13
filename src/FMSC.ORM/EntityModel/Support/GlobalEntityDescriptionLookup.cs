using System;
using System.Collections.Generic;

namespace FMSC.ORM.EntityModel.Support
{
    public interface IEntityDescriptionLookup
    {
        EntityDescription LookUpEntityByType(Type type);

        //EntityInflator GetEntityInflator(Type type);
    }

    public class GlobalEntityDescriptionLookup : IEntityDescriptionLookup
    {
        protected Dictionary<Type, EntityDescription> _entityDescriptionLookup = new Dictionary<Type, EntityDescription>();
        protected Dictionary<Type, EntityInflator> _entityInflatorLookup = new Dictionary<Type, EntityInflator>();

        protected static GlobalEntityDescriptionLookup _instance;

        public static GlobalEntityDescriptionLookup Instance
        {
            get
            {
                return _instance ?? (_instance = new GlobalEntityDescriptionLookup());
            }
            set { _instance = value; }
        }

        private GlobalEntityDescriptionLookup()
        {
        }

        public EntityDescription LookUpEntityByType(Type type)
        {
            lock (_entityDescriptionLookup)
            {
                if (!_entityDescriptionLookup.ContainsKey(type))
                {
                    var ed = new EntityDescription(type);
                    _entityDescriptionLookup.Add(type, ed);
                    return ed;
                }

                return _entityDescriptionLookup[type];
            }
        }

        //public EntityInflator GetEntityInflator(Type type)
        //{
        //    lock (_entityDescriptionLookup)
        //    {
        //        if (!_entityInflatorLookup.ContainsKey(type))
        //        {
        //            var entityDescription = LookUpEntityByType(type);
        //            var entityInflator = new EntityInflator(entityDescription);
        //            _entityInflatorLookup.Add(type, entityInflator);
        //            return entityInflator;
        //        }
        //        else
        //        {
        //            return _entityInflatorLookup[type];
        //        }
        //    }
        //}
    }
}