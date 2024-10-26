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

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? RoleTitle { get; set; }

        public string? Achievements { get; set; }
    }
}
