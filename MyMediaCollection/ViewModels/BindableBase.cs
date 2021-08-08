using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml.Data;

#nullable enable
namespace MyMediaCollection.ViewModels
{
    public class BindableBase : INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler? PropertyChanged;

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
        protected bool SetProperty<T>(ref T originalValue, T newValue, [CallerMemberName] string? propertyName = null)
        {
            bool result = false;
            if (!Equals(originalValue, newValue))
            {
                originalValue = newValue;
                OnPropertyChanged(propertyName);
                result = true;
            }

            return result;
        }
    }
}
