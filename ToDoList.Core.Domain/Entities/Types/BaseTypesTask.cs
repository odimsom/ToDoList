using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Domain.Entities.Types
{
    public abstract class BaseTypesTask : TaskItem
    {
        public abstract string GetTaskType();
    }

    public class LimpiezaGeneralTask : BaseTypesTask
    {
        public override string GetTaskType() => "Limpieza General";
    }

    public class LavarPlatosTask : BaseTypesTask
    {
        public override string GetTaskType() => "Lavar Platos";
    }

    public class BarrerYTrapearTask : BaseTypesTask
    {
        public override string GetTaskType() => "Barrer y Trapear";
    }

    public class LavarRopaTask : BaseTypesTask
    {
        public override string GetTaskType() => "Lavar Ropa";
    }

    public class DoblarYGuardarRopaTask : BaseTypesTask
    {
        public override string GetTaskType() => "Doblar y Guardar Ropa";
    }

    public class CocinarTask : BaseTypesTask
    {
        public override string GetTaskType() => "Cocinar";
    }

    public class HacerLaCompraTask : BaseTypesTask
    {
        public override string GetTaskType() => "Hacer la Compra";
    }

    public class OrganizarDespensaTask : BaseTypesTask
    {
        public override string GetTaskType() => "Organizar la Despensa";
    }

    public class SacarBasuraTask : BaseTypesTask
    {
        public override string GetTaskType() => "Sacar la Basura";
    }

    public class RegarPlantasTask : BaseTypesTask
    {
        public override string GetTaskType() => "Regar las Plantas";
    }

    public class LimpiarBanoTask : BaseTypesTask
    {
        public override string GetTaskType() => "Limpiar el Baño";
    }

    public class LimpiarCocinaTask : BaseTypesTask
    {
        public override string GetTaskType() => "Limpiar la Cocina";
    }

    public class LimpiarVentanasTask : BaseTypesTask
    {
        public override string GetTaskType() => "Limpiar las Ventanas";
    }

    public class AspirarAlfombrasTask : BaseTypesTask
    {
        public override string GetTaskType() => "Aspirar Alfombras";
    }

    public class CuidarMascotasTask : BaseTypesTask
    {
        public override string GetTaskType() => "Cuidar Mascotas";
    }

    public class CuidarNinosTask : BaseTypesTask
    {
        public override string GetTaskType() => "Cuidar Niños";
    }

    public class ReparacionesBasicasTask : BaseTypesTask
    {
        public override string GetTaskType() => "Reparaciones Básicas";
    }

    public class PagarServiciosTask : BaseTypesTask
    {
        public override string GetTaskType() => "Pagar Servicios";
    }

    public class OrganizarHabitacionesTask : BaseTypesTask
    {
        public override string GetTaskType() => "Organizar Habitaciones";
    }

    public class DesempolvarMueblesTask : BaseTypesTask
    {
        public override string GetTaskType() => "Desempolvar Muebles";
    }
}
