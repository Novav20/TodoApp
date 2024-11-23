using TodoApp.Shared.Models;

namespace TodoApp.Web.Models
{
    public class TodoItemViewModel : TodoItem
    {
        public TodoItemViewModel() { }

        public TodoItemViewModel(TodoItem todoItem)
        {
            Id = todoItem.Id;
            Title = todoItem.Title;
            Description = todoItem.Description;
            IsCompleted = todoItem.IsCompleted;
            CreatedAt = todoItem.CreatedAt;
            CompletedAt = todoItem.CompletedAt;
        }
    }
}
