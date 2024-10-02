using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Dominio;
using NegocioArticulo;
using System.Globalization;

namespace Presentacion
{
    public partial class frmArticulo : Form
    {
        private List<Articulo> listaArticulo;
        public frmArticulo()
        {
            InitializeComponent();
        }

        private void frmArticulo_Load(object sender, EventArgs e)
        {
            //this.BackColor = System.Drawing.Color.GreenYellow;
            dgvArticulo.DefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Regular); // Cambia solo el DataGridView
            dgvArticulo.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 11, FontStyle.Bold); // Cambia solo los encabezados
            

            cargar();
            
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripcion");
            cboCampo.Items.Add("Precio");
            dgvArticulo.Columns["Codigo"].DisplayIndex = 0;    // "Codigo" en la primera posición
            dgvArticulo.Columns["Nombre"].DisplayIndex = 1;    // "Nombre" en la segunda posición
            dgvArticulo.Columns["Marca"].DisplayIndex = 2;     // "Marca" en la tercera posición
            dgvArticulo.Columns["Categoria"].DisplayIndex = 3; // "Categoria" en la cuarta posición
            dgvArticulo.Columns["Descripcion"].DisplayIndex = 4; // "Descripcion" en la quinta posición
            dgvArticulo.Columns["Precio"].DisplayIndex = 5;


        }

        private void lblPrecio_Click(object sender, EventArgs e)
        {
            
        }
        private void cargar()
        {
            Negocio negocio = new Negocio();
            try
            {
                listaArticulo = negocio.listar();
                dgvArticulo.DataSource = listaArticulo;
                ocultarColumnas();
                
                pbArticulo.Load(listaArticulo[0].ImagenUrl);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }
        private void ocultarColumnas()
        {
            dgvArticulo.Columns["ImagenUrl"].Visible = false; //ocultamos la url
            dgvArticulo.Columns["Id"].Visible = false;
        }

        private void dgvArticulo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulo.CurrentRow != null)
            {

                Articulo seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.ImagenUrl);
            }
            
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbArticulo.Load(imagen);
            }
            catch (Exception)
            {

                pbArticulo.Load("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR8bikI-KUuM1IWosgqDRS5jyv2U_PPYlG6Tg&s");
            }

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.ShowDialog();
            cargar();

        }
        
        private void btnBorrar_Click(object sender, EventArgs e)
        {
            Negocio negocio = new Negocio();
            Articulo seleccionado;
            try
            {
                DialogResult respuesta =MessageBox.Show("Estas por eliminar de manera fisica, estas seguro?","Eliminando",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (respuesta == DialogResult.Yes)
                {

                    seleccionado = (Articulo)(dgvArticulo.CurrentRow.DataBoundItem);
                    negocio.eliminar(seleccionado.Id);
                    cargar();

                }
                    


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private void dgvArticulo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;

            frmAltaArticulo modificar = new frmAltaArticulo(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void btnBorrarLogico_Click(object sender, EventArgs e)
        {

        }
        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex == -1) {

                MessageBox.Show("Seleccione el campo para filtrar");
                return true;
            
            }
            if (cboCriterio.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el criterio para filtrar");
                return true;
            }

            //precio
            if (cboCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar el filtro numérico");
                    return true;
                }

                if (!float.TryParse(txtFiltroAvanzado.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                {
                    MessageBox.Show("El filtro debe ser un valor numérico.");
                    return true;
                }
            }

            //descripcion
            if (cboCampo.SelectedItem.ToString() == "Descripcion")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar el filtro");
                    return true;
                }

                // Verificar que el filtro no contenga números para "Descripción"
                if (txtFiltroAvanzado.Text.Any(char.IsDigit))
                {
                    MessageBox.Show("El filtro solo puede contener letras.");
                    return true;
                }
            }
            //nombre
            if (cboCampo.SelectedItem.ToString() == "Nombre")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar el filtro");
                    return true;
                }
            }
            return false;
        }
        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                {
                    return false;
                }
                
            }return true;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

            try
            {
                if (validarFiltro())
                
                    return;
                    string campo = cboCampo.SelectedItem.ToString();
                    string criterio = cboCriterio.SelectedItem.ToString();
                    string filtro = txtFiltroAvanzado.Text.Trim();
                    dgvArticulo.DataSource = Negocio.filtrar(campo, criterio, filtro);



            }
            catch (Exception ex)
            {

                 MessageBox.Show(ex.ToString());
            }
            
        }

        private void txtFiltro_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {



            List<Articulo> listaFiltrada;
            string filtro = txtFiltro.Text;
            if (filtro != "")
            {
                listaFiltrada = listaArticulo.FindAll(i =>
                i.Nombre.ToUpper().Contains(filtro.ToUpper()) ||
                i.Codigo.ToUpper().Contains(filtro.ToUpper()) ||
                (i.Categoria != null && i.Categoria.Descripcion.ToUpper().Contains(filtro.ToUpper())) ||
                i.Marca.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaArticulo;
            }
            dgvArticulo.DataSource = null; //primero limpieza
            dgvArticulo.DataSource = listaFiltrada;
            ocultarColumnas();

        }

        private void lblCriterio_Click(object sender, EventArgs e)
        {

        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();//guarda el elemento seleccionado
            if(opcion == "Precio")
            {
                cboCriterio .Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();

                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        private void dgvArticulo_DefaultCellStyleChanged(object sender, EventArgs e)
        {
           
        }

        private void dgvArticulo_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvArticulo.Columns["Precio"].DefaultCellStyle.Format = "N3";
            dgvArticulo.Columns["Precio"].DefaultCellStyle.Format = "0.00#";


        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dgvArticulo.Columns["Codigo"].DisplayIndex = 0;    
            dgvArticulo.Columns["Nombre"].DisplayIndex = 1;    
            dgvArticulo.Columns["Marca"].DisplayIndex = 2;     
            dgvArticulo.Columns["Categoria"].DisplayIndex = 3; 
            dgvArticulo.Columns["Descripcion"].DisplayIndex = 4; 
            dgvArticulo.Columns["Precio"].DisplayIndex = 5;    
            cargar();
        }
    }
}

