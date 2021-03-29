using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.UserInterface.Style
{
    public struct AttributeValueChangedEventArgs
    {
        public object value { get; }

        public AttributeValueChangedEventArgs(object newValue)
        {
            this.value = newValue;
        }
    }

    public delegate void AttributeValueChangedDelegate(object sender, AttributeValueChangedEventArgs eventArgs);
    
    public interface INotifyAttributeValueChanged
    {
        /// <summary>
        ///     Called when an attribute's value changes.
        /// </summary>
        event AttributeValueChangedDelegate valueChanged;
    }
    
    public abstract class OverridableAttribute : ScriptableObject, INotifyAttributeValueChanged
    {
        [SerializeField]
        private string m_AttributeName;

        [NonSerialized]
        private bool m_IsOverridden;

        public string attributeName => m_AttributeName;

        /// <inheritdoc />
        public event AttributeValueChangedDelegate valueChanged;

        /// <summary>
        /// Is the value overriden?
        /// </summary>
        public bool isOverriden
        {
            get => m_IsOverridden;
            set
            {
                if (m_IsOverridden == value)
                    return;

                m_IsOverridden = value;
                InvokeValueChanged(this, new AttributeValueChangedEventArgs(Get()));
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            return ((OverridableAttribute<T>)this).value;
        }

        public void Set<T>(T value)
        {
            ((OverridableAttribute<T>)this).value = value;
        }

        public abstract object Get();

        public abstract void Override(object value);

        public abstract void OverrideFrom(OverridableAttribute attribute);

        public void Override<T>(T value)
        {
            ((OverridableAttribute<T>)this).Override<T>(value);
        }

        /// <summary>
        ///     Sets the attribute's value.
        /// </summary>
        /// <typeparam name="T">The attribute's internal value type.</typeparam>
        /// <param name="value">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="onChanged">Callback if the value has changed.</param>
        protected void Set<T>(ref T value, T newValue, Action onChanged = null)
        {
            if (value != null && EqualityComparer<T>.Default.Equals(value, newValue))
                return;

            value = newValue;
            valueChanged?.Invoke(this, new AttributeValueChangedEventArgs(value));
            onChanged?.Invoke();
        }

        protected void InvokeValueChanged(object sender, AttributeValueChangedEventArgs args)
        {
            valueChanged?.Invoke(this, args);
        }
    }

    public abstract class OverridableAttribute<T> : OverridableAttribute
    {
        [SerializeField]
        private T m_Value;

        [NonSerialized]
        private T m_OverrideValue;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T value
        {
            get => !isOverriden ? m_Value : m_OverrideValue;
            set  
            {
                if (isOverriden)
                    Set(ref m_OverrideValue, value);
                else
                    Set(ref m_Value, value);
            }
        }

        /// <summary>
        /// Implicit conversion between <see cref="OverridableAttribute{T}"/> and its value type.
        /// </summary>
        /// <param name="attribute">The attribute to implicitly cast</param>
        /// <returns>A value of type <typeparam name="T">.</typeparam></returns>
        public static implicit operator T(OverridableAttribute<T> attribute)
        {
            return attribute.m_Value;
        }

        /// <summary>
        /// Returns the value as on object.
        /// </summary>
        /// <returns></returns>
        public override object Get()
        {
            return !isOverriden ? m_Value : m_OverrideValue;
        }

        /// <summary>
        /// Overrides the value.
        /// </summary>
        /// <param name="value">The value override.</param>
        public void Override(T value)
        {
            T oldValue = isOverriden? m_Value : m_OverrideValue;
            isOverriden = true;
            m_OverrideValue = value;
            if (value != null && EqualityComparer<T>.Default.Equals(oldValue, value))
                return;

            InvokeValueChanged(this, new AttributeValueChangedEventArgs(value));
        }

        public override void Override(object value)
        {
            Type type = value.GetType();
            if (typeof(T) != type)
                throw new InvalidCastException();

            Override((T)value);
        }

        public override void OverrideFrom(OverridableAttribute attribute)
        {
            Override(((OverridableAttribute<T>)attribute).Get<T>());
        }
    }
}