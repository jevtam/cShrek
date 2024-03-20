using System;
using System.Text;

public class SquareMatrix : ICloneable, IComparable<SquareMatrix>
{
    private int[,] _matrix;

    public int Size { get; private set; }

    public SquareMatrix(int size, Random random)
    {
        Size = size;
        _matrix = new int[Size, Size];

        for (int row = 0; row < Size; ++row)
        {
            for (int col = 0; col < Size; ++col)
            {
                _matrix[row, col] = random.Next(1, 100);
            }
        }
    }

    public SquareMatrix(int size)
    {
        Size = size;
        _matrix = new int[Size, Size];
        Random random = new Random();

        for (int row = 0; row < Size; ++row)
        {
            for (int col = 0; col < Size; ++col)
            {
                _matrix[row, col] = random.Next(1, 100);
            }
        }
    }

    public SquareMatrix(int[,] data)
    {
        Size = (int)Math.Sqrt(data.Length);
        _matrix = new int[Size, Size];
        Array.Copy(data, _matrix, data.Length);
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();

        for (int row = 0; row < Size; ++row)
        {
            for (int col = 0; col < Size; ++col)
            {
                stringBuilder.Append(_matrix[row, col] + "\t");
            }
            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    public int CompareTo(SquareMatrix other)
    {
        if (other == null)
        {
            return 1;
        }

        int sum1 = GetSum();
        int sum2 = other.GetSum();

        return sum1.CompareTo(sum2);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is SquareMatrix))
        {
            return false;
        }

        SquareMatrix other = (SquareMatrix)obj;

        if (this.Size != other.Size)
        {
            return false;
        }

        for (int row = 0; row < Size; ++row)
        {
            for (int col = 0; col < Size; ++col)
            {
                if (this._matrix[row, col] != other._matrix[row, col])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + Size.GetHashCode();

        for (int row = 0; row < Size; ++row)
        {
            for (int col = 0; col < Size; ++col)
            {
                hash = hash * 23 + _matrix[row, col].GetHashCode();
            }
        }

        return hash;
    }

    public object Clone()
    {
        int[,] clonedData = new int[Size, Size];
        Array.Copy(_matrix, clonedData, _matrix.Length);
        return new SquareMatrix(clonedData);
    }

    public int GetSum()
    {
        int sum = 0;
        for (int row = 0; row < Size; ++row)
        {
            for (int col = 0; col < Size; ++col)
            {
                sum += _matrix[row, col];
            }
        }
        return sum;
    }

    public static SquareMatrix operator +(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        if (matrix1.Size != matrix2.Size)
        {
            throw new MatrixCalculatorException("Размеры матрицы несовместимы для добавления.");
        }

        int[,] result = new int[matrix1.Size, matrix1.Size];

        for (int row = 0; row < matrix1.Size; ++row)
        {
            for (int col = 0; col < matrix1.Size; ++col)
            {
                result[row, col] = matrix1[row, col] + matrix2[row, col];
            }
        }

        return new SquareMatrix(result);
    }

    public static SquareMatrix operator *(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        if (matrix1.Size != matrix2.Size)
        {
            throw new MatrixCalculatorException("Размеры матриц несовместимы для умножения.");
        }

        int[,] result = new int[matrix1.Size, matrix1.Size];

        for (int row = 0; row < matrix1.Size; ++row)
        {
            for (int col = 0; col < matrix1.Size; ++col)
            {
                int sum = 0;
                for (int countThree = 0; countThree < matrix1.Size; ++countThree)
                {
                    sum += matrix1[row, countThree] * matrix2[countThree, col];
                }
                result[row, col] = sum;
            }
        }

        return new SquareMatrix(result);
    }

    public static bool operator >(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        return matrix1.CompareTo(matrix2) > 0;
    }

    public static bool operator <(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        return matrix1.CompareTo(matrix2) < 0;
    }

    public static bool operator >=(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        return matrix1.CompareTo(matrix2) >= 0;
    }

    public static bool operator <=(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        return matrix1.CompareTo(matrix2) <= 0;
    }

    public static bool operator ==(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        if (ReferenceEquals(matrix1, matrix2))
        {
            return true;
        }
        if (matrix1 is null || matrix2 is null)
        {
            return false;
        }

        return matrix1.Equals(matrix2);
    }

    public static bool operator !=(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        return !(matrix1 == matrix2);
    }

    public static explicit operator bool(SquareMatrix matrix)
    {
        return matrix != null && matrix.Size > 0;
    }

    public static double Determinant(SquareMatrix matrix)
    {
        if (matrix.Size == 1)
        {
            return matrix[0, 0];
        }

        if (matrix.Size == 2)
        {
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        }

        double determinant = 0;
        for (int count = 0; count < matrix.Size; ++count)
        {
            determinant += Math.Pow(-1, count) * matrix[0, count] * Determinant(matrix.GetMinor(0, count));
        }

        return determinant;
    }

    public static SquareMatrix Inverse(SquareMatrix matrix)
    {
        double determinant = Determinant(matrix);
        if (determinant == 0)
            throw new MatrixCalculatorException("Матрица вырожденная, не удается найти обратную.");

        SquareMatrix inverse = new SquareMatrix(matrix.Size);

        for (int countOne = 0; countOne < matrix.Size; ++countOne)
        {
            for (int countTwo = 0; countTwo < matrix.Size; ++countTwo)
            {
                inverse[countTwo, countOne] = (int)(Math.Pow(-1, countOne + countTwo) * Determinant(matrix.GetMinor(countOne, countTwo)) / determinant);
            }
        }

        return inverse;
    }

    private SquareMatrix GetMinor(int row, int col)
    {
        int[,] minor = new int[Size - 1, Size - 1];
        for (int countOne = 0, countThree = 0; countOne < Size; ++countOne)
        {
            if (countOne == row)
            {
                continue;
            }

            for (int countTwo = 0, countFour = 0; countTwo < Size; ++countTwo)
            {
                if (countTwo == col)
                {
                    continue;
                }

                minor[countThree, countFour] = _matrix[countOne, countTwo];
                ++countFour;
            }
            ++countThree;
        }

        return new SquareMatrix(minor);
    }

    public static bool IsIdentityMatrix(SquareMatrix matrix)
    {
        for (int countOne = 0; countOne < matrix.Size; ++countOne)
        {
            for (int countTwo = 0; countTwo < matrix.Size; ++countTwo)
            {
                if (countOne == countTwo && matrix[countOne, countTwo] != 1)
                {
                    return false;
                }
                if (countOne != countTwo && matrix[countOne, countTwo] != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int this[int row, int col]
    {
        get { return _matrix[row, col]; }
        set { _matrix[row, col] = value; }
    }

    public SquareMatrix Transpose()
    {
        int[,] transposed = new int[Size, Size];
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                transposed[col, row] = _matrix[row, col];
            }
        }
        return new SquareMatrix(transposed);
    }

    public SquareMatrix Diagonalize(SquareMatrix matrix)
    {
        if (matrix.Size != 3)
        {
            throw new MatrixCalculatorException("Метод diagonalize поддерживает только матрицы размером 3x3.");
        }

        // Создаем диагональную матрицу с элементами равными следу и остальными элементами равными 0
        int[,] diagonalized = new int[matrix.Size, matrix.Size];
        int trace = matrix.Trace();
        for (int i = 0; i < matrix.Size; i++)
        {
            diagonalized[i, i] = trace;
        }

        return new SquareMatrix(diagonalized);
    }


    public int Trace()
    {
        int trace = 0;
        for (int i = 0; i < Size; i++)
        {
            trace += _matrix[i, i];
        }
        return trace;
    }

    public delegate SquareMatrix DiagonalizeDelegate(SquareMatrix matrix);
}

public class MatrixCalculatorException : Exception
{
    public MatrixCalculatorException(string message) : base(message) { }
}

public abstract class CalculationHandler
{
    protected CalculationHandler successor;

    public void SetSuccessor(CalculationHandler successor)
    {
        this.successor = successor;
    }

    public abstract void HandleRequest(int choice, SquareMatrix matrix);
}

public class TransposeHandler : CalculationHandler
{
    public override void HandleRequest(int choice, SquareMatrix matrix)
    {
        if (choice == 1)
        {
            // Обработка запроса транспонирования матрицы
            SquareMatrix transposedMatrix = matrix.Transpose();
            Console.WriteLine("Транспонированная матрица:");
            Console.WriteLine(transposedMatrix);
        }
        else
        {
            // Передача запроса следующему обработчику в цепочке
            if (successor != null)
            {
                successor.HandleRequest(choice, matrix);
            }
        }
    }
}

public class TraceHandler : CalculationHandler
{
    public override void HandleRequest(int choice, SquareMatrix matrix)
    {
        if (choice == 2)
        {
            // Обработка запроса нахождения следа матрицы
            int trace = matrix.Trace();
            Console.WriteLine("След матрицы: " + trace);
        }
        else
        {
            // Передача запроса следующему обработчику в цепочке
            if (successor != null)
            {
                successor.HandleRequest(choice, matrix);
            }
        }
    }
}

public class DiagonalizeHandler : CalculationHandler
{
    private SquareMatrix.DiagonalizeDelegate diagonalizeDelegate;

    public DiagonalizeHandler(SquareMatrix.DiagonalizeDelegate diagonalizeDelegate)
    {
        this.diagonalizeDelegate = diagonalizeDelegate;
    }

    public override void HandleRequest(int choice, SquareMatrix matrix)
    {
        if (choice == 3)
        {
            // Обработка запроса приведения матрицы к диагональному виду
            SquareMatrix diagonalizedMatrix = diagonalizeDelegate(matrix);
            Console.WriteLine("Диагонализированная матрица:");
            Console.WriteLine(diagonalizedMatrix);
        }
        else
        {
            // Передача запроса следующему обработчику в цепочке
            if (successor != null)
            {
                successor.HandleRequest(choice, matrix);
            }
        }
    }
}

public class Menu
{
    private CalculationHandler calculationHandler;

    public Menu(CalculationHandler calculationHandler)
    {
        this.calculationHandler = calculationHandler;
    }

    public void Execute(int choice, SquareMatrix matrix)
    {
        calculationHandler.HandleRequest(choice, matrix);
    }
}

public class Program
{
    static void Main(string[] args)
    {
        Random random1 = new Random();
        Random random2 = new Random(DateTime.Now.Millisecond);

        SquareMatrix matrix1 = new SquareMatrix(3, random1);
        SquareMatrix matrix2 = new SquareMatrix(3, random2);

        Console.WriteLine("Матрица 1:");
        Console.WriteLine(matrix1);

        Console.WriteLine("Матрица 2:");
        Console.WriteLine(matrix2);

        CalculationHandler transposeHandler = new TransposeHandler();
        CalculationHandler traceHandler = new TraceHandler();
        CalculationHandler diagonalizeHandler = new DiagonalizeHandler(matrix1.Diagonalize);

        // Установка цепочки обработчиков
        transposeHandler.SetSuccessor(traceHandler);
        traceHandler.SetSuccessor(diagonalizeHandler);

        Menu menu = new Menu(transposeHandler);

        Console.WriteLine("Выберите действие:");
        Console.WriteLine("1. Транспонировать матрицу");
        Console.WriteLine("2. Найти след матрицы");
        Console.WriteLine("3. Привести матрицу к диагональному виду");

        int choice = int.Parse(Console.ReadLine());

        menu.Execute(choice, matrix1);
    }
}
