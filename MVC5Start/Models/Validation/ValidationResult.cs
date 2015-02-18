using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace MVC5Start.Models.Validation
{
    /// <summary>
    ///     Represents the result of a validation operation
    /// </summary>
    public class ValidationResult
    {
        private static readonly ValidationResult SuccessResult = new ValidationResult(true);

        /// <summary>
        ///     Failure constructor that takes error messages
        /// </summary>
        /// <param name="errors"></param>
        public ValidationResult(params string[] errors)
        {
            if (errors == null)
                throw new ArgumentNullException("errors");

            this.Succeeded = false;
            this.Errors = errors;
        }

        /// <summary>
        /// Constructor that takes whether the result is successful
        /// </summary>
        /// <param name="success"></param>
        protected ValidationResult(bool success)
        {
            this.Succeeded = success;
            this.Errors = new string[0];
        }

        /// <summary>
        ///     True if the validation was successful
        /// </summary>
        public bool Succeeded { get; private set; }

        /// <summary>
        ///     List of errors
        /// </summary>
        public string[] Errors { get; private set; }

        /// <summary>
        ///     Static success result
        /// </summary>
        /// <returns></returns>
        public static ValidationResult Success
        {
            get { return SuccessResult; }
        }

        /// <summary>
        ///     Failed helper method
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static ValidationResult Failed(params string[] errors)
        {
            return new ValidationResult(errors);
        }

        public IdentityResult IdentityResult
        {
            get
            {
                return this.Succeeded ? IdentityResult.Success : IdentityResult.Failed(this.Errors);
            }
        }
    }
}
