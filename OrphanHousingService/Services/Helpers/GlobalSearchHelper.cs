using System;
using System.Collections.Generic;
using System.Linq;

namespace OrphanHousingService.Services.Helpers
{
    public static class GlobalSearchHelper
    {
        public static bool Matches(string? searchText, params string?[] fields)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return true;

            var term = searchText.Trim().ToLowerInvariant();

            return fields.Any(field =>
                !string.IsNullOrWhiteSpace(field) &&
                field.ToLowerInvariant().Contains(term));
        }

        public static bool Matches(string? searchText, IEnumerable<string?> fields)
        {
            return Matches(searchText, fields.ToArray());
        }
    }
}
