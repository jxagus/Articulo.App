using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using NegocioArticulo;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Presentacion
{
    public partial class frmAltaArticulo : Form
    {
        private Articulo articulo = null;
        private OpenFileDialog archivo = null;

        public frmAltaArticulo()
        {
            InitializeComponent();
        }
        public frmAltaArticulo(Articulo articulo) 
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificar Articulo";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            
            Negocio negocio = new Negocio();
            try
            {
                if (articulo == null)
                {
                    articulo = new Articulo();
                }
                articulo.Codigo = tbCodigo.Text;
                articulo.Nombre = tbNombre.Text;
                articulo.Descripcion = tbDescripcion.Text;
                articulo.Categoria = (Elemento)cbTipo.SelectedItem;
                articulo.Marca = (Elemento)cbMarca.SelectedItem;
                articulo.ImagenUrl = tbImagen.Text;
                

                if (float.TryParse(tbPrecio.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float precio))
                {
                    articulo.Precio = Math.Round((decimal)precio, 2);  // Redondear a 2 decimales
                }
                else
                {
                    MessageBox.Show("Por favor, ingrese un precio válido."); // Mensaje de error si la conversión falla
                    return;
                }
                if (string.IsNullOrWhiteSpace(tbCodigo.Text) || string.IsNullOrWhiteSpace(tbNombre.Text) || string.IsNullOrWhiteSpace(tbDescripcion.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios: Código, Nombre y Descripción.");
                    return;
                }
                if (!Regex.IsMatch(tbDescripcion.Text, @"^[a-zA-Z\s]+$"))
                {
                    MessageBox.Show("La descripción solo debe contener letras.");
                    return;
                }
                if (articulo.Id != 0) {
                    negocio.modificar(articulo);
                    MessageBox.Show("modificado exitosamente");
                }
                else
                {
                    negocio.agregar(articulo);
                    MessageBox.Show("agregado exitosamente");

                }
                    if (archivo != null && !(tbImagen.Text.ToUpper().Contains("HTTP")))
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
                }

               
                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void frmAltaArticulo_Load(object sender, EventArgs e)
        {
            CategoriaNegocio negocio = new CategoriaNegocio();
            
            MarcaNegocio marca = new MarcaNegocio();

            try
            {
                //List<Elemento> listaTipos = negocio.listar();
                //List<Elemento> listaTipos2 = marca.listar();
                cbTipo.DataSource = negocio.listar();
                cbTipo.ValueMember = "Id";
                cbTipo.DisplayMember = "Descripcion";

                cbMarca.DataSource = marca.listar();
                cbMarca.ValueMember = "Id";
                cbMarca.DisplayMember = "Descripcion";

                if (articulo != null)
                {
                    tbCodigo.Text = articulo.Codigo;
                    tbNombre.Text = articulo.Nombre;
                    tbDescripcion.Text = articulo.Descripcion;
                    tbPrecio.Text = articulo.Precio.ToString();
                    tbImagen.Text = articulo.ImagenUrl;

                    cargarImagen(articulo.ImagenUrl);
                    cbTipo.SelectedValue = articulo.Categoria.Id;
                    cbMarca.SelectedValue = articulo.Marca.Id;





                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cbMarca_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tbImagen_Leave(object sender, EventArgs e)
        {
            //Cargamos imagen con este evento
            cargarImagen(tbImagen.Text);

        }

        private void tbDescripcion_Leave(object sender, EventArgs e)
        {
            
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbEjemplo.Load(imagen);
            }
            catch (Exception)
            {

                pbEjemplo.Load("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR8bikI-KUuM1IWosgqDRS5jyv2U_PPYlG6Tg&s");
            }

        }

        private void pbEjemplo_Click(object sender, EventArgs e)
        {

        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            OpenFileDialog archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg; |png|*.png";
            if(archivo.ShowDialog()== DialogResult.OK)
            {
                tbImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"]+archivo.SafeFileName);
            }

        }
    }
}
