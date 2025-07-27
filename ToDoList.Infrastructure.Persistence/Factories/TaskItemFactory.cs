using ToDoList.Core.Application.Factories;
using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.Entities.Types;
using ToDoList.Core.Domain.Enums;

namespace ToDoList.Infrastructure.Persistence.Factories
{
    public class TaskItemFactory : ITaskItemFactory
    {
        public BaseTypesTask CreateTask(TaskItem taskItem)
        {
            BaseTypesTask task = taskItem.TaskType switch
            {
                TaskType.LimpiezaGeneral => new LimpiezaGeneralTask(),
                TaskType.LavarPlatos => new LavarPlatosTask(),
                TaskType.BarrerYTrapear => new BarrerYTrapearTask(),
                TaskType.LavarRopa => new LavarRopaTask(),
                TaskType.DoblarYGuardarRopa => new DoblarYGuardarRopaTask(),
                TaskType.Cocinar => new CocinarTask(),
                TaskType.HacerLaCompra => new HacerLaCompraTask(),
                TaskType.OrganizarDespensa => new OrganizarDespensaTask(),
                TaskType.SacarBasura => new SacarBasuraTask(),
                TaskType.RegarPlantas => new RegarPlantasTask(),
                TaskType.LimpiarBano => new LimpiarBanoTask(),
                TaskType.LimpiarCocina => new LimpiarCocinaTask(),
                TaskType.LimpiarVentanas => new LimpiarVentanasTask(),
                TaskType.AspirarAlfombras => new AspirarAlfombrasTask(),
                TaskType.CuidarMascotas => new CuidarMascotasTask(),
                TaskType.CuidarNinos => new CuidarNinosTask(),
                TaskType.ReparacionesBasicas => new ReparacionesBasicasTask(),
                TaskType.PagarServicios => new PagarServiciosTask(),
                TaskType.OrganizarHabitaciones => new OrganizarHabitacionesTask(),
                TaskType.DesempolvarMuebles => new DesempolvarMueblesTask(),
                _ => throw new NotSupportedException($"TaskType '{taskItem.TaskType}' not supported.")
            };

            task.Description = taskItem.Description;
            task.DueDate = taskItem.DueDate;
            task.StatusTask = StatusTask.PENDING;
            task.AdditionalData = taskItem.AdditionalData;
            task.TaskType = taskItem.TaskType;

            return task;
        }
    } 
}
