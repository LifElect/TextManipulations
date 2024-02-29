using System;
using System.Linq;

class MatrixMatchException : Exception
{
    public MatrixMatchException(string message) : base(message){ 
    }

}

class MatrixInverseException : Exception
{
    public MatrixInverseException(string message) : base(message) {
    }
}

class SquareMatrix : ICloneable, IComparable<SquareMatrix>, IEquatable<SquareMatrix>
{
    private readonly int size;
    private int[,] matrix;

    public SquareMatrix(int size)
    {
        this.size = size;
        matrix = new int[size, size];
        Random rand = new Random();

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                matrix[currentNumberX, currentNumberY] = rand.Next(10);
            }
        }
    }

    private SquareMatrix(int[,] values)
    {
        size = values.GetLength(0);
        matrix = new int[size, size];

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                matrix[currentNumberX, currentNumberY] = values[currentNumberX, currentNumberY];
            }
        }
    }

    public static SquareMatrix operator +(SquareMatrix m1, SquareMatrix m2)
    {
        if (m1.size != m2.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int[,] result = new int[m1.size, m1.size];

        for (int currentNumberX = 0; currentNumberX < m1.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m1.size; ++currentNumberY)
            {
                result[currentNumberX, currentNumberY] = m1.matrix[currentNumberX, currentNumberY] + m2.matrix[currentNumberX, currentNumberY];
            }
        }

        return new SquareMatrix(result);
    }

    public static SquareMatrix operator *(SquareMatrix m1, SquareMatrix m2)
    {
        if (m1.size != m2.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int[,] result = new int[m1.size, m1.size];

        for (int currentNumberX = 0; currentNumberX < m1.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m1.size; ++currentNumberY)
            {
                for (int k = 0; k < m1.size; ++k)
                {
                    result[currentNumberX, currentNumberY] += m1.matrix[currentNumberX, k] * m2.matrix[k, currentNumberY];
                }
            }
        }

        return new SquareMatrix(result);
    }

    public static bool operator >(SquareMatrix m1, SquareMatrix m2)
    {
        if (m1.size != m2.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int sum1 = m1.matrix.Cast<int>().Sum();
        int sum2 = m2.matrix.Cast<int>().Sum();

        return sum1 > sum2;
    }

    public static bool operator <(SquareMatrix m1, SquareMatrix m2)
    {
        if (m1.size != m2.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int sum1 = m1.matrix.Cast<int>().Sum();
        int sum2 = m2.matrix.Cast<int>().Sum();

        return sum1 < sum2;
    }

    public static bool operator >=(SquareMatrix m1, SquareMatrix m2)
    {
        return m1 == m2 || m1 > m2;
    }

    public static bool operator <=(SquareMatrix m1, SquareMatrix m2)
    {
        return m1 == m2 || m1 < m2;
    }

    public static bool operator ==(SquareMatrix m1, SquareMatrix m2)
    {
        if (ReferenceEquals(m1, m2))
        {
            return true;
        }

        if (m1 is null || m2 is null)
        {
            return false;
        }

        if (m1.size != m2.size)
        {
            return false;
        }

        for (int currentNumberX = 0; currentNumberX < m1.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m1.size; ++currentNumberY)
            {
                if (m1.matrix[currentNumberX, currentNumberY] != m2.matrix[currentNumberX, currentNumberY])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool operator !=(SquareMatrix m1, SquareMatrix m2)
    {
        return !(m1 == m2);
    }

    public static explicit operator int(SquareMatrix m)
    {
        int sum = 0;

        for (int currentNumberX = 0; currentNumberX < m.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m.size; ++currentNumberY)
            {
                sum += m.matrix[currentNumberX, currentNumberY];
            }
        }

        return sum;
    }

    public static implicit operator bool(SquareMatrix m)
    {
        for (int currentNumberX = 0; currentNumberX < m.size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < m.size; ++currentNumberY)
            {
                if (m.matrix[currentNumberX, currentNumberY] != 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int[,] GetMatrix()
    {
        return matrix;
    }

    public double Determinant()
    {
        if (size == 1)
        {
            return matrix[0, 0];
        }

        if (size == 2)
        {
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        }

        int det = 0;

        for (int detNumber = 0; detNumber < size; ++detNumber)
        {
            int[,] subMatrix = new int[size - 1, size - 1];

            for (int currentNumberX = 1; currentNumberX < size; ++currentNumberX)
            {
                for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
                {
                    if (currentNumberY < detNumber)
                    {
                        subMatrix[currentNumberX - 1, currentNumberY] = matrix[currentNumberX, currentNumberY];
                    }
                    else if (currentNumberY > detNumber)
                    {
                        subMatrix[currentNumberX - 1, currentNumberY - 1] = matrix[currentNumberX, currentNumberY];
                    }
                }
            }

            det += Convert.ToInt32(Math.Pow(-1, detNumber) * matrix[0, detNumber] * new SquareMatrix(subMatrix).Determinant());
        }

        return det;
    }

    public SquareMatrix Inverse()
    {
        if (Determinant() == 0)
        {
            throw new MatrixInverseException("Matrix is not invertible");
        }
        int[,] inverse = new int[size, size];
        int[,] TransponseMatrix = new int[size,size];
    
        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                if (currentNumberX != currentNumberY)
                {
                    TransponseMatrix[currentNumberX, currentNumberY] = matrix[currentNumberY,currentNumberX];
                }
                else if(currentNumberX == currentNumberY)
                {
                    TransponseMatrix[currentNumberX, currentNumberY] = matrix[currentNumberX, currentNumberY];
                }
            }
        }
        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                inverse[currentNumberX, currentNumberY] =  Convert.ToInt32(Math.Pow(-1, Determinant()) * TransponseMatrix[currentNumberX, currentNumberY]);
            }
        }

                return new SquareMatrix(inverse);
    }

    public override string ToString()
    {
        string result = "";

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                result += matrix[currentNumberX, currentNumberY] + " ";
            }

            result += Environment.NewLine;
        }

        return result;
    }

    public override int GetHashCode()
    {
        int hash = size.GetHashCode();

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                hash ^= matrix[currentNumberX, currentNumberY].GetHashCode();
            }
        }

        return hash;
    }

    public override bool Equals(object obj)
    {
        if (obj is SquareMatrix otherMatrix)
        {
            return Equals(otherMatrix);
        }

        return false;
    }

    public bool Equals(SquareMatrix other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null)
        {
            return false;
        }

        if (size != other.size)
        {
            return false;
        }

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberY = 0; currentNumberY < size; ++currentNumberY)
            {
                if (matrix[currentNumberX, currentNumberY] != other.matrix[currentNumberX, currentNumberY])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int CompareTo(SquareMatrix other)
    {
        if (other is null)
        {
            return 1;
        }

        if (size != other.size)
        {
            throw new MatrixMatchException("Matrix sizes do not match");
        }

        int sum1 = matrix.Cast<int>().Sum();
        int sum2 = other.matrix.Cast<int>().Sum();

        return sum1.CompareTo(sum2);
    }

    public object Clone()
    {
        int[,] cloneMatrix = new int[size, size];

        for (int currentNumberX = 0; currentNumberX < size; ++currentNumberX)
        {
            for (int currentNumberSecond = 0; currentNumberSecond < size; ++currentNumberSecond)
            {
                cloneMatrix[currentNumberX, currentNumberSecond] = matrix[currentNumberX, currentNumberSecond];
            }
        }

        return new SquareMatrix(cloneMatrix);
    }
}

class MatrixCalculator
{
    static void Main()
    {
        try
        {
            SquareMatrix m1 = new SquareMatrix(3);
            SquareMatrix m2 = new SquareMatrix(3);

            Console.WriteLine("Matrix 1:");
            Console.Write(m1.ToString());

            Console.WriteLine("Matrix 2:");
            Console.Write(m2.ToString());

            SquareMatrix m3 = m1 + m2;
            Console.WriteLine("Sum of Matrix 1 and Matrix 2:");
            Console.Write(m3.ToString());

            SquareMatrix m4 = m1 * m2;
            Console.WriteLine("Product of Matrix 1 and Matrix 2:");
            Console.Write(m4.ToString());

            bool greater = m1 > m2;
            Console.WriteLine("Matrix 1 > Matrix 2: " + greater);

            bool less = m1 < m2;
            Console.WriteLine("Matrix 1 < Matrix 2: " + less);

            bool greaterOrEqual = m1 >= m2;
            Console.WriteLine("Matrix 1 >= Matrix 2: " + greaterOrEqual);

            bool lessOrEqual = m1 <= m2;
            Console.WriteLine("Matrix 1 <= Matrix 2: " + lessOrEqual);

            bool equal = m1 == m2;
            Console.WriteLine("Matrix 1 == Matrix 2: " + equal);

            bool notEqual = m1 != m2;
            Console.WriteLine("Matrix 1 != Matrix 2: " + notEqual);

            int sum = (int)m1;
            Console.WriteLine("Sum of elements in Matrix 1: " + sum);

            bool isNonZero = m1;
            Console.WriteLine("Matrix 1 is non-zero: " + isNonZero);

            double determinant = m1.Determinant();
            Console.WriteLine("Determinant of Matrix 1: " + determinant);

            SquareMatrix inverse = m1.Inverse();
            Console.WriteLine("Inverse of Matrix 1:");
            Console.Write(inverse.ToString());

            SquareMatrix clone = (SquareMatrix)m1.Clone();
            Console.WriteLine("Clone of Matrix 1:");
            Console.Write(clone.ToString());

            bool equalsClone = m1.Equals(clone);
            Console.WriteLine("Matrix 1 equals to its clone: " + equalsClone);

            int hashCode = m1.GetHashCode();
            Console.WriteLine("HashCode of Matrix 1: " + hashCode);
        }
        catch (MatrixMatchException ex)
        {
            Console.WriteLine("MatrixException: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }

        Console.ReadKey();
    }
}