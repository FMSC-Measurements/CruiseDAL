using FMSC.ORM.EntityModel.Attributes;
using System;

namespace FMSC.ORM.EntityModel.Support
{
    public class FieldInfo
    {
        public FieldInfo()
        {
        }

        public FieldInfo(PropertyAccessor propertyAccessor) : this((FieldAttribute)null, propertyAccessor)
        {
        }

        public FieldInfo(FieldAttribute fieldAttribute, PropertyAccessor propertyAccessor)
        {
            Property = propertyAccessor ?? throw new ArgumentNullException("propertyAccessor");

            if (fieldAttribute != null)
            {
                var alias = fieldAttribute.Alias;
                if (string.IsNullOrEmpty(alias) == false)
                {
                    Alias = alias;
                    var expression = fieldAttribute.SQLExpression;
                    if (string.IsNullOrEmpty(expression))
                    { throw new InvalidOperationException("If Field Alias is set SQLExpression must be set too"); }

                    SQLExpression = expression;
                    PersistanceFlags = PersistanceFlags.Never;
                }
                else
                {
                    Name = fieldAttribute.Name ?? propertyAccessor.Name;
                    PersistanceFlags = fieldAttribute.PersistanceFlags;
                }

                if (fieldAttribute is PrimaryKeyFieldAttribute pkFieldAttr)
                {
                    IsKeyField = true;
                }

                if (fieldAttribute is InfrastructureFieldAttribute iAttr)
                {
                    DefaultValueProvider = iAttr.DefaultValueProvider;
                }
            }
            else
            {
                Name = propertyAccessor.Name;
            }
        }

        public string Name { get; set; }

        public string Alias { get; set; }
        public string SQLExpression { get; set; }
        public PersistanceFlags PersistanceFlags { get; set; } = PersistanceFlags.Always;

        public bool IsKeyField { get; set; }
        public bool IsAutoGenerate { get; set; }

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
            return GetFieldValue(obj) ?? DefaultValueProvider?.Invoke();
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
            var alias = Alias;
            if(alias != null)
            {
                return $"EntityFieldInfo RunTimeType({RunTimeType}), Alias({alias}), Expression({SQLExpression})";
            }
            else
            {
                return String.Format("EntityFieldInfo RunTimeType({0}), FieldName({1})",
                 RunTimeType, Name ?? "null");
            }
        }
    }
}