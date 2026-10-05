using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio_4
{
    public class ValidatorTexto : IValidador
    {
        public string MensajeError { get; private set; } = string.Empty;

        public bool EsValido(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                MensajeError = "El campo de texto no puede estar vacío";
                return false;
            }//fin de IsNullOrWhiteSpace

            return true;
        }//fin del metodo EsValido
    }
}