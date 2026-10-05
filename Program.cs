using System;
using GestionVehiculos.Modelos;
using GestionVehiculos.Servicios;

namespace GestionVehiculos
{
    public class Program
    {
        static GestorFlota gestor = new GestorFlota();

        public static void Main(string[] args)
        {
            int opcion;
            do
            {
                MostrarMenu();
                opcion = LeerEntero("Seleccione una opción: ");
                Console.WriteLine();

                switch (opcion)
                {
                    case 1: RegistrarVehiculo(); break;
                    case 2: gestor.MostrarVehiculos(); break;
                    case 3: BuscarVehiculo(); break;
                    case 4: gestor.MostrarVehiculos("Disponible"); break;
                    case 5: gestor.MostrarVehiculos("Rentado"); break;
                    case 6: CambiarEstado(); break;
                    case 7: MostrarMayorKilometraje(); break;
                    case 8: MostrarPromedio(); break;
                    case 9: gestor.MostrarConteoPorTipo(); break;
                    case 10: RealizarMantenimiento(); break;
                    case 11: Console.WriteLine("Saliendo del sistema. ¡Hasta pronto!"); break;
                    default: MostrarError("Opción no válida. Elija un número entre 1 y 11."); break;
                }

                if (opcion != 11)
                {
                    Console.WriteLine("\nPresione una tecla para continuar...");
                    Console.ReadKey();
                }
            } while (opcion != 11);
        }

        static void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("===== SISTEMA DE RENTA DE VEHÍCULOS =====");
            Console.WriteLine("========================================\n");
            Console.WriteLine("1. Registrar vehículo");
            Console.WriteLine("2. Mostrar todos los vehículos");
            Console.WriteLine("3. Buscar vehículo por código");
            Console.WriteLine("4. Mostrar vehículos disponibles");
            Console.WriteLine("5. Mostrar vehículos rentados");
            Console.WriteLine("6. Cambiar estado");
            Console.WriteLine("7. Vehículo con mayor kilometraje");
            Console.WriteLine("8. Calcular precio promedio de renta");
            Console.WriteLine("9. Cantidad de vehículos por tipo");
            Console.WriteLine("10. Realizar mantenimiento");
            Console.WriteLine("11. Salir\n");
        }

        // ---------- Opción 1 ----------
        static void RegistrarVehiculo()
        {
            Console.WriteLine("--- Tipo de vehículo ---");
            Console.WriteLine("1. Automóvil");
            Console.WriteLine("2. Camioneta");
            Console.WriteLine("3. Motocicleta");
            int tipo = LeerEnteroEnRango("Seleccione el tipo: ", 1, 3);

            // Datos generales
            string codigo;
            while (true)
            {
                codigo = LeerTexto("Código: ");
                if (gestor.BuscarPorCodigo(codigo) == null) break;
                MostrarError("Ya existe un vehículo con ese código. Intente con otro.");
            }
            string marca = LeerTexto("Marca: ");
            string modelo = LeerTexto("Modelo: ");
            int anio = LeerEnteroEnRango("Año: ", 1950, DateTime.Now.Year + 1);
            double km = LeerDoubleNoNegativo("Kilometraje: ");
            double precio = LeerDoubleNoNegativo("Precio de renta por día: ");

            Vehiculo nuevo;

            // Datos específicos
            switch (tipo)
            {
                case 1:
                    int puertas = LeerEnteroEnRango("Número de puertas: ", 2, 6);
                    string transmision = LeerOpcion("Transmisión", "Manual", "Automática");
                    string combustible = LeerOpcion("Combustible", "Gasolina", "Diésel", "Híbrido");
                    bool aire = LeerBool("¿Tiene aire acondicionado? (s/n): ");
                    int pasajeros = LeerEnteroEnRango("Número de pasajeros: ", 1, 9);
                    nuevo = new Automovil(codigo, marca, modelo, anio, km, precio,
                                          puertas, transmision, combustible, aire, pasajeros);
                    break;

                case 2:
                    double carga = LeerDoubleNoNegativo("Capacidad de carga (Ton): ");
                    string traccion = LeerOpcion("Tracción", "4x2", "4x4");
                    bool doble = LeerBool("¿Tiene doble cabina? (s/n): ");
                    int pasCam = LeerEnteroEnRango("Número de pasajeros: ", 1, 9);
                    nuevo = new Camioneta(codigo, marca, modelo, anio, km, precio,
                                          carga, traccion, doble, pasCam);
                    break;

                default:
                    int cilindrada = LeerEnteroEnRango("Cilindrada (cc): ", 50, 2500);
                    string tipoMoto = LeerOpcion("Tipo de motocicleta", "Scooter", "Enduro", "Pistera");
                    bool baul = LeerBool("¿Tiene baúl? (s/n): ");
                    int cascos = LeerEnteroEnRango("Número de cascos: ", 0, 2);
                    nuevo = new Motocicleta(codigo, marca, modelo, anio, km, precio,
                                            cilindrada, tipoMoto, baul, cascos);
                    break;
            }

            if (gestor.Registrar(nuevo))
                MostrarOk("Vehículo registrado correctamente.");
            else
                MostrarError("No se pudo registrar el vehículo.");
        }

        // ---------- Opción 3 ----------
        static void BuscarVehiculo()
        {
            string codigo = LeerTexto("Código a buscar: ");
            Vehiculo? v = gestor.BuscarPorCodigo(codigo);
            if (v == null)
                MostrarError("No se encontró un vehículo con ese código.");
            else
            {
                v.MostrarInformacion();
                gestor.MostrarDetalleEspecifico(v);
                Console.WriteLine("----------------------------------------");
            }
        }

        // ---------- Opción 6 ----------
        static void CambiarEstado()
        {
            string codigo = LeerTexto("Código del vehículo: ");
            Vehiculo? v = gestor.BuscarPorCodigo(codigo);
            if (v == null)
            {
                MostrarError("No se encontró un vehículo con ese código.");
                return;
            }

            Console.WriteLine($"Estado actual: {v.Estado}");
            string nuevo = LeerOpcion("Nuevo estado", GestorFlota.EstadosValidos);
            gestor.CambiarEstado(codigo, nuevo);
            MostrarOk($"Estado actualizado a '{nuevo}'.");
        }

        // ---------- Opción 7 ----------
        static void MostrarMayorKilometraje()
        {
            Vehiculo? mayor = gestor.ObtenerMayorKilometraje();
            if (mayor == null)
            {
                MostrarError("No hay vehículos registrados.");
                return;
            }
            Console.WriteLine("Vehículo con mayor kilometraje:");
            mayor.MostrarInformacion();
            Console.WriteLine("----------------------------------------");
        }

        // ---------- Opción 8 ----------
        static void MostrarPromedio()
        {
            if (gestor.Cantidad == 0)
            {
                MostrarError("No hay vehículos registrados.");
                return;
            }
            Console.WriteLine($"Precio promedio de renta por día: ${gestor.CalcularPrecioPromedio():F2}");
        }

        // ---------- Opción 10 ----------
        static void RealizarMantenimiento()
        {
            string codigo = LeerTexto("Código del vehículo: ");
            if (!gestor.EjecutarMantenimiento(codigo))
                MostrarError("No se encontró un vehículo con ese código.");
        }

        // ================= LECTURA SEGURA =================
        public static int LeerEntero(string mensaje)
        {
            int valor;
            Console.Write(mensaje);
            while (!int.TryParse(Console.ReadLine(), out valor))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Entrada no válida. Ingrese un número entero: ");
                Console.ResetColor();
            }
            return valor;
        }

        public static int LeerEnteroEnRango(string mensaje, int min, int max)
        {
            int valor = LeerEntero(mensaje);
            while (valor < min || valor > max)
            {
                MostrarError($"El valor debe estar entre {min} y {max}.");
                valor = LeerEntero(mensaje);
            }
            return valor;
        }

        public static double LeerDoubleNoNegativo(string mensaje)
        {
            double valor;
            Console.Write(mensaje);
            while (!double.TryParse(Console.ReadLine(), out valor) || valor < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Entrada no válida. Ingrese un número mayor o igual a 0: ");
                Console.ResetColor();
            }
            return valor;
        }

        public static string LeerTexto(string mensaje)
        {
            Console.Write(mensaje);
            string? texto = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(texto))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("El campo no puede estar vacío. Intente de nuevo: ");
                Console.ResetColor();
                texto = Console.ReadLine();
            }
            return texto.Trim();
        }

        public static bool LeerBool(string mensaje)
        {
            Console.Write(mensaje);
            while (true)
            {
                string r = (Console.ReadLine() ?? "").Trim().ToLower();
                if (r == "s" || r == "si" || r == "sí") return true;
                if (r == "n" || r == "no") return false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Responda 's' o 'n': ");
                Console.ResetColor();
            }
        }

        public static string LeerOpcion(string titulo, params string[] opciones)
        {
            Console.WriteLine($"{titulo}:");
            for (int i = 0; i < opciones.Length; i++)
                Console.WriteLine($"  {i + 1}. {opciones[i]}");
            int sel = LeerEnteroEnRango("Seleccione: ", 1, opciones.Length);
            return opciones[sel - 1];
        }

        static void MostrarError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static void MostrarOk(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}