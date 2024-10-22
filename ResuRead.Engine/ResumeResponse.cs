using System;

namespace ResuRead.Engine
{
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
