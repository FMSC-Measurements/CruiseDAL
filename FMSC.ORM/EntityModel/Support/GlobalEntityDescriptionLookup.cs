using System;
using System.Collections.Generic;

namespace FMSC.ORM.EntityModel.Support
{
    public interface IEntityDescriptionLookup
    {
        EntityDescription LookUpEntityByType(Type type);

        EntityInflator GetEntityInflator(Type type);
    }

    public class GlobalEntityDescriptionLookup : IEntityDescriptionLookup
    {
        protected Dictionary<string, EntityDescription> _entityDescriptionLookup = new Dictionary<string, EntityDescription>();
        protected Dictionary<string, EntityInflator> _entityInflatorLookup = new Dictionary<string, EntityInflator>();

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
            string name = type.Name;
            if (!_entityDescriptionLookup.ContainsKey(name))
            {
                var ed = new EntityDescription(type);
                _entityDescriptionLookup.Add(name, ed);
                return ed;
            }

            return _entityDescriptionLookup[type.Name];
        }

        public EntityInflator GetEntityInflator(Type type)
        {
            string name = type.Name;
            if (!_entityInflatorLookup.ContainsKey(name))
            {
                var entityDescription = LookUpEntityByType(type);
                var entityInflator = new EntityInflator(entityDescription);
                _entityInflatorLookup.Add(name, entityInflator);
                return entityInflator;
            }
            else
            {
                return _entityInflatorLookup[name];
            }
        }
    }
}