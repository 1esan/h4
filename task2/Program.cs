using System;

class IntArray
{
    
    private int[] data;

    
    public IntArray(int size)
    {
        if (size < 0)
        {
            throw new ArgumentException("Размер массива не может быть отрицательным");
        }
        data = new int[size];
    }

    
    public void InputData()
    {
        for (int i = 0; i < data.Length; i++)
        {
            Console.Write("Введите элемент {0}: ", i);
            data[i] = int.Parse(Console.ReadLine());
        }
    }
    public void InputDataRandom()
    {
        Random random = new Random();
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = random.Next(-100, 100);
        }
    }

    public void Print(int start, int end)
    {
        if (start < 0 || start >= data.Length || end < 0 || end >= data.Length)
        {
            throw new IndexOutOfRangeException("Индекс вне диапозона размерности массива");
        }
        if (start > end)
        {
            throw new ArgumentException("Начальный индекс должен быть меньше или равен конечному индексу.");
        }
        Console.Write("Массив от {0} до {1}: ", start, end);
        for (int i = start; i <= end; i++)
        {
            Console.Write(data[i] + " ");
        }
        Console.WriteLine();
    }

    public void FindValue(int value, out int[] indexes)
    {
        indexes = new int[0];
        int count = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == value)
            {
                Array.Resize(ref indexes, count + 1);
                indexes[count] = i;
                count++;
            }
        }
    }

    public void DelValue(ref int value)
    {
        FindValue(value, out int[] indexes);
        if (indexes.Length == 0)
        {
            return;
        }
        int[] newData = new int[data.Length - indexes.Length];
        int j = 0; 
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] != value)
            {
                newData[j] = data[i];
                j++;
            }
        }
        data = newData;
        value = 0;
    }

    public static int FindMax(in IntArray array)
    {
        if (array.data.Length == 0)
        {
            throw new InvalidOperationException("Массив пуст");
        }
        int max = array.data[0];
        for (int i = 1; i < array.data.Length; i++)
        {
            if (array.data[i] > max)
            {
                max = array.data[i];
            }
        }
        return max;
    }

    public static IntArray Add(in IntArray array1,in IntArray array2)
    {
        if (array1.data.Length != array2.data.Length)
        {
            throw new ArgumentException("Массивы должны иметь одинаковую длину");
        }
        IntArray result = new IntArray(array1.data.Length);
        for (int i = 0; i < result.data.Length; i++)
        {
            result.data[i] = array1.data[i] + array2.data[i];
        }
        return result;
    }

    public void Sort()
    {
        for (int i = 0; i < data.Length - 1; i++)
        {
            for (int j = 0; j < data.Length - i - 1; j++)
            {
                if (data[j] > data[j + 1])
                {
                    int temp = data[j];
                    data[j] = data[j + 1];
                    data[j + 1] = temp;
                }
            }
        }
    }
}
