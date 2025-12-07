using SmyloxFirstUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Helpers
{
    public class ViewModelRouter
    {
        private readonly IDictionary<string, INavigationService> _routes;

        public ViewModelRouter(IDictionary<string, INavigationService> routes)
        {
            _routes = routes;
        }

        public void NavigateTo(string routeKey, params object[] parameters)
        {
            if (_routes.ContainsKey(routeKey))
            {
                _routes[routeKey].Navigate(parameters);
            }
            else
            {
                throw new ArgumentException($"No navigation service found for route key: {routeKey}");
            }
        }

        public async Task AsyncNavigateTo(string routeKey, params object[] parameters)
        {
            if (_routes.ContainsKey(routeKey))
            {
                await _routes[routeKey].AsyncNavigation(parameters);
            }
            else
            {
                throw new ArgumentException($"No navigation service found for route key: {routeKey}");
            }
        }
    }
}
