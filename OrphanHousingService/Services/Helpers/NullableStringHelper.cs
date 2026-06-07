namespace OrphanHousingService.Services.Helpers
{
    public static class NullableStringHelper
    {
        public static string? Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
