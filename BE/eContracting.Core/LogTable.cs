using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    public class LogTable
    {
        protected List<string> Headers { get; } = new List<string>();
        protected List<string[]> Rows { get; } = new List<string[]>();
        protected Dictionary<string, int> ColumnSize { get; set; }
        
        public void SetHeaders(params string[] value)
        {
            this.Headers.AddRange(value);
        }

        public void SetValues(params string[] value)
        {
            if (value.Length != this.Headers.Count)
            {
                throw new Exception("Number of headers and values is not the same");
            }

            this.Rows.Add(value.Select(x => x ?? string.Empty).ToArray());
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            var table = new List<string[]>();
            table.Add(this.Headers.ToArray());
            table.AddRange(this.Rows);

            var sizes = new List<int>();
            var columns = table.First().Length;

            for (int i = 0; i < columns; i++)
            {
                sizes.Add(table.Select(x => x[i].Length + 2).Max());
            }

            for (int i = 0; i < table.Count; i++)
            {
                string[] row = table[i];

                for (int y = 0; y < row.Length; y++)
                {
                    builder.Append(row[y].PadRight(sizes[y]));
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
