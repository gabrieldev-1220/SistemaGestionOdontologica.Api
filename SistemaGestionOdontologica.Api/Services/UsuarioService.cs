using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaGestionOdontologica.Api.Data;
using SistemaGestionOdontologica.Api.DTOs.Usuarios;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Helpers;
using SistemaGestionOdontologica.Api.Interfaces;
using SistemaGestionOdontologica.Api.Security;

namespace SistemaGestionOdontologica.Api.Services
{
    public sealed class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IAuditoriaService _auditoria;

        public UsuarioService(ApplicationDbContext context, IPasswordHasher<Usuario> passwordHasher, IAuditoriaService auditoria)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _auditoria = auditoria;
        }

        public async Task<UsuarioRespuestaDto> CrearAsync(
            CrearUsuarioDto request,
            CancellationToken cancellationToken = default)
        {
            string username = request.Username?.Trim() ?? string.Empty;
            string rol = request.Rol?.Trim().ToLowerInvariant()
                ?? string.Empty;

            if (username.Length < 3 || username.Length > 50)
            {
                throw new ValidacionNegocioException(
                    "El nombre de usuario debe tener entre 3 y 50 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(request.Password) ||
                request.Password.Length < 12)
            {
                throw new ValidacionNegocioException(
                    "La contraseña debe tener al menos 12 caracteres.");
            }

            if (rol != Roles.Admin &&
                rol != Roles.Odontologo &&
                rol != Roles.Asistente)
            {
                throw new ValidacionNegocioException(
                    "El rol indicado no es válido.");
            }

            if (rol == Roles.Odontologo)
            {
                if (!request.IdOdontologo.HasValue ||
                    request.IdOdontologo.Value <= 0)
                {
                    throw new ValidacionNegocioException(
                        "Debe indicar un odontólogo válido para este rol.");
                }
            }
            else if (request.IdOdontologo.HasValue)
            {
                throw new ValidacionNegocioException(
                    "Solo los usuarios con rol odontólogo pueden tener un profesional asociado.");
            }

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

            try
            {
                bool usernameExiste = await _context.Usuarios
                    .AnyAsync(
                        u => u.Username == username,
                        cancellationToken);

                if (usernameExiste)
                {
                    throw new ConflictoNegocioException(
                        "El nombre de usuario ya se encuentra registrado.");
                }

                if (rol == Roles.Odontologo)
                {
                    int idOdontologo = request.IdOdontologo!.Value;

                    bool odontologoActivo = await _context.Odontologos
                        .AnyAsync(
                            o => o.IdOdontologo == idOdontologo &&
                                 o.Activo,
                            cancellationToken);

                    if (!odontologoActivo)
                    {
                        throw new ValidacionNegocioException(
                            "El odontólogo no existe o está inactivo.");
                    }

                    bool asociacionExiste = await _context.Usuarios
                        .AnyAsync(
                            u => u.IdOdontologo == idOdontologo,
                            cancellationToken);

                    if (asociacionExiste)
                    {
                        throw new ConflictoNegocioException(
                            "El odontólogo ya tiene una cuenta de usuario asociada.");
                    }
                }

                var usuario = new Usuario
                {
                    Username = username,
                    Rol = rol,
                    IdOdontologo = request.IdOdontologo,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };

                usuario.PasswordHash =
                    _passwordHasher.HashPassword(
                        usuario,
                        request.Password);

                _context.Usuarios.Add(usuario);

                await _context.SaveChangesAsync(cancellationToken);

                _auditoria.Registrar(
                    "USUARIO_CREADO",
                    "Usuario",
                    usuario.IdUsuario);

                await _context.SaveChangesAsync(cancellationToken);

                await transaccion.CommitAsync(cancellationToken);

                return new UsuarioRespuestaDto
                {
                    IdUsuario = usuario.IdUsuario,
                    Username = usuario.Username,
                    Rol = usuario.Rol,
                    IdOdontologo = usuario.IdOdontologo,
                    Activo = usuario.Activo,
                    FechaCreacion = usuario.FechaCreacion
                };
            }
            catch (DbUpdateException ex)
                when (EsViolacionUnicidad(ex))
            {
                await transaccion.RollbackAsync(
                    CancellationToken.None);

                throw new ConflictoNegocioException(
                    "El nombre de usuario o el odontólogo asociado ya se encuentra registrado.");
            }
            catch
            {
                await transaccion.RollbackAsync(
                    CancellationToken.None);

                throw;
            }
        }

        private static bool EsViolacionUnicidad(
            DbUpdateException exception)
        {
            return exception.InnerException is SqlException sqlException &&
                   (sqlException.Number == 2601 ||
                    sqlException.Number == 2627);
        }
    }
}
