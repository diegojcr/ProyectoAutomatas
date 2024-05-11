using System;
using System.IO;
using System.Collections.Generic;

namespace Proyecto1_JoseAlvarado_DiegoCosillo
{
	class Program
	{
		static void Main(string[] args)
		{
			// Ruta del archivo
			string filePath = @"C:\Users\Diego\Desktop\ProyectoAutomatas\NFA #2.txt";

			try
			{
				// Leer el archivo y mostrar la información
				string[] automatonInfo = ReadAutomatonFile(filePath);
				if (automatonInfo == null)
				{
					Console.WriteLine("Error: El archivo de definición del autómata no tiene el formato esperado.");
					return;
				}
				Console.WriteLine("Nombre del archivo: " + automatonInfo[0]);
				Console.WriteLine("Estado inicial: " + automatonInfo[1]);
				Console.WriteLine("Estados finales: " + automatonInfo[2]);

				while (true)
				{
					// Solicitar al usuario que ingrese la cadena a validar
					Console.Write("Ingresa la cadena a validar (o 'q' para salir): ");
					string inputString = Console.ReadLine();

					if (inputString.ToLower() == "q")
					{
						break;
					}

					// Validar la cadena
					bool isAccepted = ValidateInput(inputString, filePath);
					if (isAccepted)
					{
						Console.WriteLine("La cadena es aceptada por el autómata.");
					}
					else
					{
						Console.WriteLine("La cadena no es aceptada por el autómata.");
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: " + e.Message);
			}
		}

		static string[] ReadAutomatonFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				Console.WriteLine("Error: El archivo de definición del autómata no existe.");
				return null;
			}

			// Leer todas las líneas del archivo
			string[] lines = File.ReadAllLines(filePath);

			if (lines.Length < 3)
			{
				Console.WriteLine("Error: El archivo de definición del autómata no tiene el formato esperado.");
				return null;
			}

			// Nombre del archivo
			string fileName = Path.GetFileName(filePath);

			// Estado inicial
			string initialState = lines[1];

			// Estados finales
			string[] finalStates = lines[2].Split(',');

			// Retornar la información como un arreglo de strings
			return new string[] { fileName, initialState, string.Join(",", finalStates) };
		}

		static bool ValidateInput(string inputString, string filePath)
		{
			// Leer todas las líneas del archivo
			string[] lines = File.ReadAllLines(filePath);

			if (lines.Length < 4)
			{
				Console.WriteLine("Error: El archivo de definición del autómata no tiene el formato esperado.");
				return false;
			}

			// Crear una lista de arreglos de strings para almacenar las transiciones
			List<string[]> transitionsList = new List<string[]>();

			// Llenar la lista de transiciones
			for (int i = 3; i < lines.Length; i++)
			{
				string[] parts = lines[i].Split(',');
				if (parts.Length != 3)
				{
					Console.WriteLine("Error: El archivo de definición del autómata no tiene el formato esperado.");
					return false;
				}
				transitionsList.Add(new string[] { parts[0].Trim(), parts[1].Trim(), parts[2].Trim() });
			}

			// Estado inicial
			string initialState = lines[1];

			// Estados finales
			string[] finalStates = lines[2].Split(',');

			// Inicializar el conjunto de estados actuales con el estado inicial
			HashSet<string> currentStates = new HashSet<string>();
			currentStates.Add(initialState);

			// Calcular epsilon cierre del estado inicial
			currentStates.UnionWith(GetEpsilonClosure(initialState, transitionsList));

			// Recorrer la cadena de entrada
			foreach (char symbol in inputString)
			{
                // Encontrar todos los estados alcanzables desde los estados actuales con el símbolo actual
                HashSet<string> nextStates = new HashSet<string>();
                foreach (string currentState in currentStates)
                {
                    foreach (string[] transition in transitionsList)
                    {
                        if (transition[0] == currentState && (transition[1] == symbol.ToString() || (transition[1] == "\u03B5" && symbol == '\u03B5')))
                        {
                            nextStates.UnionWith(GetEpsilonClosure(transition[2], transitionsList));
                        }
                    }
                }

                // Actualizar los estados actuales
                currentStates = nextStates;

				// Si no hay estados alcanzables para el símbolo actual, la cadena no es aceptada
				if (currentStates.Count == 0)
				{
					return false;
				}
			}

			// Verificar si al menos uno de los estados actuales es un estado final
			foreach (string state in currentStates)
			{
				if (Array.IndexOf(finalStates, state) != -1)
				{
					return true;
				}
			}

			// Si ningún estado actual es un estado final, la cadena no es aceptada
			return false;
		}

		static HashSet<string> GetEpsilonClosure(string state, List<string[]> transitionsList)
		{
			HashSet<string> epsilonClosure = new HashSet<string>();
			Stack<string> stack = new Stack<string>();
			stack.Push(state);
			epsilonClosure.Add(state);

			while (stack.Count > 0)
			{
				string currentState = stack.Pop();
				foreach (string[] transition in transitionsList)
				{
					if (transition[0] == currentState && transition[1] == "\u03B5")
					{
						if (!epsilonClosure.Contains(transition[2]))
						{
							epsilonClosure.Add(transition[2]);
							stack.Push(transition[2]);
						}
					}
				}
			}

			return epsilonClosure;
		}
	}
}



