using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите количество строк массива:");
        int n = int.Parse(Console.ReadLine()); 
        Console.WriteLine("Введите количество столбцов массива:");
        int m = int.Parse(Console.ReadLine()); 
        int[,] array = new int[n, m]; 
        Console.WriteLine("Введите элементы массива построчно, разделяя пробелами:");
        for (int i = 0; i < n; i++) 
        {
            string[] line = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries); 
            for (int j = 0; j < m; j++) 
            {
                if (int.TryParse(line[j], out int value))
                {
                    array[i, j] = value;
                }
                else
                {
                    array[i, j] = 0;
                }
            }
        }

        
        Console.WriteLine("Введенный массив:");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                Console.Write(array[i, j] + " ");
            }
            Console.WriteLine();
        }

        
        Console.WriteLine("Для каждой строки массива находим минимум, максимум и сумму:");
        for (int i = 0; i < n; i++)
        {
            FindMin(array, i, out int min); 
            FindMax(array, i, out int max); 
            FindSum(array, i, out int sum); 
            Console.WriteLine($"Строка {i}: минимум = {min}, максимум = {max}, сумма = {sum}"); 
        }
    }
    static void FindMin(in int[,] array, int row, out int min)
    {
        min = int.MaxValue; 
        for (int j = 0; j < array.GetLength(1); j++) 
        {
            if (array[row, j] < min) 
            {
                min = array[row, j]; 
            }
        }
    }
    static void FindMax(in int[,] array, int row, out int max)
    {
        max = int.MinValue; 
        for (int j = 0; j < array.GetLength(1); j++) 
        {
            if (array[row, j] > max) 
            {
                max = array[row, j]; 
            }
        }
    }

    static void FindSum(in int[,] array, int row, out int sum)
    {
        sum = 0; 
        for (int j = 0; j < array.GetLength(1); j++) 
        {
            sum += array[row, j]; 
        }
    }
}
