using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.Common
{
    public class collider_object : Notify.NotifyPropertyChanged
    {
        public collider_object() { }
        public collider_object(string name, string data) { this.Name = name; this.Data = data; }

        public bool Write { get => Get<bool>(); set => Set(value); }
        public bool Group
        {
            get => Get<bool>();
            set
            {
                Set(value);
                if (value) Join();
                else Split();
            }
        }
        public string Name { get => Get<string>(); set => Set(value); }
        public string Data { get => Get<string>(); set => Set(value); }

        private string[] Divide(string o, char s) => o.Split(s).ForEach(x => x.Trim());
        private string[] Divide(string o, string s) => o.Split(s).ForEach(x => x.Trim());
        /// <summary>
        /// Capsule split
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Split()
        {
            if (Data == null || Data.Contains(Environment.NewLine) && Data.Contains('&') || !Data.Contains('&')) return;
            if (Data.Contains('|'))
            {
                if (Data.Count(d => d == '|') == 1)
                {
                    string[] buffer = Divide(Data, '|');
                    if (buffer.Length != 2) throw new Exception($"Invalid data structure: {Data}");

                    string[][] medium = buffer.ForEach(x => Divide(x, '&'));
                    if (medium[0].Length != medium[1].Length) throw new Exception($"Invalid data structure: {Data}");

                    string[] output = new string[medium[0].Length];
                    for (int i = 0; i < medium[0].Length; i++)
                    {
                        output[i] = $"{medium[0][i]} | {medium[1][i]}";
                    }
                    //string output = $"{medium[0][0]} | {medium[1][0]}{Environment.NewLine}{medium[0][1]} | {medium[1][1]}";
                    Data = string.Join(Environment.NewLine, output);
                }
                else
                {

                }
            }
            else Data = string.Join(Environment.NewLine, Divide(Data, '&'));
        }
        /// <summary>
        /// Capsule join
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Join()
        {
            if (Data == null || Data.Contains(Environment.NewLine) && Data.Contains('&') || !Data.Contains(Environment.NewLine)) return;
            if (Data.Contains('|'))
            {
                string[] buffer = Divide(Data, Environment.NewLine);
                string[][] medium = buffer.ForEach(x => Divide(x, '|'));
                if (!medium.Aggregate(medium[0].Length == 2, (accu, next) => accu & medium[0].Length == next.Length)) throw new Exception($"Invalid data structure: {Data}");

                string[] out0 = new string[medium.Length];
                for (int i = 0; i < medium.Length; i++)out0[i] = medium[i][0];
                string[] out1 = new string[medium.Length];
                for (int i = 0; i < medium.Length; i++) out1[i] = medium[i][1];

                string data0 = string.Join(" & ", out0);
                string data1 = string.Join(" & ", out1);

                Data = string.Join(" | ", data0, data1);
            }
            else Data = string.Join(" & ", Divide(Data, Environment.NewLine));
        }
    }
}
