using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml.Data;

using MyMediaCollection.Interfaces;

#nullable enable
namespace MyMediaCollection.ViewModels
{
    public class BindableBase : INotifyPropertyChanged, INotifyDataErrorInfo, IValidatable
    {
        public virtual event PropertyChangedEventHandler? PropertyChanged;

        protected INavigationService? navigationService;
        protected IDataService? dataService;
        private readonly Dictionary<string, List<ValidationResult>> _errors = new();

        public bool HasErrors => _errors.Any();

        /// <summary>
        /// Signal that the property has changed and validate the value against the property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value to validate.</param>
        private void OnPropertyChanged(string? propertyName, object? value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Validate(propertyName, value);
        }

        /// <summary>
        /// Signal that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property. If null, the calling property name is used.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// If the new value is different, set the property to the new value and signal that it changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="originalValue">The original value of the property (it may be updated).</param>
        /// <param name="newValue">The new value of the property. </param>
        /// <param name="propertyName">The name of the property. If null, the calling property name is used.</param>
        /// <returns>True if the value was changed; false otherwise.</returns>
        protected bool SetProperty<T>(ref T? originalValue, T? newValue, [CallerMemberName] string? propertyName = null)
        {
            bool result = false;
            if (!Equals(originalValue, newValue))
            {
                originalValue = newValue;
                OnPropertyChanged(propertyName, newValue);
                result = true;
            }

            return result;
        }

        /// <inheritdoc/>
        public void Validate(string? memberName, object? value)
        {
            ClearErrors(memberName);
            List<ValidationResult> results = new();
            bool result = Validator.TryValidateProperty(value, new ValidationContext(this, null, null) { MemberName = memberName }, results);
            if (!result)
            {
                AddErrors(memberName, results);
            }
        }

        private void AddErrors(string? propertyName, List<ValidationResult> results)
        {
            Debug.Assert(propertyName is not null);
            if (!_errors.TryGetValue(propertyName!, out List<ValidationResult> errors))
            {
                errors = new List<ValidationResult>();
                _errors.Add(propertyName!, errors);
            }

            errors.AddRange(results);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearErrors(string? propertyName)
        {
            Debug.Assert(propertyName is not null);
            if (_errors.TryGetValue(propertyName!, out List<ValidationResult> errors))
            {
                errors.Clear();
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Get the errors associated with the property name.
        /// </summary>
        /// <param name="propertyName">The property name to get the errors for.</param>
        /// <returns>The list of errors for the property name.</returns>
        public IEnumerable<object> GetErrors(string propertyName) => _errors[propertyName];
    }
}