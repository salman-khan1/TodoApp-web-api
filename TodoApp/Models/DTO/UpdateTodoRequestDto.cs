﻿namespace TodoApp.Models.DTO
{
    public class UpdateTodoRequestDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
