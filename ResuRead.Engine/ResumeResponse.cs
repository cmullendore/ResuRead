using System;

namespace ResuRead.Engine
{
    /// <summary>
    /// The structured response to be returned from the agent which can then
    /// be used for direct integration into target systems or serialized into
    /// structured JSON or XML.
    /// </summary>
    public class ResumeResponse
    {
        public string? CandidateName { get; set; }

        public string? ContactPhone { get; set; }

        public string? ContactEmail { get; set; }

        public List<WorkHistoryItem> WorkHistory { get; set; } = new();

        public DateTime SubmittedOn = DateTime.UtcNow;

        public List<string> Skills { get; set; } = new();
    }

    public class WorkHistoryItem
    {
        public string? CompanyName { get; set; }

        // Would prefer StartDate and EndDate to be DateOnly fields but
        // if someone uses a string such as "Present" or "Current" for end date,
        // the conversion will fail. Using string allows these values.
        // It will be up to the receiver to convert these to true numeric Date
        // objects and handle the non-date values.
        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? RoleTitle { get; set; }

        public List<string>? Achievements { get; set; }
    }
}
