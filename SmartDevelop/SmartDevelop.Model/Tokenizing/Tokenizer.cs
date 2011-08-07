using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDevelop.Model.Tokenizing
{

    /// <summary>
    /// Threadsave Tokenizer base class
    /// </summary>
    public abstract class Tokenizer
    {
        #region Events

        /// <summary>
        /// Raised when the Tokenizer has finished
        /// </summary>
        public event EventHandler FinishedSucessfully;

        #endregion

        #region Public Abstract Methods

        /// <summary>
        /// Starts Tokenizing async
        /// </summary>
        public virtual async Task TokenizeAsync() { }

        /// <summary>
        /// Starts Tokenizing sync
        /// </summary>
        public abstract void TokenizeSync();

        /// <summary>
        /// Get imutalble List of code segments
        /// </summary>
        /// <returns></returns>
        public abstract TokenizerSnapshot GetSegmentsSnapshot();

        public abstract void WaitTillCompleted();

        #endregion

        /// <summary>
        /// Indicates that the Tokenizer is currently Busy
        /// </summary>
        public abstract bool IsBusy { get; protected set; }

        protected virtual void OnFinishedSucessfully() {
            if(FinishedSucessfully != null)
                FinishedSucessfully(this, EventArgs.Empty);
        }
    }
}
