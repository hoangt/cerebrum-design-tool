using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace GraphicsLibrary
{
    [Serializable()]
    public class RendererCollection : CollectionBase
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public RendererCollection()
            : base()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds the specified painter to the list.
        /// </summary>
        /// <param name="painter"></param>
        /// <returns>int</returns>
        // ------------------------------------------------------------------
        public int Add(IRenderer2D painter)
        {
            return this.List.Add(painter);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Removes the specified painter from the list.
        /// </summary>
        /// <param name="painter">IPainter</param>
        /// <returns>bool: True if successful, false if not.</returns>
        // ------------------------------------------------------------------
        public bool Remove(IRenderer2D painter)
        {
            foreach (IRenderer2D ip in this.List)
            {
                if (ip == painter)
                {
                    base.List.Remove(ip);
                    return true;
                }
            }
            return false;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns the painter at the specified index.  If  the index is
        /// out of bounds then 'null' is returned.
        /// </summary>
        /// <param name="index">int</param>
        /// <returns>IPainter</returns>
        // ------------------------------------------------------------------
        public IRenderer2D Get(int index)
        {
            if ( (index >= 0) && (index < this.List.Count) )
            {
                return (IRenderer2D)this.List[index];
            }
            else
            {
                return null;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the painter at the specified index.
        /// </summary>
        /// <param name="index">int</param>
        /// <returns>IPainter</returns>
        // ------------------------------------------------------------------
        public IRenderer2D this[int index]
        {
            get
            {
                return this.Get(index);
            }
            set
            {
                this.List[index] = value;
            }
        }
    }
}
