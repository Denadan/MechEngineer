using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace struct_generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles("template", "*.json");

            foreach (var filename in files)
            {
                Console.WriteLine(filename);
                var name = Path.GetFileName(filename);

                var file = new FileStream(filename, FileMode.Open);
                var reader = new StreamReader(file);

                var cost = int.Parse(reader.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                var add_cost = int.Parse(reader.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);
                var tonage = float.Parse(reader.ReadLine(), System.Globalization.CultureInfo.InvariantCulture);

                var str = reader.ReadToEnd();
                file.Close();

                for (int w = 20; w <= 100; w += 5)
                {
                    Console.WriteLine($"weight: {w}\tcost: {cost}\t tonnage{(w * tonage):F2} ");


                    var target_file = new FileStream("output\\" + name.Replace("^t^", w.ToString(System.Globalization.CultureInfo.InvariantCulture)), FileMode.Create);
                    var output = str.Replace("^t^", w.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    output = output.Replace("^c^", cost.ToString());
                    cost += add_cost;
                    output = output.Replace("^f^", (w * tonage).ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
                    var writer = new StreamWriter(target_file);
                    writer.WriteLine(output);
                    writer.Flush();
                    target_file.Close();
                }
            }
            Console.ReadKey();
        }
    }
}
