using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio_4
{
    public interface IValidador
    {
        bool EsValido(string valor);
        string MensajeError { get; }
    }
    

}
