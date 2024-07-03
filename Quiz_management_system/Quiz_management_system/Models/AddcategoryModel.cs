
namespace Quiz_management_system.Models
{
    public class AddcategoryModel
    {
        public int category_id { get; set; }
        public string category_name { get; set; }
        public List<AddcategoryModel> Categories { get; set; }

    }
}
