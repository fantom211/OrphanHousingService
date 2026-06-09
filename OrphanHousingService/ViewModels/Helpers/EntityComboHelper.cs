using System;
using System.Collections.ObjectModel;

namespace OrphanHousingService.ViewModels.Helpers
{
    public static class EntityComboHelper
    {
        public static T? FindById<T>(ObservableCollection<T> items, Guid id) where T : class
        {
            foreach (var item in items)
            {
                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty?.GetValue(item) is Guid itemId && itemId == id)
                    return item;
            }

            return null;
        }
    }
}
