namespace Quiz_management_system.Models
{
    public class view_questionModel
    {
        public string course_name { get; set; }
        public int question_id { get; set; }
        public int course_id  { get; set; }
        public string question { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
        public string option3 { get; set; }
        public string option4 { get; set; }
        public string answer { get; set; }
    }
}
