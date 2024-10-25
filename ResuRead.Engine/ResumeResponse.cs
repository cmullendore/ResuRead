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

        public List<WorkHistoryItem> WorkHistory { get; } = new List<WorkHistoryItem>();

        public DateTime SubmittedOn = DateTime.UtcNow;

        public List<string> Skills = new();
    }

    public class WorkHistoryItem
    {
        public string? CompanyName;

        public DateOnly? StartDate;

        public DateOnly? EndDate;

        public string? RoleTitle;

        public string? Achievements;
    }
}
