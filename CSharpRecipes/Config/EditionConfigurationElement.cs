using System;
using System.Configuration;

namespace CSharpRecipes.Config
{
    /// <summary>
    /// Configuration elements for the Editions
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class EditionConfigurationElementCollection :
        ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EditionConfigurationElementCollection()
        {
        }

        /// <summary>
        /// Override to provide hierarchical assignment of elements
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

        /// <summary>
        /// Create a typed version of the configuration element
        /// </summary>
        /// <returns>a EditionConfigurationElement</returns>
        protected override ConfigurationElement CreateNewElement() => new EditionConfigurationElement();

        /// <summary>
        /// Returns the key value for the element which is Number for the
        /// EditionConfigurationElement
        /// </summary>
        /// <param name="element">the element to get the key for</param>
        /// <returns>the Number for the element</returns>
        protected override object GetElementKey(ConfigurationElement element) => ((EditionConfigurationElement)element).Number;


        /// <summary>
        /// New property to allow customization of the add element names
        /// </summary>
        public new string AddElementName
        {
            get { return base.AddElementName; }
            set { base.AddElementName = value; }
        }

        /// <summary>
        /// New property to allow customization of the clear element names
        /// </summary>
        public new string ClearElementName
        {
            get { return base.ClearElementName; }
            set { base.AddElementName = value; }
        }

        /// <summary>
        /// New property to allow customization of the remove element names
        /// </summary>
        public new string RemoveElementName => base.RemoveElementName;

        /// <summary>
        /// New property to override how count is achieved
        /// </summary>
        public new int Count => base.Count;


        /// <summary>
        /// Direct indexing method
        /// </summary>
        /// <param name="index">index to retrieve</param>
        /// <returns>the EditionConfigurationElement at the index</returns>
        public EditionConfigurationElement this[int index]
        {
            get { return (EditionConfigurationElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Indexer with a string key
        /// </summary>
        /// <param name="number">number of the element to get</param>
        /// <returns>the element that matches the number passed in as the key</returns>
        public new EditionConfigurationElement this[string number] => (EditionConfigurationElement)BaseGet(number);

        /// <summary>
        /// returns the index of the given element
        /// </summary>
        /// <param name="edition">the edition to find</param>
        /// <returns></returns>
        public int IndexOf(EditionConfigurationElement edition) => BaseIndexOf(edition);

        /// <summary>
        /// Adds a new edition to configuration
        /// </summary>
        /// <param name="edition">the edition info to add</param>
        public void Add(EditionConfigurationElement edition)
        {
            BaseAdd(edition);
        }

        /// <summary>
        /// Private function to add the element to the config
        /// </summary>
        /// <param name="element"></param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        /// <summary>
        /// Allows for the removal of a specific merge article configuration
        /// </summary>
        /// <param name="mergeFilter"></param>
        public void Remove(EditionConfigurationElement edition)
        {
            if (edition == null)
                throw new ArgumentNullException(nameof(edition));
            if (BaseIndexOf(edition) >= 0)
                BaseRemove(edition.Number);
        }

        /// <summary>
        /// Allows removal by index
        /// </summary>
        /// <param name="index">index of item to remove</param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }


        /// <summary>
        /// Allows removal by name
        /// </summary>
        /// <param name="name">name of the item to remove</param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        /// <summary>
        /// Clears the configuration set
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }
    }


    /// <summary>
    /// Holds the information about an edition in the configuration file
    /// </summary>
    public class EditionConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EditionConfigurationElement()
        {
        }

        /// <summary>
        /// Number for the edition
        /// </summary>
        [ConfigurationProperty("Number", IsRequired=true)]
        public string Number
        {
            get { return (string)this["Number"]; }
            set { this["Number"] = value; }
        }

        /// <summary>
        /// Name of the filter
        /// </summary>
        [ConfigurationProperty("PublicationYear", IsRequired = true)]
        public int PublicationYear
        {
            get { return (int)this["PublicationYear"]; }
            set { this["PublicationYear"] = value; }
        }

    }
}
