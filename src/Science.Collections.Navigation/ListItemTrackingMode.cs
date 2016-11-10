namespace Science.Collections.Navigation
{
    internal enum ListItemTrackingMode
    {
        /// <summary>
        /// The point tracks toward the beginning of the list. An insertion at the current position leaves the point unaffected. If a replacement contains the point, it will end up at the beginning of the replacement range.
        /// </summary>
        Negative,

        /// <summary>
        /// The point tracks toward the end of the list. An insertion at the current position pushes the point to the end of the inserted range. If a replacement contains the point, it will end up at the end of the replacement range.
        /// </summary>
        Positive,
    }
}
