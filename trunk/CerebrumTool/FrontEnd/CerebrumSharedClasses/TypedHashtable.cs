/******************************************************************** 
 * Dictionary.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: Implementation of a Hashtable class with type-based keys and objects, rather than generic objects and key values.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Added set{} to TypedHashPair object, allowing for change of value directly in the pair rather than via parent Hashtable.
 * >> (18 Oct 2010) Matthew Cotter: Added ContainsValue() method in the spirit of standard Hashtable.
 * >> ( 7 Oct 2010) Matthew Cotter: Added set{} to index operator accessor.
 * >> (17 Sep 2010) Matthew Cotter: Added Count property and ContainsKey() methods in the spirit of standard Hashtable.
 * >> (13 Sep 2010) Matthew Cotter: Basic implementation, mimicing critical Hashtable methods wrapping underlying Hashtable object.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Object representing a value pair
    /// </summary>
    /// <typeparam name="AType">The type of the first value of the pair</typeparam>
    /// <typeparam name="BType">The type of the second value of the pair</typeparam>
    public class TypedPair<AType, BType>
    {
        private AType _A;
        private BType _B;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TypedPair()
        {
        }

        /// <summary>
        /// Get or set the key of the pair.
        /// </summary>
        public AType A
        {
            get
            {
                return _A;
            }
            set
            {
                _A = value;
            }
        }
        /// <summary>
        /// Get or set the value of the pair.
        /// </summary>
        public BType B
        {
            get
            {
                return _B;
            }
            set
            {
                _B = value;
            }
        }
    }
    /// <summary>
    /// Object representing a Key/Value pair of hashtable
    /// </summary>
    /// <typeparam name="KeyType">The type of the key value of the pair</typeparam>
    /// <typeparam name="ObjType">The type of the data value of the pair</typeparam>
    public class TypedHashPair<KeyType, ObjType>
    {
        private KeyType _Key;
        private ObjType _Value;

        /// <summary>
        /// Create-constructor.  Initializes a HashPair object with the specified key and value
        /// </summary>
        /// <param name="key">The key value of the pair</param>
        /// <param name="value">The data value of the pair</param>
        public TypedHashPair(KeyType key, ObjType value)
        {
            _Key = key;
            _Value = value;
        }

        /// <summary>
        /// Get the key of the key/value pair
        /// </summary>
        public KeyType Key
        {
            get
            {
                return _Key;
            }
        }
        /// <summary>
        /// Get the value of the key/value pair
        /// </summary>
        public ObjType Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
    }
    /// <summary>
    /// Defines a typed-specified hashtable object, based on the generic hashtable object type.
    /// </summary>
    /// <typeparam name="KeyType">The type of the key value of each pair</typeparam>
    /// <typeparam name="ObjType">The type of the data value of each pair</typeparam>
    public class TypedHashtable<KeyType, ObjType> : ICollection
    {
        private Hashtable _Hash;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TypedHashtable()
        {
            _Hash = new Hashtable();
        }

        /// <summary>
        /// Removes all items from the hashtable
        /// </summary>
        public void Clear()
        {
            _Hash.Clear();
        }
        /// <summary>
        /// Indicates whether the specified key is contained within the hashtable
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <returns>True if an entry with the specified key exists.  False otherwise</returns>
        public bool ContainsKey(KeyType key)
        {
            return _Hash.ContainsKey(key);
        }
        /// <summary>
        /// Indicates whether the specified value is contained within the hashtable
        /// </summary>
        /// <param name="value">The value to search for</param>
        /// <returns>True if an entry with the specified value exists.  False otherwise</returns>
        public bool ContainsValue(ObjType value)
        {
            return this.Values.Contains(value);
        }
        /// <summary>
        /// Adds a new entry to the hashtable with the specified key and value, if the key does not already exist in the hashtable
        /// </summary>
        /// <param name="key">The key of the new entry</param>
        /// <param name="value">The value of the new entry</param>
        public void Add(KeyType key, ObjType value)
        {
            if (!this._Hash.ContainsKey(key))
            {
                _Hash.Add(key, value);
            }
            else
            {
                throw new ArgumentException(String.Format("The specified key, {0}, already exists.", key.ToString()));
            }
        }
        /// <summary>
        /// Adds a new entry to the hashtable with the specified key/value pair, if the key does not already exist in the hashtable
        /// </summary>
        /// <param name="pair">The new key/value pair to add to the hashtable</param>
        public void Add(TypedHashPair<KeyType, ObjType> pair)
        {
            Add(pair.Key, pair.Value);
        }
        /// <summary>
        /// Get or set the value of a key/value pair, referenced by it's key.  If the key does not exist, a new entry is added with the key and specified value.
        /// </summary>
        /// <param name="key">The key to be referenced</param>
        /// <returns>(Get) The value of the entry referenced by the key, if it exists.</returns>
        public ObjType this[KeyType key]
        {
            get
            {
                if (this._Hash.ContainsKey(key))
                {
                    return (ObjType)_Hash[key];
                }
                else
                {
                    throw new KeyNotFoundException(String.Format("Could not find the key specified: {0}.", key.ToString()));
                }
            }
            set
            {
                if (this._Hash.ContainsKey(key))
                {
                    _Hash[key] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }
        /// <summary>
        /// Removes the entry with the specified key if it exists
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        public void Remove(KeyType key)
        {
            if (this._Hash.ContainsKey(key))
            {
                _Hash.Remove(key);
            }
            else
            {
                throw new KeyNotFoundException(String.Format("Could not find the key specified: {0}.", key.ToString()));
            }
        }
        /// <summary>
        /// Get a list of all keys used in the hashtable
        /// </summary>
        public List<KeyType> Keys
        {
            get
            {
                List<KeyType> keySet = new List<KeyType>();
                foreach (object key in _Hash.Keys)
                {
                    keySet.Add((KeyType)key);
                }
                return keySet;
            }
        }
        /// <summary>
        /// Get a list of all values stored in the hashtable
        /// </summary>
        public List<ObjType> Values
        {
            get
            {
                List<ObjType> valSet = new List<ObjType>();
                foreach (object key in _Hash.Keys)
                {
                    valSet.Add((ObjType)(_Hash[key]));
                }
                return valSet;
            }
        }
        /// <summary>
        /// Get a list of all Key/Value pairs in the hashtable
        /// </summary>
        public List<TypedHashPair<KeyType, ObjType>> Pairs
        {
            get
            {
                List<TypedHashPair<KeyType, ObjType>> pairSet = new List<TypedHashPair<KeyType, ObjType>>();
                foreach (object key in _Hash.Keys)
                {
                    pairSet.Add(new TypedHashPair<KeyType, ObjType>((KeyType)key, (ObjType)(_Hash[key])));
                }
                return pairSet;
            }
        }

        #region ICollection Implementation

        /// <summary>
        /// Implementation of ICollection.Count.  Get the number of items in the hashtable.
        /// </summary>
        public int Count
        {
            get
            {
                return _Hash.Count;
            }
        }
        /// <summary>
        /// Implementation of ICollection.CopyTo to enable foreach enumerator of objects in the hashtable.  Copies values of the hashtable into the destination array, starting at index
        /// </summary>
        /// <param name="destArray">The destination array.</param>
        /// <param name="index">The starting index.</param>
        public void CopyTo(Array destArray, int index)
        {
            foreach (ObjType o in this.Values)
            {
                destArray.SetValue(o, index);
                index = index + 1;
            }
        }
        /// <summary>
        /// Implementation of ICollection.SyncRoot.   Returns a common object that can be used to lock synchronization around accesses to this object.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }
        /// <summary>
        /// Implementation of ICollection.IsSynchronized.  Indicates whether the collection was written and designed to be thread-safe.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Implementation of ICollection.IEnumerable.GetEnumerator.  Gets an IEnumerator object used to iterate through objects in this hashtable.
        /// </summary>
        /// <returns>An enumerator object used to iterate over the objects in the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }
        #endregion
    }
}
