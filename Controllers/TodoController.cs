using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Demo.Todo.WebApi.Controllers
{
    public class Todo
    {
        public string title { get; set; }
        public bool? completed { get; set; } = false;
        public string url { get; set; } = "";
        public int? order { get; set; } = 0;
    }


    public class TodoController : ApiController
    {
        private static readonly ConcurrentDictionary<string, Todo> todos = new ConcurrentDictionary<string, Todo>();

        public IEnumerable<Todo> Get()
        {
            return todos.Values.OrderBy(x => x.order);
        }

        public Todo Get(string id)
        {
            if (todos.TryGetValue(id, out var todo))
            {
                return todo;
            }

            return null;
        }

        public Todo Post(Todo t)
        {
            var todo = new Todo
            {
                title = t.title,
                completed = t.completed,
                order = t.order,
                url = Url.Link("DefaultApi", new { Id = t.title })
            };
            todos.TryAdd(t.title, todo);
            return todo;
        }

        public Todo Patch(string id, Todo patch)
        {
            var todo = Get(id);

            var changed = new Todo
            {
                completed = patch.completed ?? todo.completed,
                order = patch.order ?? todo.order,
                title = patch.title ?? todo.title
            };

            changed.url = Url.Link("DefaultApi", new {Id = changed.title});

            todos.TryRemove(todo.title, out _);
            todos.TryAdd(changed.title, changed);

            return changed;
        }

        public void Delete()
        {
            todos.Clear();
        }
    }
}
