﻿using System;
using System.IO;
using System.Collections.Generic;

namespace Proyecto1_JoseAlvarado_DiegoCosillo
{





	class Program
	{
		static void Main(string[] args)
		{
			// Ruta del archivo
			string filePath = @"C:\Users\Diego\Desktop\ProyectoAutomatas\Automata #1.txt";

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
				Console.WriteLine("Estado final: " + automatonInfo[2]);

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

			// Estado final
			string finalState = lines[2];

			// Retornar la información como un arreglo de strings
			return new string[] { fileName, initialState, finalState };
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

			// Obtener el número de estados
			int numStates;
			if (!int.TryParse(lines[0], out numStates))
			{
				Console.WriteLine("Error: El número de estados en el archivo de definición del autómata no es válido.");
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

            // Convertir la lista de transiciones a una matriz
            string[,] transitions = new string[transitionsList.Count, 3];
            for (int i = 0; i < transitionsList.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    transitions[i, j] = transitionsList[i][j];
                }
            }

            // Estado inicial
            string initialState = lines[1];

			// Estado final
			string finalState = lines[2];

			// Inicializar el estado actual con el estado inicial
			string currentState = initialState;

			// Recorrer la cadena de entrada
			foreach (char symbol in inputString)
			{
				// Encontrar la transición correspondiente
				bool foundTransition = false;
				for (int i = 0; i < transitionsList.Count; i++)
				{
					if (transitions[i, 0] == currentState && transitions[i, 1] == symbol.ToString())
					{
						// Actualizar el estado actual
						currentState = transitions[i, 2];
						foundTransition = true;
						break;
					}
				}

				// Si no se encontró una transición, la cadena no es aceptada
				if (!foundTransition)
				{
					return false;
				}
			}

			// Verificar si el estado actual es un estado final
			return currentState == finalState;
		}
	}
}
