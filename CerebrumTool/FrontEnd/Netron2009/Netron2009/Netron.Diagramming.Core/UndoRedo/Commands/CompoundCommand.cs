using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Implementation of ICommand for multiple actions.  Made public in support of overriding Connection Tool class (Matthew Cotter 2/1/2011)
    /// </summary>
    public class CompoundCommand : Command, ICompoundCommand
    {
        #region Fields
        private CollectionBase<ICommand> commands;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the commands in this compound action.
        /// </summary>
        /// <value>The commands.</value>
        public CollectionBase<ICommand> Commands
        {
            get
            {
                return commands;
            }
            set
            {
                commands = value;
            }
        }

        #endregion
        
        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public CompoundCommand(IController controller) : base(controller)
        {
            commands = new CollectionBase<ICommand>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Perform redo of this command.
        /// </summary>
        public override void Redo()
        {
            foreach(ICommand cmd in commands)
            {
                cmd.Redo();
            }
        }

        /// <summary>
        /// Perform undo of this command.
        /// </summary>
        public override void Undo()
        {
            foreach(ICommand cmd in commands)
            {
                cmd.Undo();
            }
        }
        #endregion

    }
}
