﻿using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Domain.Delegates
{
    public class ValidateTaskDelegate
    {
        public delegate bool ValidateTask(TaskItem taskItem);
        public Action<TaskItem>? OnTaskCreated;
        public Func<TaskItem, int> CalculateDaysRemaining = (task) =>
            (task.DueDate - DateTime.Now).Days;

        public IEnumerable<TaskItem> FilterTasksByStatus(IEnumerable<TaskItem> tasks, StatusTask status)
        {
            return tasks.Where(t => t.StatusTask == status);
        }

        public async Task<bool> ValidateTaskAsync(TaskItem task, Action<string>? notify = null)
        {
            ValidateTask validate = (TaskItem taskItem) =>
            {
                if (string.IsNullOrWhiteSpace(taskItem.Description))
                {
                    notify?.Invoke("La descripción no puede estar vacía.");
                    return false;
                }

                if (taskItem.DueDate <= DateTime.Now)
                {
                    notify?.Invoke("La fecha de vencimiento debe ser en el futuro.");
                    return false;
                }

                return true;
            };

            bool isValid = validate(task);

            if (isValid)
            {
                OnTaskCreated?.Invoke(task);
            }

            return await Task.FromResult(isValid);
        }
    }
}
