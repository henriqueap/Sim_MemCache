using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemCache
{
    class Prog_MemCache
    {
        // Função para configurar a instrução do arquivo trace
        public static string ConfigInst(string val_hex)
        {
            // retirando os dois primeiros caracteres da instrução para ficar só o HEX válido
            string new_inst = val_hex.Substring(2, val_hex.Length - 2);
            if (new_inst.Length < 8)
            {
                // completando com zeros a esquerda o HEX, se necessário, até ter 8 caracteres
                while (new_inst.Length < 8)
                    new_inst = new_inst.Insert(0, "0");
            }
            //Console.WriteLine("{0}", new_inst);
            return new_inst;
        }
        
        // Função que converte um valor binário em decimal
        public static string BinToDec(string val_bin)
        {
            int val_dec = 0; // valor em DEC
            int aux = 0;
            int i = -1;
            char[] arrChar = val_bin.ToCharArray(); // dígitos do valor BIN
            Array.Reverse(arrChar); // invertendo a ordem do BIN
            string val_bin_inv = new String(arrChar);
            //Console.WriteLine(val_bin_inv);
            // lendo o BIN de trás para frente para poder converter em DEC
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
            string val_dec; // valor em DEC
            string val_bin = "";
            // de 0 a 15 em BIN de quatro dígitos
            string[] bin_lista = {"0000", "0001", "0010", "0011", "0100", "0101", "0110", "0111", "1000", "1001", "1010", "1011", "1100", "1101", "1110", "1111" };
            int count = 0;
            foreach (char val in val_hex)
            {
                count++;
                // convertendo cada letra/dígito do HEX
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
        
        // Função que testa se houve HIT (True) ou MISS (False)
        public static bool TestHit(string[,] cache, string inst, int r, int w)
        {
            // converte a instrução HEX em BIN
            string inst_bin = HexToBin(inst).Replace(" ", "");
            // captura o índice BIN da palavra
            string word = inst_bin.Substring(inst_bin.Length - w, w);
            // captura o índice BIN da linha
            string row = inst_bin.Substring(inst_bin.Length - (r+w), r);
            // captura a tag BIN
            string tag = inst_bin.Substring(0, 32 - w - r);
            // convertendo os índices de BIN para DEC
            int row_index = Convert.ToUInt16(BinToDec(row));
            int word_index  = Convert.ToUInt16(BinToDec(word));
            //Console.Write("Cache[{0},{1}] = {2}", row, word, tag);

            // comparando o conteúdo da tag com a que está na cache e retornando true or false
            if (cache[row_index, word_index].Equals("X"))
                return false;
            else
                return (tag.Equals(cache[row_index, word_index]));
        }
        
        static void Main(string[] args)
        {
            // Declaração de variáveis
            int hits = 0; // total de hits
            int miss = 0; // total de miss
            int total = 0; // total de instruções lidas
            string inst_hex; // instrução em HEX
            string index_word_bin; // índice da palavra em BIN
            string index_row_bin; // índice da linha em BIN
            string tag; // tag em BIN
            string trace_file; // nome do arquivo
            string line_file; // linha do arquivo

            // Solicitando o número de linhas da cache
            Console.Write("Digite quantas linhas sua cache terá, ela precisa ser potência de 2 e ter no máximo 1024 linhas: ");
            double rows_cache = double.Parse(Console.ReadLine()); // nova variável para receber o nº de linhas
            int index_row_size = Convert.ToUInt16(Math.Log(rows_cache, 2)); // nova variável para receber o tamanho do índice da linha
            Console.Write("Digite quantas palavras sua cache terá, ela precisa ser potência de 2 e ter no máximo 16 palavras: ");
            double words_cache = double.Parse(Console.ReadLine()); // nova variável para receber o nº de palavras
            int index_word_size = Convert.ToUInt16(Math.Log(words_cache, 2)); // nova variável para receber o tamanho do índice da palavra
            string[,] mem_cache = new string[Convert.ToUInt16(rows_cache), Convert.ToUInt16(words_cache)];
            Console.WriteLine("Sua cache terá {0} linhas com {1} palavras e os índices serão de {2} e {3} dígito(s), respectivamente.", rows_cache.ToString(), words_cache.ToString(), index_row_size.ToString(), index_word_size.ToString());

            // Populando a cache com sinal de inativo: "X"
            for (int i = 0; i <= Convert.ToUInt16(rows_cache) - 1; i++)
            {
                for (int j = 0; j <= Convert.ToUInt16(words_cache) - 1; j++)
                {
                    mem_cache[i, j] = "X";
                    //Console.WriteLine("Cache[{0}, {1}]= {2}", i, j, mem_cache[i, j]);
                }
                
            }

            // Solicitando o nome do arquivo de trace que está na pasta traces do projeto
            Console.Write("Digite o nome do arquivo de trace: ");
            trace_file = "../../traces/" + Console.ReadLine() + ".trace";

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(@trace_file);

            while ((line_file = file.ReadLine()) != null)
            {
                // só contabiliza o hit/miss se a primeira instrução for "2"
                if (line_file[0].ToString().Equals("2"))
                {
                    inst_hex = ConfigInst(line_file);
                    total++;
                    // testando se foi HIT ou MISS
                    if (TestHit(mem_cache, inst_hex, index_row_size, index_word_size))
                    {
                        hits++;
                        index_word_bin = HexToBin(inst_hex).Replace(" ", "").Substring(32 - index_word_size, index_word_size);
                        index_row_bin = HexToBin(inst_hex).Replace(" ", "").Substring(32 - (index_row_size + index_word_size), index_row_size);
                        tag = HexToBin(inst_hex).Replace(" ", "").Substring(0, 32 - (index_word_size + index_row_size));
                        //Console.WriteLine(" == {0} HIT!", mem_cache[Convert.ToUInt16(BinToDec(index_row_bin)), Convert.ToUInt16(BinToDec(index_word_bin))]);
                    }
                    else
                    {
                        miss++;
                        index_word_bin = HexToBin(inst_hex).Replace(" ", "").Substring(32 - index_word_size, index_word_size);
                        index_row_bin = HexToBin(inst_hex).Replace(" ", "").Substring(32 - (index_row_size + index_word_size), index_row_size);
                        tag = HexToBin(inst_hex).Replace(" ", "").Substring(0, 32 - (index_word_size + index_row_size));
                        //Console.WriteLine(" != {0} MISS!", mem_cache[Convert.ToUInt16(BinToDec(index_row_bin)), Convert.ToUInt16(BinToDec(index_word_bin))]);
                        // Alterando o conteúdo da linha
                        mem_cache[Convert.ToUInt16(BinToDec(index_row_bin)), Convert.ToUInt16(BinToDec(index_word_bin))] = tag;
                    }
                }
            }
            file.Close();

            Console.Write("Houveram {0} hits, {1} miss de um total de {2} instruções. Taxa de Hits: {0}/{2} ({3:0.00}%)", hits, miss, total, ((100*hits)/total).ToString());
            // Suspend the screen.
            Console.ReadKey();
        }
    }
}