using System;
using System.Configuration;

namespace CSharpRecipes.Config
{
    /// <summary>
    /// Configuration elements for the Merge Articles
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class ChapterConfigurationElementCollection :
        ConfigurationElementCollection
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ChapterConfigurationElementCollection ()
        {
        }

        /// <summary>
        /// Override to provide hierarchical assignment of elements
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

        /// <summary>
        /// Create a typed version of the configuration element
        /// </summary>
        /// <returns>a ChapterConfigurationElement</returns>
        protected override ConfigurationElement CreateNewElement() => new ChapterConfigurationElement();

        /// <summary>
        /// Returns the key value for the element which is Number for the
        /// ChapterConfigurationElement
        /// </summary>
        /// <param name="element">the element to get the key for</param>
        /// <returns>the Number for the element</returns>
        protected override object GetElementKey(ConfigurationElement element) => ((ChapterConfigurationElement)element).Number;

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
        /// <returns>the ChapterConfigurationElement at the index</returns>
        public ChapterConfigurationElement this[int index]
        {
            get { return (ChapterConfigurationElement)BaseGet(index); }
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
        public new ChapterConfigurationElement this[string number] => (ChapterConfigurationElement)BaseGet(number);

        /// <summary>
        /// returns the index of the given element
        /// </summary>
        /// <param name="chapter">the chapter to find</param>
        /// <returns></returns>
        public int IndexOf(ChapterConfigurationElement chapter) => BaseIndexOf(chapter);

        /// <summary>
        /// Adds a new chapter to configuration
        /// </summary>
        /// <param name="chapter">the chapter info to add</param>
        public void Add(ChapterConfigurationElement chapter)
        {
            BaseAdd(chapter);
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
        /// Allows for the removal of a specific chapter configuration
        /// </summary>
        /// <param name="chapter"></param>
        public void Remove(ChapterConfigurationElement chapter)
        {
            if (chapter == null)
                throw new ArgumentNullException(nameof(chapter));
            if (BaseIndexOf(chapter) >= 0)
                BaseRemove(chapter.Number);
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
        /// <param name="number">number of the item to remove</param>
        public void Remove(string number)
        {
            BaseRemove(number);
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
    /// Holds the information about a chapter in the configuration file
    /// </summary>
    public class ChapterConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ChapterConfigurationElement()
        {
        }

        /// <summary>
        /// The number of the Chapter
        /// </summary>
        [ConfigurationProperty("Number", IsRequired=true)]
        public string Number
        {
            get { return (string)this["Number"]; }
            set { this["Number"] = value; }
        }

        /// <summary>
        /// The title of the Chapter
        /// </summary>
        [ConfigurationProperty("Title", IsRequired=true)]
        public string Title
        {
            get { return (string)this["Title"]; }
            set { this["Title"] = value; }
        }
    }
}
