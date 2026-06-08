namespace OrphanHousingService.Models.Helpers
{
    public static class SystemEntityIds
    {
        public static readonly Guid SpecialHousingFund =
            Guid.Parse("00000000-0000-0000-0000-000000000001");

        public static readonly Guid SocialRent =
            Guid.Parse("00000000-0000-0000-0000-000000000002");

        public static bool IsSystemEntity(Guid personId) =>
            personId == SpecialHousingFund || personId == SocialRent;

        public static bool IsCitizen(Guid personId) => !IsSystemEntity(personId);
    }
}
