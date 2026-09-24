using MySql.Data.MySqlClient;
using Mysqlx.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorio_4
{
    public class Conexion
    {
        private static string cadenaConexion = "Server=localhost;Database=productosdb;Uid=root;Pwd=Em@xXD120105";
        public static MySqlConnection ObtenerConexion()
        {
            try
            {
                //Crear un timpo de dato de MySQLConnection
                MySqlConnection conexion = new MySqlConnection(cadenaConexion);
                conexion.Open();
                return (conexion);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error al conectar: " + ex.Message);
                return null;
            }//fin del catch
        }//fin del metodo estatico MySqlConnection

        public static List<Producto> GetProductos(string filtro)
        {
            List<Producto> listaProductos = new List<Producto>();
            string query = "SELECT id,nombre, precio, cantidad, imagen FROM productos";
            // Si viene un filtro, modifoetros, pero adaptado al ejemplo visual que tienes)
            if (!string.IsNullOrEmpty(filtro))
            {
                query += " WHERE id LIKE @filtro OR nombre LIKE @filtro OR precio LIKE @filtro OR cantidad LIKE @filtro";


            }
            using (MySqlConnection conn = ObtenerConexion())
            {
                if (conn == null)
                {
                    return listaProductos;
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    //si hay filtro , agregamos el parametro para evitar inyeccion sql
                    if (!string.IsNullOrEmpty(filtro)) {
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                    }

                    //Ejecutamos el lector de datos
                    using (MySqlDataReader mReader = cmd.ExecuteReader()) {
                        // Recorremos el lector fila por fila mientras haya registros
                        while (mReader.Read()) { 
                        Producto prod = new Producto();
                            //Mapeamos los campos de la base de datos a las propiedades de tu clase Producto
                            //(Ajusta los nombres de las columnas o indices segun tu base de datos) 
                            prod.Id = Convert.ToInt32(mReader["id"]);
                            prod.Nombre = mReader["nombre"].ToString();
                            prod.Precio = Convert.ToDecimal(mReader["precio"]);
                            prod.Cantidad = Convert.ToInt32(mReader["cantidad"]);
                            //prod.Imagen = (byte[])mReader.GetCalue(4);
                            prod.Imagen = mReader["imagen"] != DBNull.Value ? (byte[])mReader["imagen"] : null;
                            // Agregamos el objeto listo a la lista generica
                            listaProductos.Add(prod);
                        
                        }//MySqlDaTA rREADASD
                    
                    }
                }
            }

            return listaProductos;
        } //fin del metodo
        public static bool UpdateSeguro(string tbName, Dictionary<string, object> data, string idColumn, int idValue)
        {
            var setParts = new List<string>();
            foreach (var key in data.Keys)
            {
                setParts.Add($"{key} = @{key}");

            }
            string setClause = string.Join(", ", setParts);
            //armamos la consulta sql completa incluyendo el where
            string sql = $"UPDATE {tbName} SET {setClause} WHERE {idColumn} = @idCondicion";
            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;
                    using (MySqlCommand smt = new MySqlCommand(sql, conexion))
                    {
                        foreach(var kvp in data)
                        {
                            smt.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value); 
                         
                        }
                        smt.Parameters.AddWithValue("@idCondicion", idValue);
                        smt.ExecuteNonQuery();
                        return true;
                    }
                }
                    
            }catch(MySqlException ex)
            {
                Console.WriteLine("Error en UPDATE: " + ex.Message);
                return false;
            }
        }
        public static bool DeleteSeguro(string tbName, string idColumn, int idValue)
        {
            string sql = $"DELETE FROM {tbName} WHERE {idColumn} = @idCondicion";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand smt = new MySqlCommand(sql, conexion))
                    {
                        smt.Parameters.AddWithValue("@idCondicion", idValue);

                        smt.ExecuteNonQuery();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
                return false;
            }
        }
        public static bool InsertSeguro(string tbName, Dictionary<string, object> data)
        {
            var columns = string.Join(", ", data.Keys);
            var placeholders = "@" + string.Join(", @", data.Keys);
            string sql = $"INSERT INTO {tbName} ({columns}) VALUES ({placeholders})";
            try
            {
                //Pedimos la conexion usando nuestra clase externa
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {

                        foreach (var kvp in data)
                        {
                            stmt.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        stmt.ExecuteNonQuery();
                        return true;
                    }


                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error en Insert: " + ex.Message);
                return false;
            }
        }//fin del metodo insertar seguro
    } //FIN DEL METODO  
}
