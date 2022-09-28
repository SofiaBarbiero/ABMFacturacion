using CRUDfacturacion.Dominio;
using CRUDfacturacion.Datos;
using CRUDfacturacion.Servicios.Implementacion;
using CRUDfacturacion.Servicios.Interfaz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CRUDfacturacion.Datos.Implementacion;

namespace CRUDfacturacion
{
    public partial class FrmAlta : Form
    {
        private iServicio gestor;
        private Factura nueva;
        public FrmAlta()
        {
            InitializeComponent();
            gestor = new Servicio();
            nueva = new Factura();
        }

        private void FrmAlta_Load(object sender, EventArgs e)
        {
            ObtenerProximo();
            ObtenerArticulos();
            ObtenerFormas();
        }

        private void ObtenerFormas()
        {
            cboFormas.DataSource = gestor.ObtenerFormas();
            cboFormas.ValueMember = "IdFormaPago";
            cboFormas.DisplayMember = "TipoFP";
            cboFormas.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ObtenerArticulos()
        {
            cboArticulos.DataSource = gestor.ObtenerArticulos();
            cboArticulos.ValueMember = "idArticulo";
            cboArticulos.DisplayMember = "nombre";
            cboArticulos.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ObtenerProximo()
        {
            int next = gestor.ObtenerProximo();
            if (next > 0)
            {
                lblNumFac.Text = "Factura N°: " + next.ToString();
            }
            else
            {
                MessageBox.Show("Error de datos. No se puede obtener Nº de factura!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if(cboArticulos.SelectedIndex == -1)
            {
                MessageBox.Show("Tiene que seleccionar un articulo", "Control", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            if(int.Parse(txtCantidad.Text) < 0)
            {
                MessageBox.Show("Ingrese una cantidad valida", "Control", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            foreach(DataGridViewRow row in dgvFacturas.Rows)
            {
                if (row.Cells["ColArticulo"].Value.ToString().Equals(cboArticulos.Text))
                {
                    MessageBox.Show("El articulo: " + cboArticulos.Text + " ya se encuentra en la lista",
                                    "Control", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            Articulo a = (Articulo)cboArticulos.SelectedItem;
            int cantidad = int.Parse(txtCantidad.Text);
            DetalleFactura d = new DetalleFactura(a, cantidad);
            nueva.AgregarDetalle(d);
            dgvFacturas.Rows.Add(d.Articulo.IdArticulo, d.Articulo.Nombre, d.Cantidad, d.Articulo.PrecioUnitario);

            txtSub.Text = d.CalcularSubTotal().ToString();
            txtTotal.Text = nueva.CalcularTotal().ToString();
           
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if(txtCliente.Text == string.Empty)
            {
                MessageBox.Show("Tiene que agregar un cliente", "Control", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            if(cboFormas.SelectedIndex == -1)
            {
                MessageBox.Show("Tiene que seleccionar una forma de pago", "Control", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            GuardarFactura();
        }

        private void GuardarFactura()
        {
            FormaPago f = (FormaPago)cboFormas.SelectedItem;
            nueva.FormaPago = f;
            nueva.Cliente = txtCliente.Text;
            nueva.Fecha = dateTimePicker1.Value; 

            if(Helper.ObtenerInstancia().ConfirmarFactura(nueva))
            {
                MessageBox.Show("Factura registrada con exito!", "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("No se pudo registrar la factura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtCantidad.Text = "";
            txtCliente.Text = "";
            txtSub.Text = "";
            txtTotal.Text = "";
            cboArticulos.SelectedIndex = -1;
            cboFormas.SelectedIndex = -1;
            dgvFacturas.Rows.Clear();
        }
    }
}
