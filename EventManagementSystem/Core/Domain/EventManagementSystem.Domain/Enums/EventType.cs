namespace EventManagementSystem.Domain.Enums
{
    /// <summary>
    /// Provides string constants for supported event types.
    /// </summary>
    public static class EventType
    {
        public const string Conference = "Conference";
        public const string Workshop = "Workshop";
        public const string Seminar = "Seminar";
        public const string Webinar = "Webinar";
        public const string Meetup = "Meetup";
        public const string Birthday = "Birthday";
        public const string Party = "Party";
        public const string Homecoming = "Homecoming";
        public const string Reunion = "Reunion";
        public const string Social = "Social";
        public const string Festival = "Festival";
        public const string Ceremony = "Ceremony";

        /// <summary>
        /// List of all supported event types.
        /// </summary>
        public static readonly List<string> Types = new List<string>
        {
            Conference,
            Workshop,
            Seminar,
            Webinar,
            Meetup,
            Birthday,
            Party,
            Homecoming,
            Reunion,
            Social,
            Festival,
            Ceremony
        };
    }
}
