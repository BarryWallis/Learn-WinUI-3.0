using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WebViewBrowser.Bus.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Assign the new value to the original value and signal the property has changed.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="originalValue">The original value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property that is being updated. The calling prooerty name is automatically provided, if omitted.</param>
        /// <returns><see langword="true"/> if the original value and the new Value are different; otherwise, <see langword="false"/>.</returns>
        protected bool SetProperty<T>(ref T originalValue, T newValue, [CallerMemberName] string? propertyName = null)
        {
            bool result = false;
            if (!EqualityComparer<T>.Default.Equals(originalValue, newValue))
            {
                originalValue = newValue;
                OnPropertyChanged(propertyName/*, newValue*/); // TODO: Does the second argument need to be added?
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Signal that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name pf the property that was changed. The calling prooerty name is automatically provided, if omitted.</param>
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
