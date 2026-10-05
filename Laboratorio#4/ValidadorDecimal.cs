using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio_4
{
    public class ValidadorDecimal : IValidador
    {
        public string MensajeError { get; private set; } = string.Empty;

        public bool EsValido(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor) || !decimal.TryParse(valor, out decimal resultado) || resultado < 0)
            {
                MensajeError = "Debe ingresar un numero decimal valido";
                return false;
            }//fin de IsWhiteNullOrSpace

            return true;
        }//fin del metodo EsValido
    }
}
