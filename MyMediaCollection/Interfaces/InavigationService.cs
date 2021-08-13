using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMediaCollection.Interfaces
{
    public interface INavigationService
    {
        string CurrentPage { get; }

        /// <summary>
        /// Navigate to a specific page.
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
        void NavigateTo(string page);

        /// <summary>
        /// Navigte to a specific page and supply the given parameter.
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
        /// <param name="parameter">The parameter to supply.</param>
        void NavigateTo(string page, object parameter);

        /// <summary>
        /// Go back to the previous page.
        /// </summary>
        void GoBack();
    }
}
