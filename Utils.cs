using System;
using System.Collections.Generic;
using System.Linq;

namespace SATSP
{
    public static class Utils
    {
        private static readonly Random random = new();
        public static double GetTotalDistance(this IEnumerable<Node> data) =>
            data.Zip(data.Skip(1).Append(data.First()))
            .Select(v => v.First - v.Second)
            .Sum();

        public static IOrderedEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
        {
            var r = new Random();
            return source.OrderBy(x => r.Next());
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T element)
        {
            var result = -1;
            var index = 0;
            foreach (var i in source)
            {
                if (i?.Equals(element) ?? Equals(i, element))
                {
                    result = index;
                    break;
                }
                index++;
            }
            return result;
        }

        public static Node[] Transform(Node[] source) =>
            (random.Next() % 5) switch
            {
                0 => Transform1(source),
                1 => Transform2(source),
                2 => Transform3(source),
                3 => Transform4(source),
                4 => Transform5(source),
                _ => throw new InvalidOperationException("Invalid transform algorithm.")
            };

        // 随机交换两个点的位置
        public static Node[] Transform1(Node[] source)
        {
            int index1, index2;
            do
            {
                index1 = random.Next() % source.Length;
                index2 = random.Next() % source.Length;
            } while (index1 == index2);
            var newData = new Node[source.Length];
            source.CopyTo(newData, 0);
            (newData[index1], newData[index2]) = (newData[index2], newData[index1]);
            return newData;
        }

        // 随机将一段序列放到另一个随机点后面
        public static Node[] Transform2(Node[] source)
        {
            int u, v, w;
            do
            {
                u = random.Next() % source.Length;
                v = random.Next() % source.Length;
                w = random.Next() % source.Length;
            } while (u == v || v == w || u == w);
            if (u > v) (u, v) = (v, u);
            if (v > w) (v, w) = (w, v);
            if (u > v) (u, v) = (v, u);
            var newData = new Node[source.Length];
            for (var i = 0; i < u; i++) newData[i] = source[i];
            for (var i = v; i < w; i++) newData[u - v + i] = source[i];
            for (var i = u; i < v; i++) newData[w - v + i] = source[i];
            for (var i = w; i < source.Length; i++) newData[i] = source[i];
            return newData;
        }

        // 随机将一段序列反转
        public static Node[] Transform3(Node[] source)
        {
            int index1, index2;
            do
            {
                index1 = random.Next() % source.Length;
                index2 = random.Next() % source.Length;
            } while (index1 == index2);

            if (index1 > index2) (index1, index2) = (index2, index1);
            var newData = new Node[source.Length];
            source.CopyTo(newData, 0);
            for (int i = index1, j = index2; i < j; i++, j--)
            {
                (newData[i], newData[j]) = (newData[j], newData[i]);
            }
            return newData;
        }

        // 随机找到两点，将两个点的左右两个序列调转
        public static Node[] Transform4(Node[] source)
        {
            int index1, index2;
            do
            {
                index1 = random.Next() % source.Length;
                index2 = random.Next() % source.Length;
            } while (index1 == index2);
            if (index1 > index2) (index1, index2) = (index2, index1);
            var newData = new Node[source.Length];
            source.CopyTo(newData, 0);
            var index = 0;
            for (var i = index2; i < source.Length; i++)
            {
                newData[index++] = source[i];
            }
            for (var i = index1 + 1; i < index2; i++)
            {
                newData[index++] = source[i];
            }
            for (var i = 0; i <= index1; i++)
            {
                newData[index++] = source[i];
            }
            return newData;
        }

        // 随机将一段序列放到头部
        public static Node[] Transform5(Node[] source)
        {
            int index1, index2;
            do
            {
                index1 = random.Next() % source.Length;
                index2 = random.Next() % source.Length;
            } while (index1 == index2);
            if (index1 > index2) (index1, index2) = (index2, index1);
            var newData = new Node[source.Length];
            source.CopyTo(newData, 0);
            newData[0] = source[index1];
            newData[1] = source[index2];
            var index = 2;
            for (var i = 0; i < source.Length; i++)
            {
                if (i == index1 || i == index2) continue;
                newData[index++] = source[i];
            }
            return newData;
        }
    }
}
