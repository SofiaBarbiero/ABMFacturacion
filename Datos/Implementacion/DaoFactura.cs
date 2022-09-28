using CRUDfacturacion.Datos.Interfaz;
using CRUDfacturacion.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDfacturacion.Datos.Implementacion
{
    class DaoFactura : iDaoFactura
    {
        public List<Articulo> ObtenerArticulos()
        {
            List<Articulo> lst = new List<Articulo>();
            string sp = "SP_ARTICULOS";
            DataTable table = Helper.ObtenerInstancia().CargarCombo(sp);
            foreach (DataRow dr in table.Rows)
            {
                int id = int.Parse(dr["id_articulo"].ToString());
                string nombre = dr["descripcion"].ToString();
                double precio = double.Parse(dr["pre_unitario"].ToString());
                Articulo aux = new Articulo(id, nombre, precio);
                lst.Add(aux);
            }
            return lst;
        }

        public List<FormaPago> ObtenerFormas()
        {
            List<FormaPago> lst = new List<FormaPago>();
            string sp = "SP_FORMAS_PAGO";
            DataTable table = Helper.ObtenerInstancia().CargarCombo(sp);
            foreach(DataRow dr in table.Rows)
            {
                int id = Convert.ToInt32(dr["ID_FORMAPAGO"]);
                string forma = dr["FORMAPAGO"].ToString();
                FormaPago aux = new FormaPago(id, forma);
                lst.Add(aux);
            }
            return lst;
        }

        public int ObtenerProximo()
        {
            string sp = "SP_PROXIMO";
            string pOut = "@next";
            return Helper.ObtenerInstancia().ObtenerProximo(sp, pOut);
        }
    }
}
