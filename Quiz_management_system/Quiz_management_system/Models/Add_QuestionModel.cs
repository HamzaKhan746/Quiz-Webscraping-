namespace Quiz_management_system.Models
{
    public class Add_QuestionModel
    {
        public string paper_id { get; set; }
        public string u_id { get; set; }
        public int register_id { get; set; }
        public string question_id { get; set; }
        public int course_id { get; set; }
        public string course_name { get; set; }
        public string question { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
        public string option3 { get; set; }
        public string option4 { get; set; }
        public string answer { get; set; }
        public string your_answer { get; set; }

        public string user_answer { get; set; }
        public int score = 0;


    }
}
