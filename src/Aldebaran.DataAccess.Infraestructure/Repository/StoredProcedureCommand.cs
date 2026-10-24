using Microsoft.Data.SqlClient;
using System.Data;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    /// <summary>
    /// Arma la llamada a un SP con parámetros nombrados (<see cref="SqlParameter"/>), nunca con valores concatenados.
    /// Los parámetros sin valor (null o texto vacío) no se envían, de modo que el SP aplica su valor por defecto,
    /// igual que cuando el filtro no se incluía en la llamada.
    /// </summary>
    public sealed class StoredProcedureCommand
    {
        private readonly string _procedure;
        private readonly List<SqlParameter> _parameters = new();

        public StoredProcedureCommand(string procedure)
        {
            _procedure = string.IsNullOrWhiteSpace(procedure) ? throw new ArgumentNullException(nameof(procedure)) : procedure;
        }

        public SqlParameter[] Parameters => _parameters.ToArray();

        public string Sql => _parameters.Count == 0
            ? $"EXEC {_procedure}"
            : $"EXEC {_procedure} " + string.Join(", ", _parameters.Select(p => $"{p.ParameterName} = {p.ParameterName}"));

        public StoredProcedureCommand Int(string name, int? value) => Add(name, SqlDbType.Int, value);

        public StoredProcedureCommand Bit(string name, bool? value) => Add(name, SqlDbType.Bit, value);

        public StoredProcedureCommand Date(string name, DateTime? value) => Add(name, SqlDbType.Date, value?.Date);

        public StoredProcedureCommand DateTime(string name, DateTime? value) => Add(name, SqlDbType.DateTime, value);

        public StoredProcedureCommand VarChar(string name, string? value, int size = -1) =>
            string.IsNullOrWhiteSpace(value) ? this : Add(name, SqlDbType.VarChar, value.Trim(), size);

        public StoredProcedureCommand NVarChar(string name, string? value, int size = -1) =>
            string.IsNullOrWhiteSpace(value) ? this : Add(name, SqlDbType.NVarChar, value.Trim(), size);

        private StoredProcedureCommand Add(string name, SqlDbType type, object? value, int size = 0)
        {
            if (value == null)
                return this;

            var parameter = new SqlParameter(name, type) { Value = value };
            if (size != 0)
                parameter.Size = size;
            _parameters.Add(parameter);
            return this;
        }
    }
}
