using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.TokenizerBase
{

    /// <summary>
    /// Threadsave Tokenizer base class
    /// </summary>
    public abstract class Tokenizer
    {
        #region Events

        public event EventHandler Finished;

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
        /// Get imutalble List of segments
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<CodeSegment> GetSegmentsSnapshot();

        #endregion

        public abstract bool IsBusy { get; }


        protected virtual void OnFinished() {
            if(Finished != null)
                Finished(this, EventArgs.Empty);
        }
    }
}
