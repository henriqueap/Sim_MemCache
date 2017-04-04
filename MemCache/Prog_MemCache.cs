using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemCache
{
    class Prog_MemCache
    {
        // Função que converte um valor binário em decimal
        public static string BinToDec(string val_bin)
        {
            int val_dec = 0;
            int aux = 0;
            int i = -1;
            char[] arrChar = val_bin.ToCharArray();
            Array.Reverse(arrChar);
            string val_bin_inv = new String(arrChar);
            //Console.WriteLine(val_bin_inv);
            foreach (char val in val_bin_inv)
            {
                i++;
                if (val.ToString() == "1")
                {
                    if (i == 0)
                        val_dec = val_dec + 1;
                    else
                    {
                        aux = 1;
                        for (int j = 1; j <= i; j++)
                        {
                            aux = (aux * 2);
                        }
                    }
                    val_dec = val_dec + aux;
                }
            }
            return val_dec.ToString();
        }

        // Função que converte um valor hexadecimal em binário
        public static string HexToBin(string val_hex)
        {
            string val_dec;
            string val_bin = "";
            string[] bin_lista = {"0000", "0001", "0010", "0011", "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011", "1100", "1101", "1110", "1111" };
            int count = 0;
            foreach (char val in val_hex)
            {
                count++;
                if (val.ToString().ToUpper() == "A")
                    val_dec = "10";
                else if (val.ToString().ToUpper() == "B")
                    val_dec = "11";
                else if (val.ToString().ToUpper() == "C")
                    val_dec = "12";
                else if (val.ToString().ToUpper() == "D")
                    val_dec = "13";
                else if (val.ToString().ToUpper() == "E")
                    val_dec = "14";
                else if (val.ToString().ToUpper() == "F")
                    val_dec = "15";
                else 
                    val_dec = val.ToString();
                //Console.WriteLine("{0}: {1}", count, bin_lista[Convert.ToUInt16(val_dec)]);
                //Console.ReadKey();
                val_bin = val_bin + bin_lista[Convert.ToUInt16(val_dec)];
                if (count < val_hex.Length)
                    val_bin = val_bin +  " ";
            }
            return val_bin;
        }

        // Função que testa se houve HIT
        public static bool TestHit(string[] arr, string inst, int filter)
        {
            string inst_bin = HexToBin(inst.Substring(4, 8)).Replace(" ", "");
            string index = inst_bin.Substring(32 - filter, filter);
            string tag = inst_bin.Substring(0, 32 - filter);
            int i = Convert.ToUInt16(BinToDec(index));
            //Console.WriteLine("inst: {0}, index: {1} e tag: {2}", inst_bin, index, tag);
            //Console.ReadKey();

            if (arr[i] == "X")
                return false;
            else
                return (tag.Equals(arr[i], StringComparison.OrdinalIgnoreCase));
        }
        
        static void Main(string[] args)
        {
            // Declaração das variáveis
            UInt16 index;
            int hits = 0;
            int miss = 0;
            int total = 0;
            string index_inst;
            string line;
            string tag;
            string trace_file;

            // Solicitando o número de linhas da cache
            Console.Write("Digite quantas linhas sua cache terá, ela precisa ser potência de 2 e ter no máximo 1024 linhas: ");
            string rows_cache = Console.ReadLine();
            switch (rows_cache)
            {
                case "2":
                    index = 1;
                    break;
                case "4":
                    index = 2;
                    break;
                case "8":
                    index = 3;
                    break;
                case "16":
                    index = 4;
                    break;
                case "32":
                    index = 5;
                    break;
                case "64":
                    index = 6;
                    break;
                case "128":
                    index = 7;
                    break;
                case "256":
                    index = 8;
                    break;
                case "512":
                    index = 9;
                    break;
                default:
                    index = 10;
                    break;
            }
            string[] mem_cache = new string[Convert.ToUInt16(rows_cache)];
            // Populando a cache com sinal de inativo: "X"
            for (int i = 0; i <= mem_cache.Length - 1; i++)
            {
                mem_cache[i] = "X";
            }
            Console.WriteLine("Sua cache terá {0} linhas, e o índice será de {1} dígitos.", rows_cache, index.ToString());

            // Solicitando o nome do arquivo de trace
            Console.Write("Digite o nome do arquivo de trace: ");
            trace_file = "../../" + Console.ReadLine() + ".trace";

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(@trace_file);
            while ((line = file.ReadLine()) != null)
            {
                total++;
                if (TestHit(mem_cache, line, index))
                {
                    hits++;
                    index_inst = HexToBin(line.Substring(4, 8)).Replace(" ", "").Substring(32 - index, index);
                    tag = HexToBin(line.Substring(4, 8)).Replace(" ", "").Substring(0, 32 - index);
                    //Console.WriteLine("Index = {0}: {1} == {2} HIT!", index_inst, tag, mem_cache[Convert.ToUInt16(BinToDec(index_inst))]);
                }
                else
                {
                    miss++;
                    index_inst = HexToBin(line.Substring(4, 8)).Replace(" ", "").Substring(32 - index, index);
                    tag = HexToBin(line.Substring(4, 8)).Replace(" ", "").Substring(0, 32 - index);
                    //Console.WriteLine("Index = {0}: {1} != {2} MISS!", index_inst, tag, mem_cache[Convert.ToUInt16(BinToDec(index_inst))]);
                    // Alterando o conteúdo da linha
                    mem_cache[Convert.ToUInt16(BinToDec(index_inst))] = tag;
                }
            }
            file.Close();

            Console.Write("Houveram {0} hits, {1} miss de um total de {2} instruções. Taxa de Hits: {0}/{2} ({3:0.00}%)", hits, miss, total, ((100*hits)/total).ToString());
            // Suspend the screen.
            Console.ReadKey();
        }
    }
}