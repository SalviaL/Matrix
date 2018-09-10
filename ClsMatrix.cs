using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Name_Space
{

    /// <summary>
    /// 矩阵类
    /// 提供矩阵定义
    /// 提供矩阵加减
    /// 提供矩阵乘法、数乘、数加、数减矩阵
    /// 提供矩阵的行列式、转置、求逆
    /// 运算直接采用基本的+ - * 运算符即可
    /// 求逆、转置、行列式需要调用方法名
    /// </summary>
    public sealed class Matrix : ObservableCollection<ObservableCollection<double>>
    {

        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount;
        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount; 
        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="index">行号</param>                       
        public void RemoveRow(int index)
        {
            this.RemoveAt(index);
            RowCount--;
        }
        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="index">列号</param>
        public void RemoveColumn(int index)
        {
            for (int i = 0; i < RowCount; i++)
                this[i].RemoveAt(index);
            ColumnCount--;
        }
        /// <summary>
        /// 插入加行（该行全为0）
        /// </summary>
        /// <param name="index">行号</param>
        public void InsertRow(int index)
        {
            this.InsertRow(index);
            RowCount++;
        }
        /// <summary>
        /// 插入列（该列全为0）
        /// </summary>
        /// <param name="index">列号</param>
        public void InsertColumn(int index)
        {
            for (int i = 0; i < RowCount; i++)
                this[i].Insert(index,0);
            ColumnCount++;
        }
       
        public Matrix() { }
        /// <summary>
        /// 生成一个i*j的矩阵
        /// </summary>
        /// <param name="i">行数</param>
        /// <param name="j">列数</param>
        public Matrix(int i, int j)
        {
            for (int ii = 0; ii < i; ii++)
            {
                this.Add(new ObservableCollection<double>());
                for (int jj = 0; jj < j; jj++)
                {
                    this[ii].Add(0);
                }
            }
            RowCount = i;
            ColumnCount = j;
        }
        /// <summary>
        /// 除去第i行，第j列的矩阵（用于求余子式）
        /// </summary>
        /// <param name="i">行数</param>
        /// <param name="j">列数></param>
        /// <param name="M">原矩阵</param>
        private Matrix(int i, int j, Matrix M)
        {
            if (i < M.RowCount && j < M.ColumnCount)
            {
                for (int ii = 0; ii < i-1; ii++)
                {
                    this.Add(new ObservableCollection<double>());
                    for (int jj = 0; jj < j-1; jj++)
                    {
                        this[ii].Add(0);
                    }
                }
                int icount = 0;
                for (int ii = 0; ii < M.RowCount; ii++)
                {
                    int jcount = 0;
                    for (int jj = 0; jj < M.ColumnCount; jj++)
                        if (ii != i && jj != j)
                            this[icount][jcount++] = M[ii][jj];
                    if (ii != i)
                        icount++;
                }
            }
            RowCount = M.RowCount - 1;
            ColumnCount = M.ColumnCount - 1;
        }
        /// <summary>
        /// 二维数组转矩阵
        /// </summary>
        /// <param name="M">矩阵</param>
        public static explicit operator Matrix(double[][] M)
        {
            int m = M.GetLength(0);
            int n = M[0].GetLength(0);
            Matrix matrix = new Matrix(m, n);
            for (int i = 0; i < matrix.RowCount; i++)
                for (int j = 0; j < matrix.ColumnCount; j++)
                    matrix[i][j] = M[i][j];
            return matrix;
        }
        /// <summary>
        /// 二维数组转矩阵
        /// </summary>
        /// <param name="M">矩阵</param>
        public static explicit operator Matrix(double[,] M)
        {
            Matrix matrix = new Matrix(M.GetLength(0), M.GetLength(1));
            for (int i = 0; i < matrix.RowCount; i++)
                for (int j = 0; j < matrix.ColumnCount; j++)
                    matrix[i][j] = M[i, j];
            return matrix;
        }
        private static bool tryPlus(Matrix A, Matrix B, out Matrix C)
        {
            if (A.RowCount == B.RowCount && A.ColumnCount == B.ColumnCount)
            {
                C = new Matrix(A.RowCount, A.ColumnCount);
                for (int i = 0; i < A.RowCount; i++)
                    for (int j = 0; j < A.ColumnCount; j++)
                        C[i][j] = A[i][j] + B[i][j];
                return true;
            }
            else
            {
                C = new Matrix();
                return false;
            }
        }
        private static bool tryMinus(Matrix A, Matrix B, out Matrix C)
        {
            return tryPlus(A, -B, out C);
        }
        private static bool tryMulti(Matrix A, Matrix B, out Matrix C)
        {
            if (A.ColumnCount == B.RowCount)
            {
                C = new Matrix(A.RowCount, B.ColumnCount);
                for (int i = 0; i < C.RowCount; i++)
                    for (int j = 0; j < C.ColumnCount; j++)
                    {
                        double temp = 0;
                        for (int r = 0; r < A.ColumnCount; r++)
                            temp += A[i][r] * B[r][j];
                        C[i][j] = temp;
                    }
                return true;
            }
            else
            {
                C = new Matrix();
                return false;
            }
        }
        /// <summary>
        /// 行列式
        /// </summary>
        /// <param name="M">矩阵</param>
        /// <returns>行列式值</returns>
        public static double Det(Matrix M)
        {
            if (M.RowCount == M.ColumnCount)
            {
                if (M.RowCount == 2)
                    return M[0][0] * M[1][1] - M[0][1] * M[1][0];
                else
                {
                    List<Matrix> cofactor = new List<Matrix>();
                    for (int i = 0; i < M.RowCount; i++)
                        cofactor.Add(new Matrix(i, 0, M));

                    double temp = 0;
                    for (int index = 0; index < cofactor.Count; index++)
                        temp += Math.Pow(-1, index) * M[index][0] * Matrix.Det(cofactor[index]);

                    return temp;
                }
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 矩阵转置
        /// </summary>
        /// <param name="M">矩阵</param>
        /// <returns>转置矩阵</returns>
        public static Matrix Transform(Matrix M)
        {
            Matrix C = new Matrix(M.ColumnCount, M.RowCount);
            for (int i = 0; i < C.RowCount; i++)
                for (int j = 0; j < M.ColumnCount; j++)
                    C[i][j] = M[j][i];
            return C;
        }
        /// <summary>
        /// 矩阵求逆
        /// </summary>
        /// <param name="M">矩阵</param>
        /// <returns>逆阵</returns>
        public static Matrix Inverst(Matrix M)
        {
            double detM = Matrix.Det(M);
            if (M.ColumnCount == M.RowCount && Math.Abs(detM) > 1e-8)
            {
                Matrix Inv = new Matrix(M.RowCount, M.ColumnCount);
                for (int i = 0; i < M.RowCount; i++)
                    for (int j = 0; j < M.ColumnCount; j++)
                        Inv[i][j] = Matrix.Det(new Matrix(i, j, M)) * Math.Pow(-1, i + j);
                Inv = Matrix.Transform(Inv);
                Inv /= detM;
                return Inv;
            }
            else
                return new Matrix();
        }
        public static Matrix operator +(Matrix A, Matrix B)
        {
            Matrix C;
            tryPlus(A, B, out C);
            return C;
        }
        public static Matrix operator -(Matrix A)
        {
            Matrix C = new Matrix(A.RowCount, A.ColumnCount);
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    C[i][j] = A[i][j] * (-1);
            return A;
        }
        public static Matrix operator -(Matrix A, Matrix B)
        {
            Matrix C;
            tryMinus(A, B, out C);
            return C;
        }
        public static Matrix operator *(double a, Matrix A)
        {
            Matrix C = new Matrix(A.RowCount, A.ColumnCount);
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    C[i][j] = A[i][j] * a;
            return C;
        }
        public static Matrix operator *(Matrix A, double a)
        {
            Matrix C = new Matrix(A.RowCount, A.ColumnCount);
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColumnCount; j++)
                    C[i][j] = A[i][j] * a;
            return C;
        }
        public static Matrix operator *(Matrix A, Matrix B)
        {
            Matrix C;
            tryMulti(A, B, out C);
            return C;
        }
        public static Matrix operator /(Matrix A, double a)
        {
            return (1.0 / a) * A;
        }
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < RowCount; i++)
            {
                s += this[i][0].ToString("0.000");
                for (int j = 1; j < ColumnCount; j++)
                    s += ' ' + this[i][j].ToString("0.000");
                s += "\r\n";
            }
            return s;
        }
    }
}
