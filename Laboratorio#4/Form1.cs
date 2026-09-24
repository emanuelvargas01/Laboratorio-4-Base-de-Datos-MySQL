using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorio_4
{
    public partial class Form1 : Form
    {
        private List<Producto> listaProductos;
        private Dictionary<string, object> myProducto = new Dictionary<string, object>();





        public Form1()
        {
            InitializeComponent();
            listaProductos = new List<Producto>();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];
            txtFolio.Text = Convert.ToInt32(fila.Cells["Folio"].Value).ToString();
            txtNombre.Text = Convert.ToString(fila.Cells["Nombre"].Value);
            txtPrecio.Text = Convert.ToDecimal(fila.Cells["Precio"].Value).ToString();
            txtCantidad.Text = Convert.ToInt32(fila.Cells["Cantidad"].Value).ToString();

            btnAgregar.Enabled = false;
            btnModificar.Enabled = true;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Seleccionar imagen del producto";
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //carga la imagen seleccionada en el pictureBox y ajusta su tamaño
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                }




            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            DataGridViewImageColumn columnaImagen =
                (DataGridViewImageColumn)dgvProductos.Columns["Imagen"];

            columnaImagen.ImageLayout = DataGridViewImageCellLayout.Zoom;

            dgvProductos.Columns["Imagen"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;


            cargarProductos();
        }
        private void cargarProductos(string filtro = "")
        {
            dgvProductos.Rows.Clear();
            dgvProductos.Refresh();
            listaProductos = Conexion.GetProductos(filtro);
            foreach (var prod in listaProductos)
            {
                Image img = null;
                if (prod.Imagen != null && prod.Imagen.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(prod.Imagen))
                    {
                        using (Bitmap bmp = new Bitmap(ms))
                        {
                            img = new Bitmap(bmp); // esto clona la imagen y evita que falle

                        }


                    }

                }

                dgvProductos.Rows.Add(prod.Id, prod.Nombre, prod.Precio, prod.Cantidad, img);
            }//fin del foreach listaProductos
        }//Fin de cargarProductos

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            cargarProductos(txtBusqueda.Text.Trim());
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!datosCorrectos())
            {
                return; //No vamos a hacer nada-- Se detiene en este punto. puedes crear un punto de interrupcion
            }
            CargarDatosProductos();
            if (Conexion.InsertSeguro("productos", myProducto))
            {
                MessageBox.Show("Se ha guardado satisfactoriamente el registro");
                //aqui refrescas el grid volviendo a consultar la base de datos
                cargarProductos();
            }//fin del if InsertSeguro
        }

        private void CargarDatosProductos()
        {
            string nombreNormalizado = NormalizarNombre(txtNombre.Text);

            myProducto["Cantidad"] = int.Parse(txtCantidad.Text.Trim());
            myProducto["Precio"] = decimal.Parse(txtPrecio.Text.Trim());
            myProducto["Nombre"] = nombreNormalizado;
            myProducto["Imagen"] = ImageToByteArray(pictureBox1.Image);
        }
        private byte[] ImageToByteArray(Image image)//aqui deberia ir un signo de interrogacion pero tira error
        {
            if (image == null)
                return null;

            using (MemoryStream mMemoryStream = new MemoryStream())
            {
                //guardamos la imagen usando su formato original (RawFormat)
                image.Save(mMemoryStream, image.RawFormat);
                return mMemoryStream.ToArray();

            }
        }
        private bool datosCorrectos()
        {
            string nombre = txtNombre.Text.Trim();
            string precioTexto = txtPrecio.Text.Trim();
            string cantidadTexto = txtCantidad.Text.Trim();

            // VALIDAR NOMBRE
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese el nombre del producto.");
                txtNombre.Focus();
                return false;
            }

            if (nombre.Length < 2)
            {
                MessageBox.Show("El nombre del producto debe tener al menos 2 caracteres.");
                txtNombre.Focus();
                return false;
            }

            if (nombre.Length > 100)
            {
                MessageBox.Show("El nombre del producto no puede superar los 100 caracteres.");
                txtNombre.Focus();
                return false;
            }

            // VALIDAR PRECIO
            if (!decimal.TryParse(precioTexto, out decimal precio))
            {
                MessageBox.Show("Ingrese un precio válido.");
                txtPrecio.Focus();
                return false;
            }

            if (precio < 0)
            {
                MessageBox.Show("El precio no puede ser negativo.");
                txtPrecio.Focus();
                return false;
            }

            if (precio > 999999.99m)
            {
                MessageBox.Show("El precio ingresado es demasiado grande.");
                txtPrecio.Focus();
                return false;
            }

            // VALIDAR CANTIDAD
            if (!int.TryParse(cantidadTexto, out int cantidad))
            {
                MessageBox.Show("Ingrese una cantidad válida.");
                txtCantidad.Focus();
                return false;
            }

            if (cantidad < 0)
            {
                MessageBox.Show("La cantidad no puede ser negativa.");
                txtCantidad.Focus();
                return false;
            }

            if (cantidad > 999999)
            {
                MessageBox.Show("La cantidad ingresada es demasiado grande.");
                txtCantidad.Focus();
                return false;
            }

            return true;
        }
        private string NormalizarNombre(string texto)
        {
            texto = texto.Trim();

            // Elimina espacios repetidos
            texto = System.Text.RegularExpressions.Regex.Replace(
                texto,
                @"\s+",
                " "
            );

            // Primera letra de cada palabra en mayúscula
            // y el resto en minúscula
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(texto.ToLower());
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            ModificarDatosBD();
        }
   
        private void ModificarDatosBD()
        {
            if (!datosCorrectos())
            {
                return;
            }

            CargarDatosProductos();

            int folio;

            if (!int.TryParse(txtFolio.Text.Trim(), out folio))
            {
                MessageBox.Show("El Folio no es válido.");
                return;
            }

            bool resultado = Conexion.UpdateSeguro(
                "productos",
                myProducto,
                "id",
                folio
            );

            if (resultado)
            {
                MessageBox.Show("Actualización exitosa");
                cargarProductos();
                limpiarCampos();
                btnModificar.Enabled = false;
            }
        }
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarCampos();
            btnModificar.Enabled = false;
        }
        private void limpiarCampos()
        {
            txtFolio.Text = "";
            txtNombre.Text = "";
            txtCantidad.Text = "";
            txtPrecio.Text = "";
            btnAgregar.Enabled = true;

        }

        private void btnEiminar_Click(object sender, EventArgs e)
        {
            EliminarDatosBD();
        }
        private void EliminarDatosBD()
        {
            int folio;

            if (!int.TryParse(txtFolio.Text.Trim(), out folio))
            {
                MessageBox.Show("Seleccione un producto para eliminar.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de que desea eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            bool resultado = Conexion.DeleteSeguro(
                "productos",
                "id",
                folio
            );

            if (resultado)
            {
                MessageBox.Show("Producto eliminado correctamente.");

                cargarProductos();
                limpiarCampos();
            }
        }
    }
}
