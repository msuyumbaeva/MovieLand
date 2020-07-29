using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.BLL
{
    // Class describing operation result
    public class OperationDetails<T>
    {
        #region Properties
        public bool IsSuccess { get; private set; }

        public T Entity { get; }

        public List<string> Errors { get; private set; }
        #endregion Properties

        #region Private constructors
        private OperationDetails(T entity) {
            Entity = entity;
            Errors = new List<string>();
        }

        private OperationDetails() {
            Errors = new List<string>();
        }
        #endregion Private constructors

        #region Static methods
        // Creates OperationDetails with success result
        public static OperationDetails<T> Success(T entity) {
            var result = new OperationDetails<T>(entity);
            result.IsSuccess = true;
            return result;
        }

        // Creates OperationDetails with failure result
        public static OperationDetails<T> Failure() {
            var result = new OperationDetails<T>();
            result.IsSuccess = false;
            result.Errors = new List<string>();

            return result;
        }
        #endregion

        // Adds errors to list
        public OperationDetails<T> AddError(params string[] errorMessages) {
            if (Errors == null) {
                Errors = new List<string>();
            }
            Errors.AddRange(errorMessages);

            return this;
        }
    }
}
