using System;
using System.Collections.Generic;
using GestionVehiculos.Interfaces;
using GestionVehiculos.Modelos;

namespace GestionVehiculos.Servicios
{
    public class GestorFlota
    {
        private readonly List<Vehiculo> flota = new List<Vehiculo>();

        public static readonly string[] EstadosValidos = { "Disponible", "Rentado", "En Mantenimiento" };

        public int Cantidad => flota.Count;

        // Registro polimórfico: se guarda cualquier derivada como Vehiculo
        public bool Registrar(Vehiculo vehiculo)
        {
            if (BuscarPorCodigo(vehiculo.Codigo) != null)
                return false; // código duplicado
            flota.Add(vehiculo);
            return true;
        }

        // Opción 3: búsqueda por código
        public Vehiculo? BuscarPorCodigo(string codigo)
        {
            foreach (Vehiculo v in flota)
            {
                if (v != null && v.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                    return v;
            }
            return null;
        }

        // Opción 2 (sobrecarga 1): mostrar todos
        public void MostrarVehiculos()
        {
            if (flota.Count == 0)
            {
                Console.WriteLine("No hay vehículos registrados.");
                return;
            }
            foreach (Vehiculo v in flota)
                v.MostrarInformacion(); // polimorfismo: se ejecuta el override de cada clase
            Console.WriteLine("----------------------------------------");
        }

        // Opciones 4 y 5 (sobrecarga 2): filtrar por estado
        public void MostrarVehiculos(string estado)
        {
            int encontrados = 0;
            foreach (Vehiculo v in flota)
            {
                if (v.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase))
                {
                    v.MostrarInformacion();
                    encontrados++;
                }
            }
            if (encontrados == 0)
                Console.WriteLine($"No hay vehículos con estado '{estado}'.");
            else
                Console.WriteLine("----------------------------------------");
        }

        // Opción 6: cambiar estado
        public bool CambiarEstado(string codigo, string nuevoEstado)
        {
            Vehiculo? v = BuscarPorCodigo(codigo);
            if (v == null) return false;

            if (nuevoEstado == "En Mantenimiento")
                v.SolicitarMantenimiento();
            else
                v.Estado = nuevoEstado;
            return true;
        }

        // Opción 7: máximo kilometraje
        public Vehiculo? ObtenerMayorKilometraje()
        {
            if (flota.Count == 0) return null;

            Vehiculo mayor = flota[0];
            foreach (var v in flota)
            {
                if (v != null && v.Kilometros > mayor.Kilometros)
                    mayor = v;
            }
            return mayor;
        }

        // Opción 8: precio promedio
        public double CalcularPrecioPromedio()
        {
            if (flota.Count == 0) return 0;

            double suma = 0;
            foreach (var v in flota)
                suma += v.PrecioRenta;
            return suma / flota.Count;
        }

        // Opción 9: conteo por tipo con el operador is
        public void MostrarConteoPorTipo()
        {
            int autos = 0, camionetas = 0, motos = 0;
            foreach (var v in flota)
            {
                if (v is Automovil) autos++;
                else if (v is Camioneta) camionetas++;
                else if (v is Motocicleta) motos++;
            }

            Console.WriteLine($"Automóviles : {autos}");
            Console.WriteLine($"Camionetas  : {camionetas}");
            Console.WriteLine($"Motocicletas: {motos}");
            Console.WriteLine($"Total       : {flota.Count}");
        }

        // Opción 10: mantenimiento mediante la interfaz IMantenimiento
        public bool EjecutarMantenimiento(string codigo)
        {
            Vehiculo? v = BuscarPorCodigo(codigo);
            if (v == null) return false;

            IMantenimiento mantenimiento = v; // el vehículo se usa a través del contrato de la interfaz
            mantenimiento.ConsultarMantenimiento();
            mantenimiento.RealizarMantenimiento();
            mantenimiento.ConsultarMantenimiento();
            return true;
        }

        // Casting explícito para mostrar datos propios de cada derivada
        public void MostrarDetalleEspecifico(Vehiculo v)
        {
            if (v is Automovil)
            {
                Automovil auto = (Automovil)v; // casting explícito
                Console.WriteLine($"(Automóvil de {auto.NumeroPuertas} puertas)");
            }
            else if (v is Camioneta)
            {
                Camioneta cam = (Camioneta)v;
                Console.WriteLine($"(Camioneta con {cam.CapacidadCarga} Ton de carga)");
            }
            else if (v is Motocicleta)
            {
                Motocicleta moto = (Motocicleta)v;
                Console.WriteLine($"(Motocicleta de {moto.Cilindrada} cc)");
            }
        }
    }
}