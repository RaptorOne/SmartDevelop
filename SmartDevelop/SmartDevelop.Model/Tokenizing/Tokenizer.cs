using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public abstract void TokenizeAsync();

        /// <summary>
        /// Starts Tokenizing sync
        /// </summary>
        public abstract void TokenizeSync();

        /// <summary>
        /// Get imutalble List of code segments
        /// </summary>
        /// <returns></returns>
        public abstract TokenizerSnapshot GetSegmentsSnapshot();

        #endregion

        /// <summary>
        /// Indicates that the Tokenizer is currently Busy
        /// </summary>
        public abstract bool IsBusy { get; }

        protected virtual void OnFinishedSucessfully() {
            if(FinishedSucessfully != null)
                FinishedSucessfully(this, EventArgs.Empty);
        }
    }
}
