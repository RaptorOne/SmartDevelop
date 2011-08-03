using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using Archimedes.Patterns;

namespace SmartDevelop.Model
{
    /// <summary>
    /// Represents the IDE, which means the top level object
    /// </summary>
    public class IDE : Singleton<IDE>
    {
        SmartSolution _currentSolution;

        public event EventHandler<EventArgs<SmartSolution>> CurrentSolutionChanged;

        /// <summary>
        /// Gets/Sets the current Solution
        /// </summary>
        public SmartSolution CurrentSolution {
            get { return _currentSolution; }
            set { 
                _currentSolution = value;
                OnCurrentSolutionChanged(value);
            }
        }

        protected void OnCurrentSolutionChanged(SmartSolution newSolution) {
            if(CurrentSolutionChanged != null)
                CurrentSolutionChanged(this, new EventArgs<SmartSolution>(newSolution));
        }

        /// <summary>
        /// Get the current solution
        /// </summary>
        /// <returns></returns>
        public static SmartSolution GetSolution() {
               return IDE.Instance.CurrentSolution;
        }
    }
}
