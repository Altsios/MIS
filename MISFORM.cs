using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace IndSet
{
    public partial class MISform : Form
    {
        //100 150
        Graph _100;
        //1000 1500
        Graph _1000;
        //10000 15000
        Graph _10000;
        //100000 150000
        Graph _100000;
        //1000000 1500000
        Graph _1000000;
        //тип действия
        string action = "";

        public MISform()
        {
            InitializeComponent();
            Serial.Enabled = false;
            Parall.Enabled = false;
            Graphic.Enabled = false;
            //подписка на событие кнопки
            Generate.Click += Generate_Click;
            button_cancel.Click += Button_cancel_Click;
            Serial.Click += Serial_Click;
            Parall.Click += Parall_Click;
            Graphic.Click += Graphic_Click;
            //обработка worker-a
            backgroundWorker1.DoWork += worker_Dowork;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
        }
private void Graphic_Click(object sender, EventArgs e)
        {
            action = "graphic";
            Generate.Enabled = false;
            button_cancel.Enabled = false;
            Serial.Enabled = false;
            Parall.Enabled = false;
            Graphic.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }



        //ассинхронный процесс завершен. Это тоже в основном потоке
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //если ошибка
            if (e.Error != null)
            {
                MessageBox.Show(string.Format("Ошибка: {0}", e.Error.Message));
            }
            else
            {
                string message;
                if (e.Cancelled)
                {
                    Perc.Text = "";
                    progressBar1.Value = 0;
                    message = "Действие отменено";
                }
                else
                    message = "Готово!";
                MessageBox.Show(message);
            }
            Generate.Enabled = true;
            button_cancel.Enabled = true;
            //алгоритмы доступны только если сгенерированы все 5 графов
            if (Graph1000000.TextLength != 0)
            {
                Serial.Enabled = true;
                Parall.Enabled = true;
            }
            //активируем график только если получены значения как для параллельной, так и для последовательной версии 
            if (Res5.TextLength != 0 && textBox5.TextLength != 0) Graphic.Enabled = true;
        }

//изменения в ассинхр процессе
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //вытащили текст
            string txt = (string)((object[])e.UserState)[0];
            int PCNT = e.ProgressPercentage;
            double time;
            int cnt;
            switch (action)
            {
                case "generate":
                    switch (PCNT)
                    {
                        case 20:
                            Graph100.Text += txt;
                            break;
                        case 40:
                            Graph1000.Text += txt;
                            break;
                        case 60:
                            Graph10000.Text += txt;
                            break;
                        case 80:
                            Graph100000.Text += txt;
                            break;
                        case 100:
                            Graph1000000.Text += txt;
                            break;
                    }
                    break;
                case "serial":
                   time = (double)((object[])e.UserState)[1];
                   cnt= (int)((object[])e.UserState)[2];
                    switch (PCNT)
                    {
                        case 20:
                            Res1.Text += txt;
                            Time1.Text += time.ToString();
                            CNT1.Text += cnt.ToString();
                            break;
                        case 40:
                            Res2.Text += txt;
                            Time2.Text += time.ToString();
                            CNT2.Text += cnt.ToString();
                            break;
                        case 60:
                            Res3.Text += txt;
                            Time3.Text += time.ToString();
                            CNT3.Text += cnt.ToString();
                            break;
                        case 80:
                            Res4.Text += txt;
                            Time4.Text += time.ToString();
                            CNT4.Text += cnt.ToString();
                            break;
                        case 100:
                            Res5.Text += txt;
                            Time5.Text += time.ToString();
                            CNT5.Text += cnt.ToString();
                            break;
                    }
                    break;
                case "parallel":
                    time = (double)((object[])e.UserState)[1];
                    cnt = (int)((object[])e.UserState)[2];
                    switch (PCNT)
                    {
                        case 20:
                            textBox1.Text += txt;
                            label21.Text += cnt.ToString();
                            label26.Text += time.ToString();
                            break;
                        case 40:
                            textBox2.Text += txt;
                            label22.Text += cnt.ToString();
                            label27.Text += time.ToString();
                            break;
                        case 60:
                            textBox3.Text += txt;
                            label23.Text += cnt.ToString();
                            label28.Text += time.ToString();
                            break;
                        case 80:
                            textBox4.Text += txt;
                            label24.Text += cnt.ToString();
                            label29.Text += time.ToString();
                            break;
                        case 100:
                            textBox5.Text += txt;
                            label25.Text += cnt.ToString();
                            label30.Text += time.ToString();
                            break;
                    }
                    break;
            }
            Perc.Text = (PCNT).ToString() + "%";
            progressBar1.Value = PCNT;
        }
     //возникает при запуске асинхронного фонового процесса
        private void worker_Dowork(object sender, DoWorkEventArgs e)
        {
            //подсчет графов
            BackgroundWorker WorkerGraph = new BackgroundWorker();//для работы с графами
            //выбор действия
            //если action
            if (action == "generate")
            {
                    DoWork();//генерация графов
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true; //показывает, что отменена, а не исключение
                    return;
                    }
                    while (progressBar1.Value != 100) { }
            }
            else if(action=="graphic")
            {
                Graphic gr = new Graphic(Time1.Text, Time2.Text, Time3.Text, Time4.Text, Time5.Text, label26.Text, label27.Text, label28.Text, label29.Text, label30.Text);
                gr.ShowDialog();
            }
            //если serial||parallel
            else
            {
                DoWorkAlgor();
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true; //показывает, что отменена, а не исключение
                    return;
                }
                while (progressBar1.Value != 100) { }
                }
            


        }


        private void FillLists(int old)
        {
            List<List<int>> AdjList=new List<List<int>>();
            string txt = "";
            int idx = 0;

            switch(old)
            {
                case 20:
                    AdjList = _100.getAdj();
                    break;
                case 40:
                    AdjList = _1000.getAdj();
                    break;
                case 60:
                    AdjList = _10000.getAdj();
                    break;
                case 80:
                    AdjList = _100000.getAdj();
                    break;
                default:
                    AdjList = _1000000.getAdj();
                    break;
            }

            //20, 40 или 60-100
            if(old==20||old==40)
            {
                for (int i = 0; i < AdjList.Count; i++)
                {
                    txt += string.Format("[{0}] | ", i.ToString());
                    foreach (var item in AdjList[i])
                        txt += string.Format("{0}->", item.ToString());
                    idx = txt.LastIndexOf("->");
                    if (idx != -1)
                        txt = txt.Substring(0, idx);
                    txt += Environment.NewLine;
                }
            }
            else
            {
                Thread[] th = new Thread[4];
                int cnt = AdjList.Count;
                string[] arr = new string[cnt];
                for (int i = 0; i < 4; i++)
                {
                    th[i] = new Thread(Work);
                    th[i].Start(new object[] { i, cnt, AdjList, arr });
                }

                for (int i = 0; i < 4; i++)
                    th[i].Join();

                foreach (var item in arr)
                    txt += item + Environment.NewLine;
            }

            //после того, как сделали список
            backgroundWorker1.ReportProgress(old,new object[] { txt });
        }
private void Work(object o)
        {
            int idx = 0;
            int th = (int)((object[])o)[0];
            int cnt = (int)((object[])o)[1];
            List<List<int>> AdjList = (List<List<int>>)((object[])o)[2];
            string[] arr = (string[])((object[])o)[3];

            for (int i = th; i < cnt; i += 4)
            {
                arr[i] += string.Format("[{0}] | ", i.ToString());
                foreach (var item in AdjList[i])
                    arr[i] += string.Format("{0}->", item.ToString());
                idx = arr[i].LastIndexOf("->");
                if (idx != -1)
                    arr[i] = arr[i].Substring(0,idx);
            }
        }

         private void DoWorkAlgor()
        {
            //последовательная или параллельная версия?
            DateTime d1, d2;
            List<int> S = new List<int>();
            double time;
            switch (action)
            {
                case "serial":
                    d1 = DateTime.Now;
                    S = MIS.SerMis(_100);
                    d2 = DateTime.Now;
                    time=(d2 - d1).TotalMilliseconds;
                    FillListsAlg(20,S,time);
                    if (backgroundWorker1.CancellationPending) return;
                    d1 = DateTime.Now;
                    S = MIS.SerMis(_1000);
                    d2 = DateTime.Now;
                    time = (d2 - d1).TotalMilliseconds;
                    FillListsAlg(40, S, time);
                    if (backgroundWorker1.CancellationPending) return;
                    d1 = DateTime.Now;
                    S =  MIS.SerMis(_10000);
                    d2 = DateTime.Now;
                    time = (d2 - d1).TotalMilliseconds;
                    FillListsAlg(60, S, time);
                    if (backgroundWorker1.CancellationPending) return;
                    d1 = DateTime.Now;
                    S =  MIS.SerMis(_100000);
                    d2 = DateTime.Now;
                    time = (d2 - d1).TotalMilliseconds;
                    FillListsAlg(80, S, time);
                    if (backgroundWorker1.CancellationPending) return;
                    d1 = DateTime.Now;
                    S =  MIS.SerMis(_1000000);
                    d2 = DateTime.Now;
                    time = (d2 - d1).TotalMilliseconds;
                    FillListsAlg(100, S, time);
                    if (backgroundWorker1.CancellationPending) return;
                    break;
                case "parallel":
                    //список для слива результата
                    ConcurrentBag<int> tmp = new ConcurrentBag<int>();
                    d1 = DateTime.Now;
                    tmp = MIS.RandMIS(_100);
                    d2 = DateTime.Now;
                    S = new List<int>();
                    foreach (var item in tmp)
                        S.Add(item);
                    time = (d2 - d1).TotalMilliseconds;
                    FillListsAlg(20, S, time);
                    if (backgroundWorker1.CancellationPending) return;
                    d1 = DateTime.Now;
                    tmp = MIS.RandMIS(_1000);
                    d2 = DateTime.Now;
                    S = new List<int>();
                    foreach (var item in tmp)
                        S.Add(item);
                    time = (d2 - d1).TotalMilliseconds;
                    FillListsAlg(40, S, time);
                    if (backgroundWorker1.CancellationPending) return;
                    d1 = DateTime.Now;
                    tmp = MIS.RandMIS(_10000);
                    d2 = DateTime.Now;
                    S = new List<int>();
                    foreach (var item in tmp)
                        S.Add(item);
                    time=(d2 - d1).TotalMilliseconds;
                    FillListsAlg(60, S, time);
                    if (backgroundWorker1.CancellationPending) return;
                    d1 = DateTime.Now;
                    tmp = MIS.RandMIS(_100000);
                    d2 = DateTime.Now;
                    S = new List<int>();
                    foreach (var item in tmp)
                        S.Add(item);
                    time = (d2 - d1).TotalMilliseconds;
                    FillListsAlg(80, S, time);
                    if (backgroundWorker1.CancellationPending) return;
                    d1 = DateTime.Now;
                    tmp = MIS.RandMIS(_1000000);
                    d2 = DateTime.Now;
                    S = new List<int>();
                    foreach (var item in tmp)
                        S.Add(item);
                    time= (d2 - d1).TotalMilliseconds;
                    FillListsAlg(100, S, time);
                    if (backgroundWorker1.CancellationPending) return;
                    break;
            }

        }
        //заполнение списка независимых множеств
        private void FillListsAlg(int old, List<int> S,double time)
        {
            string txt = "";

            if (old == 20 || old == 40)
            {
                //создаем строку с элементами множества
                foreach (var item in S)
                    txt += string.Format("{0}, ", item.ToString());
            }
            else
            {
                Thread[] th = new Thread[4];
                string[] arr = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    th[i] = new Thread(WorkAlg);
                    th[i].Start(new object[] { i,arr,S});
                }
                for (int i = 0; i < 4; i++)
                    th[i].Join();

                foreach (var item in arr)
                    txt += item;
            }
            int idx = 0;
            idx = txt.LastIndexOf(",");
            if(idx!=-1)
                txt = txt.Substring(0,idx);
            //после того, как сделали список
            backgroundWorker1.ReportProgress(old, new object[] { txt, time,S.Count});


        }
private void WorkAlg(object o)
        {
            int th = (int)((object[])o)[0];
            string[] arr = (string[])((object[])o)[1];
            List<int> S= (List<int>)((object[])o)[2];

            for (int j = th; j <S.Count; j+=4)
                arr[th] += string.Format("{0}, ", S[j].ToString());
        }

        //работа для  WorkerGraph-генерация 5 графов(создание)
        //private void WorkerGraph_DoWork(object sender, DoWorkEventArgs e)
        private void DoWork()
        {

            _100 = new Graph(int.Parse(V1.Text), int.Parse(E1.Text));
            if (backgroundWorker1.CancellationPending) return;

            FillLists(20);

            _1000 = new Graph(int.Parse(V2.Text), int.Parse(E2.Text));
            if (backgroundWorker1.CancellationPending) return;
            FillLists(40);
     
            _10000 = new Graph(int.Parse(V3.Text), int.Parse(E3.Text));
            if (backgroundWorker1.CancellationPending) return;
            FillLists(60);
        
            _100000 = new Graph(int.Parse(V4.Text), int.Parse(E4.Text));
            if (backgroundWorker1.CancellationPending) return;
            FillLists(80);
         
            _1000000 = new Graph(int.Parse(V5.Text), int.Parse(E5.Text));
            if (backgroundWorker1.CancellationPending) return;
            FillLists(100);
        }


        //отмена расчетов или генерации
        private void Button_cancel_Click(object sender, EventArgs e)
        {
            //отмена фоновой операции
            backgroundWorker1.CancelAsync();
        }

        //это выполняется в сосновном потоке
        private void Generate_Click(object sender, EventArgs e)
        {
            action = "generate";
            //откл возможность клика 
            Perc.Text = "";
            progressBar1.Value = 0;
            Graph100.Text = "";
            Graph1000.Text = "";
            Graph10000.Text = "";
            Graph100000.Text = "";
            Graph1000000.Text = "";
            if (V1.TextLength == 0 || E1.TextLength == 0 || V2.TextLength == 0 || E2.TextLength == 0 ||
            V3.TextLength == 0 || E3.TextLength == 0 || V4.TextLength == 0 || E4.TextLength == 0 ||
            V5.TextLength == 0 || E5.TextLength == 0)
                throw new Exception("Ошибка: Введены не все параметры");
            Generate.Enabled = false;
            Serial.Enabled = false;
            Parall.Enabled = false;
            backgroundWorker1.RunWorkerAsync();//запуск кода в обработчике do_work, выполняется в отдельном потоке
        }

        private void Serial_Click(object sender, EventArgs e)
        {
            action = "serial";
            Perc.Text = "";
            progressBar1.Value = 0;
            Res1.Text = "";
            Res2.Text = "";
            Res3.Text = "";
            Res4.Text = "";
            Res5.Text = "";
            Time1.Text = "Время=";
            Time2.Text = "Время=";
            Time3.Text = "Время=";
            Time4.Text = "Время=";
            Time5.Text = "Время=";
            CNT1.Text = "CNT=";
            CNT2.Text = "CNT=";
            CNT3.Text = "CNT=";
            CNT4.Text = "CNT=";
            CNT5.Text = "CNT=";
            Generate.Enabled = false;
            Serial.Enabled = false;
            Parall.Enabled = false;
            Graphic.Enabled = false;
            backgroundWorker1.RunWorkerAsync();//запуск кода в обработчике do_work, выполняется в отдельном потоке
        }

        private void Parall_Click(object sender, EventArgs e)
        {
            action = "parallel";
            Perc.Text = "";
            progressBar1.Value = 0;
            textBox1.Text = "";
            label21.Text = "CNT=";
            label26.Text = "Время=";
            textBox2.Text = "";
            label22.Text = "CNT=";
            label27.Text = "Время=";
            textBox3.Text = "";
            label23.Text = "CNT=";
            label28.Text = "Время=";
            textBox4.Text = "";
            label24.Text = "CNT=";
            label29.Text = "Время=";
            textBox5.Text = "";
            label25.Text = "CNT=";
            label30.Text = "Время=";
            Generate.Enabled = false;
            Serial.Enabled = false;
            Parall.Enabled = false;
            Graphic.Enabled = false;
            backgroundWorker1.RunWorkerAsync();//запуск кода в обработчике do_work, выполняется в отдельном потоке
        }
    }
}
