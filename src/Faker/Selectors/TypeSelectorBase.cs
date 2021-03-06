using System;
using System.Reflection;

namespace Faker.Selectors
{
    //Base class used to signify to the TypeTable that this is the option of last resort for any given type
    public abstract class PrimitiveSelectorBase<T> : TypeSelectorBase<T>
    {
    }

    /// <summary>
    ///     Abstract base class used to enforce some constraints on how we manage TypeSelectors
    /// </summary>
    /// <typeparam name="T">The type that this selector works for</typeparam>
    public abstract class TypeSelectorBase<T> : ITypeSelector
    {
        protected bool _can_be_null;

        protected Func<T> _setter;

        protected TypeSelectorBase()
        {
            //Set the targetType to the value of the type selector
            TargetType = typeof (T);
            Priority = SelectorPriorityConstants.PrimitiveSelectorPriority;
            _setter = Generate;
        }

        public virtual Func<T> Setter
        {
            get { return _setter; }
            set { _setter = value; }
        }

        public int Priority { get; set; }

        public void BeNull(bool canBeNull = false)
        {
            _can_be_null = canBeNull;
        }

        public virtual bool CanBind(PropertyInfo field)
        {
            return CanBind(field.PropertyType);
        }

        public virtual bool CanBind(Type type)
        {
            return type.IsAssignableFrom(TargetType);
        }

        public void Generate(object targetObject, PropertyInfo property)
        {
            property.SetValue(targetObject, Setter(), null);
        }

        public object Generate(ref object targetObject)
        {
            targetObject = Setter();
            return targetObject;
        }

        public object GenerateInstance()
        {
            return Generate();
        }

        public Type TargetType { get; }

        public abstract T Generate();

        protected bool Equals(TypeSelectorBase<T> other)
        {
            return Priority == other.Priority && TargetType == other.TargetType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeSelectorBase<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (Priority*397) ^ TargetType.GetHashCode();
                hash = (hash*397) ^ GetType().GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(TypeSelectorBase<T> left, TypeSelectorBase<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeSelectorBase<T> left, TypeSelectorBase<T> right)
        {
            return !Equals(left, right);
        }
    }
}