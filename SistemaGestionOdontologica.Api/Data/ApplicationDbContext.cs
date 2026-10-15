using Microsoft.EntityFrameworkCore;
using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Pacientes y profesionales.
        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Odontologo> Odontologos => Set<Odontologo>();

        // Agenda.
        public DbSet<Turno> Turnos => Set<Turno>();
        public DbSet<RecordatorioTurno> RecordatoriosTurno => Set<RecordatorioTurno>();

        // Historia clínica.
        public DbSet<HistorialClinico> HistorialesClinicos => Set<HistorialClinico>();
        public DbSet<Tratamiento> Tratamientos => Set<Tratamiento>();
        public DbSet<HistorialTratamiento> HistorialTratamientos => Set<HistorialTratamiento>();
        public DbSet<ArchivoClinico> ArchivosClinicos => Set<ArchivoClinico>();

        // Odontogramas.
        public DbSet<Odontograma> Odontogramas => Set<Odontograma>();
        public DbSet<OdontogramaDetalle> OdontogramaDetalles => Set<OdontogramaDetalle>();

        // Facturación y pagos.
        public DbSet<Factura> Facturas => Set<Factura>();
        public DbSet<FacturaDetalle> FacturaDetalles => Set<FacturaDetalle>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<PagoAplicacion> PagoAplicaciones => Set<PagoAplicacion>();

        // Usuarios y seguridad.
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // Para auditoria.
        public DbSet<Bitacora> Bitacora => Set<Bitacora>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
