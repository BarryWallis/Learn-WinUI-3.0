using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using MyMediaCollection.Interfaces;

namespace MyMediaCollection.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IDictionary<string, Type> _pages = new ConcurrentDictionary<string, Type>();

        public string CurrentPage { get; }

        private static Frame AppFrame => (Frame)Window.Current.Content;



        /// <summary>
        /// Add a page and its associated type to .the dictionary.
        /// </summary>
        /// <param name="page">The page's name to add.</param>
        /// <param name="type">The page's type.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="page"/> name must be provided or
        /// <para>The <paramref name="page"/> has already been registered or</para>
        /// <para>The <paramref name="type"/> view has already been registered.</para>
        /// </exception>
        public void Configure(string page, Type type)
        {
            if (string.IsNullOrWhiteSpace(page))
            {
                throw new ArgumentException("A Page name must be provided", nameof(page));
            }
            if (_pages.ContainsKey(page))
            {
                throw new ArgumentException($"The page {page} has already been registered.", nameof(page));
            }
            if (_pages.Values.Any(t => t == type))
            {
                throw new ArgumentException($"The {type.Name} view has already been registered.", nameof(type));
            }

            _pages[page] = type;
        }

        /// <inheritdoc/>
        public void GoBack()
        {
            if (AppFrame.CanGoBack)
            {
                AppFrame.GoBack();
            }
        }

        /// <inheritdoc/>
        public void NavigateTo(string page) => NavigateTo(page, null);

        /// <inheritdoc/>
        public void NavigateTo(string page, object parameter)
        {
            if (!_pages.ContainsKey(page))
            {
                throw new ArgumentException($"Unable to fund a page registered with the name {page}.", nameof(page));
            }

            _ = AppFrame.Navigate(_pages[page], parameter);
        }
    }
}
