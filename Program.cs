using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Exp
{
    class Program
    {
        static void Main(string[] args)
        {
            string fpath = @"D:/Data.csv"; //File path

            FileStream fs = new FileStream(fpath, FileMode.Open, FileAccess.ReadWrite);     // File stream object to access file in mentioned mode.
            //streambuilder objects for json file
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
                            values[1] = values[1].Replace(",", " ");
                        line = string.Empty;
                        foreach (var a1 in values)
                        {
                            line += a1;
                        }
                        list.Add(line);
                    }
                }
            }

            //Converting list to array
            string[] result = list.ToArray();

            int[] counter2 = new int[30];   //counters to calculate values for first json file.
            int[] counter3 = new int[30];   //counters to calculate values for second json file.
            int[] counter4 = new int[4];    //counters to calculate values for third json file.

            int a = 2001;
            int intYear, intFBIcode;

            foreach (var i in result)   // Taking one row at a time
            {
                           var inRow = i.Split(',');      // Splitting row with "," delimiter
                          
                            if (int.TryParse(inRow[17], out intYear))   // converting year (string) to year (integer)
                            {
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
                                if (intYear == 2015)        //only for 2015
                                {
                                    if (int.TryParse(inRow[14], out intFBIcode))
                                    {
                                        //Indexed-Crime
                                        if (inRow[14] == "01A" || inRow[14] == "04A" || inRow[14] == "04B" || inRow[14] == "04A" || intFBIcode == 02 || intFBIcode == 03 || intFBIcode == 05 || intFBIcode == 06 || intFBIcode == 07 || intFBIcode == 09)
                                        {
                                            counter4[0] = counter4[0] + 1;
                                        }
                                        //Non-indexed Crime
                                        if (inRow[14] == "01B" || inRow[14] == "08A" || inRow[14] == "08B" || inRow[14] == "04A" || (intFBIcode <= 10 && intFBIcode <= 20) || intFBIcode == 22 || intFBIcode == 24 || intFBIcode == 26)
                                        {
                                            counter4[1] = counter4[1] + 1;
                                        }
                                        //Violent Crime
                                        if (inRow[14] == "01A" || inRow[14] == "04A" || inRow[14] == "04B" || intFBIcode == 02 || intFBIcode == 03 || intFBIcode == 14)
                                        {
                                            counter4[2] = counter4[2] + 1;
                                        }
                                        //Property Crime
                                        if (intFBIcode == 05 || intFBIcode == 06 || intFBIcode == 07 || intFBIcode == 09)
                                        {
                                            counter4[3] = counter4[3] + 1;
                                        }
                                    }
                                }
                            }                   
            }
            // Appending values of counter in file1 and file2.
            file1.Append("[").Append('\n');
            file2.Append('{').Append("\"ASSAULT\":").Append("[").Append('\n');
            
            for (int k = 0; k < 29; k = k + 2)
            {
                file1.Append('{').AppendFormat("\"YEAR\":{0}", a + (k / 2)).Append(',').Append('\n').AppendFormat("\"OVER_$500\":{0}", counter2[k]).Append(',').Append('\n').AppendFormat("\"$500_AND_UNDER\":{0}", counter2[k + 1]).Append('}'); 
                file2.Append('{').AppendFormat("\"YEAR\":{0}", a + (k / 2)).Append(',').Append('\n').AppendFormat("\"Arrest\":{0}", counter3[k]).Append(',').Append('\n').AppendFormat("\"Not_Arrest\":{0}", counter3[k + 1]).Append('}');

                if (a + (k / 2) != 2015)
                {
                    file1.Append(',').Append('\n');
                    file2.Append(',').Append('\n');
                }
            }
            file1.Append(']');
            file2.Append(']').Append('}');
            // Appending values of counter in file3.
            file3.Append("[").Append('\n').Append('{').AppendFormat("\"Crime\":\"Indexed_Crime\"").Append(',').AppendFormat("\"Value\":{0}", counter4[0]).Append('}').Append(',').Append('\n').Append('{').AppendFormat("\"Crime\":\"Non-Indexed_Crime\"").Append(',').AppendFormat("\"Value\":{0}", counter4[1]).Append('}').Append(',').Append('\n').Append('{').AppendFormat("\"Crime\":\"Violent_Crime\"").Append(',').AppendFormat("\"Value\":{0}", counter4[2]).Append('}').Append(',').Append('\n').Append('{').AppendFormat("\"Crime\":\"Property_Crime\"").Append(',').AppendFormat("\"Value\":{0}", counter4[3]).Append('}').Append('\n').Append(']');
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
        }
    }
}

