﻿using FMSC.ORM.EntityModel.Attributes;
using System;

namespace FMSC.ORM.EntityModel.Support
{
    public class FieldInfo
    {
        public FieldInfo()
        {
        }

        public FieldInfo(PropertyAccessor propertyAccessor)
        {
            Property = propertyAccessor ?? throw new ArgumentNullException("propertyAccessor");
        }

        public FieldInfo(FieldAttribute fieldAttribute, PropertyAccessor propertyAccessor) : this(propertyAccessor)
        {
            Name = fieldAttribute.Name;
            PersistanceFlags = fieldAttribute.PersistanceFlags;
            Alias = fieldAttribute.Alias;
            SQLExpression = fieldAttribute.SQLExpression;

            if (fieldAttribute is PrimaryKeyFieldAttribute)
            {
                IsKeyField = true;
                KeyType = ((PrimaryKeyFieldAttribute)fieldAttribute).KeyType;
            }

            if (fieldAttribute is InfrastructureFieldAttribute)
            {
                DefaultValueProvider = ((InfrastructureFieldAttribute)fieldAttribute).DefaultValueProvider;
            }
        }

        public string Name { get; set; }

        public string Alias { get; set; }
        public string SQLExpression { get; set; }
        public PersistanceFlags PersistanceFlags { get; set; } = PersistanceFlags.Always;

        public bool IsKeyField { get; set; }
        public KeyType KeyType { get; set; }

        public Func<object> DefaultValueProvider { get; set; }

        public PropertyAccessor Property { get; set; }
        public Type RunTimeType { get { return Property.RuntimeType; } }

        public object GetFieldValue(Object obj)
        {
            object value = Property.GetValue(obj);
            return value;
        }

        public object GetFieldValueOrDefault(Object obj)
        {
            var defaultValue = DefaultValueProvider?.Invoke();
            return GetFieldValue(obj) ?? defaultValue;
        }

        public void SetFieldValue(Object dataObject, object value)
        {
            try
            {
                Property.SetValue(dataObject, value);
            }
            catch (Exception e)
            {
                throw new ORMException(String.Format("unable to set value; Value = {0}; FieldInfo = {1}", value, this), e);
            }
        }

        public override string ToString()
        {
            return String.Format("EntityFieldInfo RunTimeType({0}), FieldName({1}), Expression({2})",
                 RunTimeType, Name ?? "null", SQLExpression ?? "null");
        }
    }
}