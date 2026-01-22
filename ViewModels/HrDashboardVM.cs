namespace RecruitmentSystem.ViewModels
{
    public class HrDashboardVM
    {
        public int TotalApplications { get; set; }

        public int AspNetDeveloperCount { get; set; }
        public int JavaDeveloperCount { get; set; }
        public int AiMlDeveloperCount { get; set; }

        public int AppliedCount { get; set; }
        public int SelectedCount { get; set; }
        public int RejectedCount { get; set; }
        public int ExamSentCount { get; set; }
        public int EmployeeCreatedCount { get; set; }
    }
}

