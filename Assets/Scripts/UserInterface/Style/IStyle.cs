using System.Collections.Generic;

namespace Aci.Unity.UserInterface.Style
{
    public interface IStyle : IEnumerable<OverridableAttribute>
    {
        /// <summary>
        ///     Returns the number of attributes in the style
        /// </summary>
        int count { get; }

        /// <summary>
        ///     Gets an attribute from the style
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns></returns>
        T GetAttribute<T>(string attributeName) where T : OverridableAttribute;

        /// <summary>
        ///     Removes an attribute from the style
        /// </summary>
        /// <param name="attribute"></param>
        bool RemoveAttribute(OverridableAttribute attribute);

        /// <summary>
        ///     Adds an attribute to the style
        /// </summary>
        /// <param name="attribute"></param>
        void AddAttribute(OverridableAttribute attribute);

        /// <summary>
        ///     Overwrites an attribute's value.
        /// </summary>
        /// <typeparam name="T">The attribute value type</typeparam>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="attribute">The new Attribute value</param>
        void SetAttributeValue<T>(string attributeName, T attributeValue);

        /// <summary>
        ///     Tries to get an attribute by its name
        /// </summary>
        /// <typeparam name="T">The type of attribute</typeparam>
        /// <param name="attributeName">The attribute name</param>
        /// <param name="attribute">The outgoing attribute</param>
        /// <returns>Returns true if the attribute could be found</returns>
        bool TryGetAttribute<T>(string attributeName, out T attribute) where T : OverridableAttribute;

        /// <summary>
        ///     Returns the attribute at the given index
        /// </summary>
        /// <param name="index">The attribute index</param>
        /// <returns>Returns the attribute</returns>
        OverridableAttribute this[int index]
        {
            get;
        }

        /// <summary>
        ///     Gets an attribute by its attribute name
        /// </summary>
        /// <param name="name">The attribute name</param>
        /// <returns>Returns the attribute</returns>
        OverridableAttribute this[string name]
        {
            get;
        }
    }
}

