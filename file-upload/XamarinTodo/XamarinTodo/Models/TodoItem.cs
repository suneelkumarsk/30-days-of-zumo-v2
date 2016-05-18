using XamarinTodo.Helpers;

namespace XamarinTodo.Models
{
    /// <summary>
    /// The TodoItem - this must match the TodoItem in the Backend/DataObjects directory
    /// precisely.  Note that the EntityData in the DTO and the EntityData here are actually
    /// different to account for the difference in the Deleted property.
    /// </summary>
    public class TodoItem : EntityData
    {
        public string Text { get; set; }
        public bool Complete { get; set; }
    }
}
