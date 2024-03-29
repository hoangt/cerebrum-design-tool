using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml.Schema;
namespace Netron.Diagramming.Core
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Complementary partial class related to (de)serialization.
    /// </summary>
    // ----------------------------------------------------------------------
    [Serializable]
    public partial class DiagramEntityBase :
        ISerializable,
        IXmlSerializable,
        IDeserializationCallback
    {
        #region Deserialization constructor

        // ------------------------------------------------------------------
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        // ------------------------------------------------------------------
        protected DiagramEntityBase(
            SerializationInfo info,
            StreamingContext context)
        {
            if (Tracing.BinaryDeserializationSwitch.Enabled)
            {
                Trace.WriteLine(
                    "Deserializing the fields of 'DiagramEntityBase'.");
            }
            Initialize();

            string version = info.GetString("DiagramEntityBaseVersion");
            this.mName = info.GetString("Name");
            this.mSceneIndex = info.GetInt32("SceneIndex");
            this.mUid = new Guid(info.GetString("Uid"));
            this.mResizable = info.GetBoolean("Resizable");

            this.mPaintStyle = info.GetValue(
                "PaintStyle",
                typeof(IPaintStyle)) as IPaintStyle;

            this.mPenStyle = info.GetValue(
                "PenStyle",
                typeof(IPenStyle)) as IPenStyle;

            mAllowDelete = (bool)info.GetValue("AllowDelete", typeof(bool));
            myMinSize = (Size)info.GetValue("MinSize", typeof(Size));
            myMaxSize = (Size)info.GetValue("MaxSize", typeof(Size));
            mVisible = (bool)info.GetValue("Visible", typeof(bool));
            mEnabled = (bool)info.GetValue("Enabled", typeof(bool));
        }
        #endregion

        #region Serialization events
        /*
        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            Trace.WriteLine("Starting to serializing the 'DiagramEntityBase' class...");
        }
        [OnSerialized]
        void OnSerialized(StreamingContext context)
        {
            Trace.WriteLine("...serialization of 'DiagramEntityBase' finished");
        }
        */
        #endregion

        #region Deserialization events
        /*
        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            Trace.Indent();
            Trace.WriteLine("Starting deserializing the 'DiagramEntityBase' class...");
        }
        */
        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("...deserialization of 'DiagramEntityBase' finished");
        }

        #endregion

        #region Serialization

        // ------------------------------------------------------------------
        /// <summary>
        /// Populates a <see cref=
        /// "T:System.Runtime.Serialization.SerializationInfo"></see> with 
        /// the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The 
        /// <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> 
        /// to populate with data.</param>
        /// <param name="context">The destination (see 
        /// <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) 
        /// for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller 
        /// does not have the required permission. </exception>
        // ------------------------------------------------------------------
        public virtual void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            if (Tracing.BinarySerializationSwitch.Enabled)
            {
                Trace.WriteLine("Serializing the fields of " +
                    "'DiagramEntityBase'.");
            }

            info.AddValue("DiagramEntityBaseVersion", diagramEntityBaseVersion);
            info.AddValue("Name", this.Name);
            info.AddValue("SceneIndex", this.SceneIndex);
            info.AddValue("Uid", this.Uid.ToString());
            info.AddValue("Resizable", this.mResizable);
            info.AddValue("PaintStyle", this.mPaintStyle, typeof(IPaintStyle));
            info.AddValue("PenStyle", this.mPenStyle, typeof(IPenStyle));
            info.AddValue("AllowDelete", mAllowDelete, typeof(bool));
            info.AddValue("MinSize", myMinSize, typeof(Size));
            info.AddValue("MaxSize", myMaxSize, typeof(Size));
            info.AddValue("Visible", mVisible, typeof(bool));
            info.AddValue("Enabled", mEnabled, typeof(bool));
        }
        #endregion

        #region Xml serialization
        /// <summary>
        /// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </returns>
        public virtual XmlSchema GetSchema()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion

        #region IDeserializationCallback Members

        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
        public virtual void OnDeserialization(object sender)
        {
            if (Tracing.BinaryDeserializationSwitch.Enabled)
                Trace.WriteLine("IDeserializationCallback of 'DiagramEntityBase' called.");
            UpdatePaintingMaterial();
        }

        #endregion
    }
}
