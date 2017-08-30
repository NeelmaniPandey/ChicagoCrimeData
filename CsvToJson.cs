using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
[assembly: System.Reflection.AssemblyTitleAttribute("CrimeData")]
[assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]

namespace CrimeData
{
    static class CsvToJson
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            string fpath = "Data/Data.csv";     //Data File path.

            FileStream fs = new FileStream(fpath, FileMode.Open, FileAccess.ReadWrite);     // File stream object to access file in mentioned mode.
            //streambuilder objects for json writing files.
            StringBuilder file1 = new StringBuilder();
            StringBuilder file2 = new StringBuilder();
            StringBuilder file3 = new StringBuilder();

            var list = new List<string>();
            // Adding into list
            using (var reader = new StreamReader(fs))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //Add only those line which have below mentioned substring (other line are not needed)
                    if (line.Contains("ASSAULT") || line.Contains("OVER $500") || line.Contains("$500 AND UNDER") || line.Contains("2015"))
                    {
                        string[] values = line.Split('"');
                        if (values.Length > 1)
                        {
                            for (int i = 1; i < values.Length; i += 2)
                            {
                                values[i] = values[i].Replace(",", " ");
                            }
                        }
                        line = string.Empty;
                        foreach (var a1 in values)
                        {
                            line += a1;
                        }
                        list.Add(line);
                    }
                }
            }

            string[] result = list.ToArray();   //Converting list to array
            int[] counter2 = new int[30];   //counters to calculate values for first json file.
            int[] counter3 = new int[30];   //counters to calculate values for second json file.
            int[] counter4 = new int[4];    //counters to calculate values for third json file.

            int a = 2001;
            int intYear;
            //Array Containing FBI codes for Indexed Crime.
            string[] IndexedCode = { "01A", "04A", "04B", "02", "03", "05", "06", "07", "09" };     
            string IndexedString = String.Join(",", IndexedCode);
            //Array Containing FBI codes for Non-Indexed Crime.
            string[] NonIndexedCode = { "01B", "08A", "08B", "04A", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "22", "24", "26" };
            string NonIndexedString = String.Join(",", NonIndexedCode);
            //Array Containing FBI codes for Vioplent Crime.
            string[] ViolentCode = { "01A", "04A", "04B", "02", "03", "14" };
            string ViolentString = String.Join(",", ViolentCode);
            //Array Containing FBI codes for Property Crime.
            string[] PropertyCode = { "05", "06", "07", "09" };
            string PropertyString = String.Join(",", PropertyCode);

            foreach (var i in result)   // Taking one row at a time
            {
                var inRow = i.Split(',');      // Splitting row with "," delimiter

                int.TryParse(inRow[17], out intYear);   // converting year (string) to year (integer)
                
                    for (int k = 0; k < 29; k = k + 2)      // 15 times loop
                    {
                        if (intYear == a + (k / 2))
                        {
                            //If Crime Description is OVER $500, increase counter by 1
                            if (inRow[6] == "OVER $500")
                                counter2[k] += 1;
                            //If Crime Description is $500 AND UNDER, increase counter by 1
                            if (inRow[6] == "$500 AND UNDER")
                                counter2[k + 1] += 1;
                            //If Crime Primary_type is Assault and Arrest is true, increase counter by 1
                            if (inRow[5] == "ASSAULT" && inRow[8] == "true")
                                counter3[k] += 1;
                            //If Crime Primary_type is Assault and Arrest is false, increase counter by 1                                       
                            if (inRow[5] == "ASSAULT" && inRow[8] == "false")
                                counter3[k + 1] += 1;
                        }
                    }
                    if (inRow[17] == "2015")        //only for 2015
                    {
                        //Indexed-Crime
                        if (IndexedString.Contains(inRow[14]))
                        {
                            counter4[0] = counter4[0] + 1;
                        }
                        //Non-indexed Crime
                        if (NonIndexedString.Contains(inRow[14]))
                        {
                            counter4[1] = counter4[1] + 1;
                        }
                        //Violent Crime
                        if (ViolentString.Contains(inRow[14]))
                        {
                            counter4[2] = counter4[2] + 1;
                        }
                        //Property Crime
                        if (PropertyString.Contains(inRow[14]))
                        {
                            counter4[3] = counter4[3] + 1;
                        }
                    }
                
            }
            // Appending values of respective counters in file1 and file2.

            file1.AppendLine("[ \n");
            file2.AppendLine("{ \n \"ASSAULT\": [ \n");
            for (int k = 0; k < 29; k = k + 2)
            {
                file1.AppendLine(" { \"YEAR\":"+ (a + (k / 2)) +", \n \"OVER $500\":" + counter2[k]+", \n \"$500 AND UNDER\":"+ counter2[k + 1] +"}");
                file2.AppendLine(" { \"YEAR\":" + (a + (k / 2)) + ", \n \"Arrest\":" + counter3[k] + ", \n \"Not_Arrest\":" + counter3[k + 1] + "}");

                if (a + (k / 2) != 2015)
                {
                    file1.AppendLine(", \n");
                    file2.AppendLine(", \n");
                }
            }
            file1.Append(']');
            file2.AppendLine("] }");
            // Appending values of counters in file3.
            file3.AppendLine(" [ \n { \"Crime\":\"Indexed_Crime\" , \"Value\":"+ counter4[0]+ "} , \n { \"Crime\":\"Non-Indexed_Crime\" , \"Value\":"+ counter4[1] + " } , \n { \"Crime\":\"Violent_Crime\" , \"Value\":"+ counter4[2]+ " } , \n { \"Crime\":\"Property_Crime\" , \"Value\":"+ counter4[3]+"} \n ]");
            //Writing into first json file
            string fp1 = @"json/barchart.json";
            FileStream fs2 = new FileStream(fp1, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs2);
            sw.WriteLine(file1);
            sw.Flush();
            //Writing into second json file
            string fp2 = @"json/linechart.json";
            FileStream fs3 = new FileStream(fp2, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            sw = new StreamWriter(fs3);
            sw.WriteLine(file2);
            sw.Flush();
            //Writing into third json file
            string fp3 = @"json/piechart.json";
            FileStream fs4 = new FileStream(fp3, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            sw = new StreamWriter(fs4);
            sw.WriteLine(file3);
            sw.Flush();

            watch.Stop();
            var elapsedMs = watch.Elapsed;
            Console.WriteLine(elapsedMs);
        }
    }
}

