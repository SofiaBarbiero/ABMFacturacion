using CRUDfacturacion.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDfacturacion.Datos
{
    class Helper
    {
        private static Helper instancia;
        private SqlConnection cnn;

        public static Helper ObtenerInstancia()
        {
            if (instancia == null)
            {
                instancia = new Helper();
            }
            return instancia;
        }

        public Helper()
        {
            cnn = new SqlConnection(Properties.Resources.cnnString);
        }

        public int ObtenerProximo(string sp, string nombrePOut)
        {
            SqlCommand cmdProximo = new SqlCommand();
            cnn.Open();
            cmdProximo.Connection = cnn;
            cmdProximo.CommandText = sp;
            cmdProximo.CommandType = CommandType.StoredProcedure;
            SqlParameter pOut = new SqlParameter();
            pOut.ParameterName = nombrePOut;
            pOut.Direction = ParameterDirection.Output;
            pOut.DbType = DbType.Int32;
            cmdProximo.Parameters.Add(pOut);
            cmdProximo.ExecuteNonQuery();
            cnn.Close();
            return (int)pOut.Value;
        }

        public DataTable CargarCombo(string sp)
        {
            DataTable table = new DataTable();
            SqlCommand cmdCombo = new SqlCommand();
            cnn.Open();
            cmdCombo.Connection = cnn;
            cmdCombo.CommandType = CommandType.StoredProcedure;
            cmdCombo.CommandText = sp;
            table.Load(cmdCombo.ExecuteReader());
            cnn.Close();
            return table;
        }

        public bool ConfirmarFactura(Factura oFactura)
        {
            bool ok = true;
            SqlTransaction trs = null;
            try
            {
                SqlCommand cmdMaestro = new SqlCommand();
                cnn.Open();
                trs = cnn.BeginTransaction();
                cmdMaestro.Connection = cnn;
                cmdMaestro.Transaction = trs;
                cmdMaestro.CommandType = CommandType.StoredProcedure;
                cmdMaestro.CommandText = "SP_INSERTAR_MAESTRO";
                cmdMaestro.Parameters.AddWithValue("@fecha", oFactura.Fecha);
                cmdMaestro.Parameters.AddWithValue("@id_formapago", oFactura.FormaPago.IdFormaPago);
                cmdMaestro.Parameters.AddWithValue("@cliente", oFactura.Cliente);
                SqlParameter pOut = new SqlParameter();
                pOut.ParameterName = "@nro_factura";
                pOut.DbType = DbType.Int32;
                pOut.Direction = ParameterDirection.Output;
                cmdMaestro.Parameters.Add(pOut);
                cmdMaestro.ExecuteNonQuery();

                int facturaNro = (int)pOut.Value;

                foreach(DetalleFactura item in oFactura.ListDetalles)
                {
                    SqlCommand cmdDetalle = new SqlCommand();
                    cmdDetalle.Connection = cnn;
                    cmdDetalle.Transaction = trs;
                    cmdDetalle.CommandType = CommandType.StoredProcedure;
                    cmdDetalle.CommandText = "SP_INSERTAR_DETALLES";
                    cmdDetalle.Parameters.AddWithValue("@nro_factura", facturaNro);
                    cmdDetalle.Parameters.AddWithValue("@id_articulo", item.Articulo.IdArticulo);
                    cmdDetalle.Parameters.AddWithValue("@cantidad", item.Cantidad);
                    cmdDetalle.ExecuteNonQuery();
                }
                trs.Commit();
            }
            catch(Exception)
            {
                if(trs != null)
                {
                    trs.Rollback();
                    ok = false;
                }
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
            return ok;
        }
    }
}
