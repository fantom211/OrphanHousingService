using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace OrphanHousingService.ViewModels.Helpers
{
    public class ListCollectionManager<T> where T : class
    {
        private readonly ObservableCollection<T> _source = [];
        private readonly Func<T, IEnumerable<string?>> _searchFieldProvider;
        private string? _searchText;

        public ListCollectionManager(Func<T, IEnumerable<string?>> searchFieldProvider)
        {
            _searchFieldProvider = searchFieldProvider;
            View = CollectionViewSource.GetDefaultView(_source);
            View.Filter = FilterItem;
        }

        public ICollectionView View { get; }

        public string? SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                View.Refresh();
            }
        }

        public void SetItems(IEnumerable<T> items)
        {
            _source.Clear();

            foreach (var item in items)
                _source.Add(item);

            ApplyDefaultSort();
            View.Refresh();
        }

        public T? RestoreSelection(Guid? selectedId, Func<T, Guid> idSelector)
        {
            if (!selectedId.HasValue)
                return default;

            foreach (var item in _source)
            {
                if (idSelector(item) == selectedId.Value)
                    return item;
            }

            return default;
        }

        public void ApplyDefaultSort()
        {
            View.SortDescriptions.Clear();

            if (typeof(IHasCreatedAt).IsAssignableFrom(typeof(T)))
            {
                View.SortDescriptions.Add(new SortDescription(
                    nameof(IHasCreatedAt.CreatedAt),
                    ListSortDirection.Descending));
            }
        }

        private bool FilterItem(object item)
        {
            if (string.IsNullOrWhiteSpace(_searchText))
                return true;

            return GlobalSearchHelper.Matches(_searchText, _searchFieldProvider((T)item));
        }
    }
}
