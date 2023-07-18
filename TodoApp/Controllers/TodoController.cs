using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Models.DTO;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoDbContext dbContext;

        public TodoController(TodoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var tasks = await dbContext.TodoItems.ToListAsync();

            //map domain model to dto
            var todoDto=new List<TodoDto>();
            foreach (var todoItem in tasks)
            {
                todoDto.Add(new TodoDto()
                {
                    Id = todoItem.Id,
                    Title = todoItem.Title,
                    Description = todoItem.Description,
                    IsCompleted = todoItem.IsCompleted,
                 
                });
            }
            return Ok(todoDto);
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            //get todo domain model from database
            var todoItem = await dbContext.TodoItems.SingleOrDefaultAsync(x => x.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            //map todo domain to dto
            var todoDto = new TodoItem
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                IsCompleted = todoItem.IsCompleted,

            };
         // Return Dto to client
            return Ok(todoDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddTodoRequestDto addTodoRequestDto)
        {
            //map or convert dto to model
            var todoModel = new TodoItem
            {
                Title = addTodoRequestDto.Title,
                Description = addTodoRequestDto.Description,
                IsCompleted = addTodoRequestDto.IsCompleted,
            };

            //use  model to create region
           await dbContext.TodoItems.AddAsync(todoModel);
           await dbContext.SaveChangesAsync();

            //map  model back to dto
            var todoDto = new TodoDto
            {
                Id=todoModel.Id,
                Title = todoModel.Title,
                Description = todoModel.Description,
                IsCompleted = todoModel.IsCompleted,
            };
            return CreatedAtAction(nameof(GetById),new {id=todoDto.Id},todoDto);

        }

        //Update
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTodoRequestDto updateTodoRequestDto)
        {
            var regionTodo=await dbContext.TodoItems.FirstOrDefaultAsync(x => x.Id==id);
            if (regionTodo == null)
            {
                return NotFound();
            }
            //Map Dto to domain model
            regionTodo.Title = updateTodoRequestDto.Title;
            regionTodo.Description = updateTodoRequestDto.Description;
            regionTodo.IsCompleted = updateTodoRequestDto.IsCompleted;

            await dbContext.SaveChangesAsync();

            //convert model to dto
            var todoDto = new TodoDto
            {
                Id=regionTodo.Id,
                Title=regionTodo.Title,
                Description = regionTodo.Description,
                IsCompleted = regionTodo.IsCompleted,
            };
            return Ok(todoDto);
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var existingTodo=await dbContext.TodoItems.FirstOrDefaultAsync(x=>x.Id==id);
            if(existingTodo == null)
            {
                return NotFound();
            }
             dbContext.TodoItems.Remove(existingTodo);
            dbContext.SaveChanges();
            var todoDto = new TodoDto
            {
                Id = existingTodo.Id,
                Title = existingTodo.Title,
                Description = existingTodo.Description,
                IsCompleted = existingTodo.IsCompleted,
            };
            return Ok(todoDto);
        }
    }
}
