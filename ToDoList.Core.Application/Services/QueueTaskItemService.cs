﻿using System.Collections.Concurrent;
using System.Reactive.Subjects;
using ToDoList.Core.Application.Interfaces;
using ToDoList.Core.Domain.Entities;

namespace ToDoList.Core.Application.Services
{
    public class QueueTaskItemService : IQueueTaskItemService
    {
        private readonly ConcurrentQueue<(TaskItem, Func<Task>)> _queue = new();
        private readonly Subject<TaskItem> _newTask = new();
        private readonly object _lock = new();
        private bool _processing = false;

        public QueueTaskItemService()
        {
            _newTask.Subscribe(async _ =>
            {
                lock (_lock)
                {
                    if (_processing) return;
                    _processing = true;
                }

                await ProcessQueueAsync();
            });
        }

        public void AddTaskItem(TaskItem taskItem, Func<Task> action, CancellationToken cancellationToken)
        {
            _queue.Enqueue((taskItem, action));
            _newTask.OnNext(taskItem);
        }

        private async Task ProcessQueueAsync()
        {
            while (_queue.TryDequeue(out var item))
            {
                var (taskItem, action) = item;

                try
                {
                    Console.WriteLine($"Procesando: {taskItem.Description}");
                    await action();
                    Console.WriteLine($"Completado: {taskItem.Description}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando tarea: {taskItem.Description}. Detalle: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine($"Finalizado: {taskItem.Description}");
                }
            }

            lock (_lock)
            {
                _processing = false;
            }
        }
    }
}