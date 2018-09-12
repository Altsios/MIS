using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace IndSet
{
    static class MIS
    {
        static int M = 4;//кол-во потоков
        static List<List<int>> V;
        static int[] C;
        static int[] r;
        static ConcurrentBag<int> S;
        static int cnt = 0;
        static Barrier Phasebar = new Barrier(M, (Phasebar) =>
        {
            cnt = 0;
            //рандомные значения метки 

            for (int i = 0; i < C.Length; i++)
            {
                if (C[i] == 0) r[i] = C.Length;//метка заведомо больше максимальной
                else
                    cnt++;
            }
        });
        //последовательная версия
        public static List<int> SerMis(Graph vertex)
        {
            List<int> S = new List<int>();

            List<List<int>> V = vertex.getAdj();

            for (int i = 0; i < V.Count; i++)
                if (!V[i].HasNeighbors(S))
                    S.Add(i);
            return S;
        }

        //проверка на наличие смежной вершины во множестве
        static bool HasNeighbors(this List<int> v, List<int> S)
        {
            foreach (var neighbor in v)
                if (S.Contains(neighbor)) return true;
            return false;
        }


        //Люби алгоритм
        public static ConcurrentBag<int> RandMIS(Graph vertex)
        {
            S = new ConcurrentBag<int>();
            //записали в C исходные вершины
            V = vertex.getAdj();
            cnt = V.Count;
            //вектор, обозначающий наличие вершины
            C = new int[cnt];
            r = new int[cnt];
            for (int i = 0; i < cnt; i++)
            {
                C[i] = 1;
                r[i] = i;
            }
            //массив поткоов
            Thread[] Th = new Thread[M];
            for (int i = 0; i < M; i++)
            {
                Th[i] = new Thread(Work);
                Th[i].Start(i);
            }
            for (int i = 0; i < M; i++)
            {
                Th[i].Join();
            }
            return S;
        }

        //рандомные метки вершинам
        static void Work(object o)
        {
            int thn = (int)o;
            int len = C.Length;
            while (cnt != 0)
            {
                //ждем, пока все отработают

                for (int v = thn; v < len; v += M)
                {
                    //если вершину удалили
                    if (C[v] == 0) continue;
                    if (rmin(r[v], V[v]))
                    {
                        S.Add(v);
                        Del(v, V[v]);
                    }
                }
                Phasebar.SignalAndWait();
            }
        }


        static bool rmin(int rv, List<int> neighbors)
        {
            foreach (var v in neighbors)
            {
                if (rv > r[v]) return false;
            }
            return true;
        }

        static void Del(int v, List<int> neighbors)
        {
            C[v] = 0;
            foreach (var ver in neighbors)
            {
                C[ver] = 0;
            }
        }
    }
}




